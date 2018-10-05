using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SMT.Common;

namespace SMT.Online{

public class ControllerAction : MonoBehaviour {
	// The script to broadcast the actions to the server.
	public PlayerControllerAction player;
	bool isRightController;
	// Object with which interact.
	public GameObject toGrab, grabbed;
	// Vive controller.
	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device Controller {
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}
	// Laser.
	InteractionLaser laser;

	/// <summary>
	/// 	Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake() {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		isRightController = gameObject.name.Contains("right");
	}

	/// <summary>
	/// 	Start is called on the frame when a script is enabled just before
	/// 	any of the Update methods is called the first time.
	/// </summary>
	void Start() {
		VRKeyHandler handler = GetComponent<VRKeyHandler>();
		toGrab = grabbed = null;
		laser = GetComponent<InteractionLaser>();
		player = transform.parent.GetComponent<PlayerControllerAction>();

		// Set key binding.
		handler.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.TRIGGER, (RaycastHit hit) => { player.CallCommand(PlayerControllerAction.commands.grab, isRightController, Vector3.zero, Vector3.zero); });
		handler.AddCallback(VRKeyHandler.Map.KEY_UP, VRKeyHandler.Key.TRIGGER, (RaycastHit hit) => { player.CallCommand(PlayerControllerAction.commands.release, isRightController, Controller.velocity, Controller.angularVelocity); });
		handler.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.GRIP, (RaycastHit hit) => { player.CallCommand(PlayerControllerAction.commands.showLaser, isRightController, Vector3.zero, Vector3.zero); });
		handler.AddCallback(VRKeyHandler.Map.KEY_UP, VRKeyHandler.Key.GRIP, (RaycastHit hit) => { player.CallCommand(PlayerControllerAction.commands.hideLaser, isRightController, Vector3.zero, Vector3.zero); });
		handler.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.TRIGGER, (RaycastHit hit) => { player.CallCommand(PlayerControllerAction.commands.grabCompanion, isRightController, Vector3.zero, Vector3.zero); });
	}

	/// <summary>
	/// 	OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.GetComponent<Rigidbody>())
			toGrab = other.gameObject;
	}

	/// <summary>
	/// 	OnTriggerExit is called when the Collider other has stopped touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerExit(Collider other) {
		if (toGrab != null)
			toGrab = null;
	}

	/// <summary>
	/// 	Grabs the object with which is colliding, if one.
	/// </summary>
	public void Grab(){
		if (toGrab != null){
			// Create a fixed joint with the in hand object.
			FixedJoint fx = gameObject.AddComponent<FixedJoint>();
			fx.breakForce = 20000;
			fx.breakTorque = 20000;

			grabbed = toGrab;
			toGrab = null;
			fx.connectedBody = grabbed.GetComponent<Rigidbody>();
		}
	}

	/// <summary>
	/// 	Releases the grabbed object if one.
	/// </summary>
	public void Release(Vector3 velocity, Vector3 angularVelocity){
		 if (GetComponent<FixedJoint>()) {
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());
			grabbed.GetComponent<Rigidbody>().velocity = velocity;
			grabbed.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
		}

		grabbed = null;
	}

	/// <summary>
	/// 	Shows the laser that starts from controller.
	/// </summary>
	public void ShowLaser(){
		laser.enabled = true;
	}

	/// <summary>
	/// 	Hides the laser that starts from controller.
	/// </summary>
	public void HideLaser(){
		laser.enabled = false;
	}

	/// <summary>
	/// 	Grab the companion cube if hit by the laser.
	/// </summary>
	public void GrabCompanion(){
		GameObject hit = laser.hit;

		if(laser.enabled && toGrab == null && hit != null && hit.layer == 8){
			toGrab = hit;
			Grab();
		}
	}
}

}