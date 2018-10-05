using UnityEngine;
using UnityEngine.Networking;

using SMT.Common;

namespace SMT.Online{

public class Player : NetworkBehaviour {

	public float speed = 1;
	public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

	// Use this for initialization
	void Start () {
		if(isLocalPlayer){
			GameObject mainCamera = GameObject.Find("Main Camera");
			mainCamera.transform.SetParent(transform);
			mainCamera.transform.localPosition = new Vector3(0, 0, -0.8f);
			mainCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);

			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.D, MoveRight);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.A, MoveLeft);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.W, MoveForward);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.S, MoveBackward);

			// Remove unused components.
			Destroy(GetComponent<OVRLipSyncContextMorphTarget>());
			Destroy(GetComponent<OVRLipSyncContext>());
		}
	}

	void Update () {
		if (isLocalPlayer){
			yaw += speedH * Input.GetAxis("Mouse X");
			pitch += speedV * Input.GetAxis("Mouse Y");

			transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
		}
    }

	void MoveForward(){
		transform.position -= transform.forward * speed * Time.deltaTime;
	}

	void MoveBackward(){
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void MoveLeft(){
		transform.position += transform.right * speed * Time.deltaTime;
	}

	void MoveRight(){
		transform.position -= transform.right * speed * Time.deltaTime;
	}

	void OnDelete(){
		KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.D, MoveRight);
		KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.A, MoveLeft);
		KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.W, MoveForward);
		KeyboardHandler.RemoveCallback(KeyboardHandler.Map.KEY_PRESSED, KeyCode.S, MoveBackward);
	}
}

}