using UnityEngine;
using UnityEngine.Networking;

using Valve.VR.InteractionSystem;
using SMT.Common;

namespace SMT.Online{

public class OnlineVRPlayer : NetworkBehaviour {
	public GameObject teleporting;

	/// <summary>
	/// 	Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake() {
		// Disable player script.
		if(!isLocalPlayer)
			GetComponent<Valve.VR.InteractionSystem.Player>().enabled = false;
	}

	
	/// <summary>
	/// 	Start is called on the frame when a script is enabled just before
	/// 	any of the Update methods is called the first time.
	/// </summary>
	void Start () {
		transform.Find("Camera (eye)").gameObject.SetActive(true);

		// Delete tracking components, camera and audio source.
		if(!isLocalPlayer){
			GetComponent<SteamVR_ControllerManager>().enabled = false;
			GetComponent<SteamVR_PlayArea>().enabled = false;

			Destroy(transform.Find("Controller (left)").gameObject.GetComponent<SteamVR_TrackedObject>());
			Destroy(transform.Find("Controller (right)").gameObject.GetComponent<SteamVR_TrackedObject>());
			Destroy(transform.Find("Controller (left)").gameObject.GetComponent<VRKeyHandler>());
			Destroy(transform.Find("Controller (right)").gameObject.GetComponent<VRKeyHandler>());

			Destroy(transform.Find("Camera (eye)").gameObject.GetComponent<SteamVR_Camera>());
			Destroy(transform.Find("Camera (eye)").gameObject.GetComponent<FlareLayer>());
			Destroy(transform.Find("Camera (eye)").gameObject.GetComponent<Camera>());

			Destroy(transform.Find("Camera (eye)/Camera (ears)").gameObject.GetComponent<SteamVR_Ears>());
			Destroy(transform.Find("Camera (eye)/Camera (ears)").gameObject.GetComponent<AudioListener>());
		}
		else{
			// Teleport handling.
			GameObject teleport = Instantiate(teleporting);
			teleport.name = "Teleporting";
			GameObject.Find("Room/Fake Floor").AddComponent<TeleportArea>();

			// Delete lipsync component
			Destroy(transform.Find("Camera (eye)/Camera (mouth)/InputType_Microphone").gameObject);
		}
	}

	/// <summary>
	/// 	This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	void OnDestroy() {
		if(isServer)
			NetworkManager.singleton.StopHost();
		else
			NetworkManager.singleton.StopClient();
	}
}

}