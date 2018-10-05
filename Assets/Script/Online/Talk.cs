using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

using SMT.Common;
using FragLabs.Audio.Codecs;

namespace SMT.Online{

public class Talk : NetworkBehaviour {
	// Controller.
	public VRKeyHandler controller;
	// Audio settings.
	const int FREQUENCY = 24000, secs = 4, FRAMESIZE = (FREQUENCY / 1000) * 20;
	static float[] cleaner = null;
	public float audioThreshold = 0.01f;
	// The microphone.
	static AudioClip mic = null;
	// The audio source.
	AudioSource audioSource;
	public AudioSource lipSource;
	// Opus decoder and encoder.
	static OpusDecoder decoder;
	static OpusEncoder encoder;
	static List<float> samples = new List<float>();
	int micLastPos = 0, lastPos = 0;
	public float volume = 1.1f;

	// bool isLocalPlayer = true;
	

	// Use this for initialization
	void Start () {
		// Init.
		if(mic == null){
			// Get microphone.
			mic = Microphone.Start(null, true, secs, FREQUENCY);

			// Create encoder and decoder.
			decoder = OpusDecoder.Create(FREQUENCY, mic.channels);
			encoder = OpusEncoder.Create(FREQUENCY, mic.channels, FragLabs.Audio.Codecs.Opus.Application.Voip);

			// Create array to clean the buffer.
			cleaner = new float[secs * FREQUENCY * mic.channels];
		}	

		// Create audio clip and cleaner for voice replication and lip sync.
		if(!isLocalPlayer){
		// if(isLocalPlayer){
			audioSource = transform.Find("Camera (eye)/Camera (mouth)").GetComponent<AudioSource>();
			audioSource.clip = AudioClip.Create("remoteVoice", secs * FREQUENCY, mic.channels, FREQUENCY, false);
			audioSource.clip.SetData(cleaner, 0);
			
			lipSource.clip = AudioClip.Create("remoteVoice", secs * FREQUENCY, mic.channels, FREQUENCY, false);
			lipSource.clip.SetData(cleaner, 0);
			controller.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.MENU, SwitchAudio);
		}
	}

	// Update is called once per frame
	void Update () {
		int pos;

		if(isLocalPlayer){
			if((pos = Microphone.GetPosition(null)) > 0){
				if(micLastPos > pos)
					micLastPos = 0;

				if(pos - micLastPos > 0){
					// Allocate the space for the sample.
					float[] sample = new float[(pos - micLastPos) * mic.channels];

					// Get the data from microphone.
					mic.GetData(sample, micLastPos); 

					// Check if sound is loud enough.
					bool filter = Filter(sample);
					for(int i = 0; i < sample.Length; ++i)
						sample[i] *= filter ? volume : 0;

					// Encode and send the data.
					Encode(sample);

					// Update position of the microphone.
					micLastPos = pos;	
				}
			}
		}
	}

	bool Filter(float[] audio){
		float volume = audio.AsEnumerable().Sum(item => Mathf.Abs(item)) / audio.Length;

		return volume > audioThreshold;
	}

	[Command(channel=1)]
	void CmdSend(byte[] audioData){
		// RpcSend(audioData, micLastPos);
		if(!isLocalPlayer)	Decode(audioData);
	}

	[ClientRpc(channel=1)]
	void RpcSend(byte[] audioData){
		if(!isLocalPlayer)	Decode(audioData);
	}

	/// <summary>
	/// 	Encodes and sends data.
	/// </summary>
	/// <param name="sample"> The uncompressed audio. </param>
	private void Encode(float[] sample){
		// Add the new audio data to the list of samples.
		samples.AddRange(sample);

		// While there is enough audio in the list.
		while(samples.Count >= FRAMESIZE){
			// Encode data.
			int length;
			byte[] audioData = encoder.Encode(samples.GetRange(0, FRAMESIZE).ToArray(), FRAMESIZE, out length);
			samples.RemoveRange(0, FRAMESIZE);

			// Resize packet to make it smaller before sending.
			byte[] packet = new byte[length];
			Buffer.BlockCopy(audioData, 0, packet, 0, length);
		
			// Send.
			if(isServer && NetworkServer.connections.Count > 1)
				RpcSend(packet);
			else
				CmdSend(packet);
			// Decode(packet);
		}
	}

	/// <summary>
	/// 	Decodes and play audio.
	/// </summary>
	/// <param name="data"> The compressed audio. </param>
	private void Decode(byte[] data){
		int decodedLength;		
		float[] sample = decoder.DecodeFloat(data, data.Length, out decodedLength);

		ReceiveAudio(sample, decodedLength);
	}

	/// <summary>
	/// 	Plays the audio.
	/// </summary>
	/// <param name="audioData"> The audio to play. </param>
	/// <param name="length"> The length of the audio in byte. </param>
	void ReceiveAudio(float[] audioData, int length){
		Debug.Log("Receiving audio...");

		// If the space is not enough, clean the buffer and put the index to the start.
		if(lastPos + length >= cleaner.Length){
			audioSource.clip.SetData(cleaner, 0);
			lipSource.clip.SetData(cleaner, 0);
			lastPos = 0;
		}

		// Put the data in the audio source.
		audioSource.clip.SetData(audioData, lastPos);
		lipSource.clip.SetData(audioData, lastPos);

		// Update position.
		lastPos += length;
				
		// Play if it is not playing.
		if(!audioSource.isPlaying)	audioSource.Play();
		if(!lipSource.isPlaying)	lipSource.Play();
	}

	/// <summary>
	/// 	Switches between 2D and 3D audio.
	/// </summary>
	/// <param name="hit"></param>
	void SwitchAudio(RaycastHit hit){
		audioSource.spatialBlend = audioSource.spatialBlend == 1 ? 0 : 1;
	}

	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	void OnDestroy(){	
		if(!isLocalPlayer)
			controller.RemoveCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.MENU, SwitchAudio);
	}
}

}