using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MouthFeatures;
using SMT.Common;

namespace SMT.Offline{

public class TrainingManager : MonoBehaviour {
	public VRKeyHandler controller;
	// The material on which show the image.
	Material image;
	// Mouth settings.
	public MouthSettings settings;
	public Mouth mouth;

	// Keep track of last frame.
	byte lastFrame;

	// The training phase.
	enum training : ushort{
		NORMAL = 1, SMILE = 2, OPENED = 3
	}
	ushort trainingPhase;
	Text phaseText;

	// Use this for initialization
	void Start () {
		// Check if the cam is plugged in the headset.
		if(GameObject.Find("Mouth") == null || !Featurer.lipFeed)
			gameObject.SetActive(false);
		else{
			mouth = GameObject.Find("Mouth").GetComponent<Mouth>();
			image = transform.Find("Training Image").GetComponent<Renderer>().material;
			phaseText = GameObject.Find("Training Canvas/Phase").GetComponent<Text>();
			trainingPhase = 0;

			#if USE_VR
			
			controller.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.GRIP, NextTraining);

			#else

			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.N, NextTraining);

			#endif
		}

	}

	/// <summary>
	/// 	Start the training.
	/// </summary>
	public void StartTraining(){
		// Show the image of the mouth.
		if(trainingPhase == 0)
			mouth.SetMouthTexture(Mouth.ImageType.STANDARD, image);

		trainingPhase = 1;
		phaseText.text = "NORMAL";
	}

	/// <summary>
	/// 	Go to the next phase of training.
	/// </summary>
	#if USE_VR
	public void NextTraining(RaycastHit hit){
	#else
	public void NextTraining(){
	#endif
		switch((training)(trainingPhase++)){
			case training.NORMAL:
				TrainNormal();
				phaseText.text = "SMILE";
				break;
			case training.SMILE:
				TrainSmile();
				phaseText.text = "OPENED";
				break;
			case training.OPENED:
				TrainOpened();
				phaseText.text = "END";
				mouth.RemoveTexture(image);
				trainingPhase = 0;
				break;
		}
	}

	/// <summary>
	/// 	Train the normal expression.
	/// </summary>
	void TrainNormal(){
		settings.normalVOpeness = ((int)mouth.points[5] - mouth.points[1]);
		settings.normalHOpeness = ((int)mouth.points[8] - mouth.points[0]);
	}

	/// <summary>
	/// 	Train the smile.
	/// </summary>
	void TrainSmile(){
		settings.smileVOpeness = ((int)mouth.points[5] - mouth.points[1]);
		settings.smileHOpeness = ((int)mouth.points[8] - mouth.points[0]);
	}

	/// <summary>
	/// 	Train the openess.
	/// </summary>
	void TrainOpened(){
		settings.maxVOpeness = ((int)mouth.points[5] - mouth.points[1]);
	}

	void OnDestroy(){
		#if USE_VR

		controller.RemoveCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.GRIP, NextTraining);

		#else

		KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.N, NextTraining);

		#endif
	}
}
}