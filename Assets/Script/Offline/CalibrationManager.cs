using UnityEngine;
using UnityEngine.UI;

using MouthFeatures;
using SMT.Common;

namespace SMT.Offline{
public class CalibrationManager : MonoBehaviour {
	public VRKeyHandler controller;
	// Calibration button.
	Text buttonText;
	// The dll wrapper.
	Featurer featurer;
	// The mouth interface.
	Mouth mouth;
	public MouthSettings settings;
	Material image, normImage, lipImage;
	bool isCalibrating;

	// Use this for initialization
	void Start () {
		// Check if the cam is plugged in the headset.
		if((featurer = Featurer.GetMouthFeatures()) == null || !Featurer.lipFeed){
			mouth = null;
			gameObject.SetActive(false);
		}
		else{	
			buttonText = transform.Find("Calibration Canvas/Button/Text").GetComponent<Text>();
			// Set image texture.
			image = transform.Find("Image").GetComponent<Renderer>().material;
			normImage = transform.Find("Normalized Image").GetComponent<Renderer>().material;
			lipImage = transform.Find("Lip Image").GetComponent<Renderer>().material;

			mouth = GameObject.Find("Mouth").GetComponent<Mouth>();
			mouth.SetMouthTexture(Mouth.ImageType.STANDARD, image);
			mouth.SetMouthTexture(Mouth.ImageType.NORMALIZED, normImage);
			mouth.SetMouthTexture(Mouth.ImageType.LIP, lipImage);
		
			// Set keys for color change.
			#if USE_VR 

			controller.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.AXIS0, ChangeParameters);
			
			#else

			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.RightArrow, IncreaseGreen);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.LeftArrow, DecreaseGreen);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.UpArrow, IncreaseBlue);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.DownArrow, DecreaseBlue);
			
			#endif
		}
	}

	/// <summary>
	/// 	Changes the color of the filter or the height of the line between lips
	/// 	basing on the current phase.
	/// </summary>
	/// <param name="hit"></param>
	void ChangeParameters(RaycastHit hit){
		switch(controller.GetPadDirection()){
			case VRKeyHandler.PadDirection.UP:
				if (isCalibrating) 	featurer.MoveMiddleLine(++settings.lineHeight);
				else				featurer.SetColorFilter(settings.green, ++settings.blue);
				break;
			case VRKeyHandler.PadDirection.DOWN:
				if (isCalibrating) 	featurer.MoveMiddleLine(--settings.lineHeight);
				else				featurer.SetColorFilter(settings.green, --settings.blue);
				break;
			case VRKeyHandler.PadDirection.RIGHT:
				if (!isCalibrating) featurer.SetColorFilter(++settings.green, settings.blue);
				break;
			case VRKeyHandler.PadDirection.LEFT:
				if (!isCalibrating)	featurer.SetColorFilter(--settings.green, settings.blue);
				break;
		}
	}
	
	// Functions to increase and decrease color filters.
	void IncreaseBlue(){
		if (isCalibrating)	featurer.SetColorFilter(settings.green, ++settings.blue);
	}
	void DecreaseBlue(){
		if (isCalibrating)	featurer.SetColorFilter(settings.green, --settings.blue);
	}
	void IncreaseGreen(){
		if (isCalibrating)	featurer.SetColorFilter(++settings.green, settings.blue);
	}
	void DecreaseGreen(){
		if (isCalibrating)	featurer.SetColorFilter(--settings.green, settings.blue);
	}

	/// <summary>
	/// 	Turn on/off calibration.
	/// </summary>
	public void SwitchCalibration(){
		if(isCalibrating) 	featurer.StopCalibration();
		else				featurer.StartCalibration();

		isCalibrating ^= true;
		buttonText.text = "Calibration: " + (isCalibrating ? "ON" : "OFF");
	}
	
	void OnDestroy(){
		if(mouth != null){
			mouth.RemoveTexture(image);
			mouth.RemoveTexture(normImage);
			mouth.RemoveTexture(lipImage);

			KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.RightArrow, IncreaseGreen);
			KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.LeftArrow, DecreaseGreen);
			KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.UpArrow, IncreaseBlue);
			KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.DownArrow, DecreaseBlue);
		}
	}
}
}