using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MouthFeatures;

namespace SMT.Online{
public class MouthVideo : MonoBehaviour {

	byte []black;
	Featurer featurer;
	Texture2D mouthTexture;

	void Awake(){
		// Init dll.
		featurer = Featurer.GetMouthFeatures();
	}

	// Use this for initialization
	void Start () {
		// Init mouth texture.
		Material mat = transform.Find("Visor/Head").GetComponent<Renderer>().material;
		black = new byte[featurer.width * 1024 * 3];
		mouthTexture = new Texture2D(featurer.width, 1024, TextureFormat.RGB24, false);
		// mouthTexture = new Texture2D(500, 460, TextureFormat.RGB24, false);
		mat.mainTextureScale = new Vector2(-1, -1);
		mat.SetTexture("_MainTex", mouthTexture);
		mat.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		// Update texture.
		int status;
		byte[] frame = featurer.GetVideo(out status);

		if(status == (int)Errors.OK){		
			// Copy the video in the buffer.
			Debug.Log("FRAME");
			CopyVideoOnTexture(frame);
			mouthTexture.LoadRawTextureData(black);
			mouthTexture.Apply();
		}
		else
			Debug.Log((Errors) status);
	}

	/// <summary>
	/// 	Copies the video on the head texture.
	/// </summary>
	/// <param name="video"> The video to copy. </param>
	void CopyVideoOnTexture(byte[] video){
		int baseRow = 0, baseCol = 0;

		for(int i = 0; i < video.Length; i += 3){
			// Compute the video row and column and then the row and column of the texture on which copy the video.
			int row = (i / 3) / mouthTexture.width, col = (i / 3) % mouthTexture.width;
			int textureRow = row + baseRow, textureCol = col + baseCol;
			int j = (textureRow * mouthTexture.width + textureCol) * 3;

			black[j] = video[i];
			black[j + 1] = video[i + 1];
			black[j + 2] = video[i + 2];
		}
	}
}

}