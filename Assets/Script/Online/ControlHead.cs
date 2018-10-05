using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using SMT.Common;

namespace SMT.Online{
public class ControlHead : NetworkBehaviour {
	public VRKeyHandler controller;
	// The heads stuff.
	enum HeadType{
		TRACKING = 0, VIDEO = 1, LIPSYNC = 2
	}
	GameObject []heads;
	byte currentIndex;

	// Use this for initialization
	void Start () {
		// Find the three kind of heads.
		heads = new GameObject[3];
		#if USE_VR

		heads[0] = transform.Find("Camera (eye)/Head").gameObject;
		heads[1] = transform.Find("Camera (eye)/Talking Head").gameObject;
		heads[2] = transform.Find("Camera (eye)/LipSync Head").gameObject;
		if(isLocalPlayer && isServer){
			controller.AddCallback(VRKeyHandler.Map.KEY_DOWN, VRKeyHandler.Key.GRIP, NextHead);
		}

		#else

		heads[0] = transform.Find("Head").gameObject;
		heads[1] = transform.Find("Talking Head").gameObject;
		heads[2] = transform.Find("LipSync Head").gameObject;
		if(isLocalPlayer && isServer){
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.L, NextHead);
			KeyboardHandler.AddCallback(KeyboardHandler.Map.KEY_DOWN, KeyCode.K, PreviousHead);
		}

		#endif

		heads[1].SetActive(false); heads[2].SetActive(false);

		currentIndex = 0;

		
		
	}
	
	/// <summary>
	/// 	Go to the next head.
	/// </summary>
	#if USE_VR 
	void NextHead(RaycastHit hit){
	#else
	void NextHead(){
	#endif
	
		if(isLocalPlayer){
			Debug.Log("next");
			byte index = (byte) (currentIndex + 1 >= 3 ? 0 : currentIndex + 1);

			if(isServer && NetworkServer.connections.Count > 1)
				RpcChangeHead(index);
			else
				CmdChangeHead(index);
		}
	}

	/// <summary>
	/// 	Go to the previous head.
	/// </summary>
	void PreviousHead(){
		if(isLocalPlayer){
			byte index = (byte) (currentIndex - 1 < 0 ? 2 : currentIndex - 1);

			if(isServer && NetworkServer.connections.Count > 1)
				RpcChangeHead(index);
			else
				CmdChangeHead(index);
		}
	}

	/// <summary>
	/// 	Changes the head on the client.
	/// </summary>
	/// <param name="newIndex"> The index of the new head. </param>
	[Command]
	void CmdChangeHead(byte newIndex){
		heads[currentIndex].SetActive(false);
		currentIndex = newIndex;
		heads[currentIndex].SetActive(true);
	}

	/// <summary>
	/// 	Changes the head on the server.
	/// </summary>
	/// <param name="newIndex"> The index of the new head. </param>
	[ClientRpc]
	void RpcChangeHead(byte newIndex){
		heads[currentIndex].SetActive(false);
		currentIndex = newIndex;
		heads[currentIndex].SetActive(true);
	}

}
}