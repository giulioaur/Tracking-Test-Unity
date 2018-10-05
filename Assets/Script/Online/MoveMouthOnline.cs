using UnityEngine;
using UnityEngine.Networking;

using MouthFeatures;
using SMT.Common;

namespace SMT.Online{
	
public class MoveMouthOnline : NetworkBehaviour {
	// Mouth stuffs.
	Mouth mouth;
	MouthSettings settings = null;
	public SkinnedMeshRenderer lips;
	
	enum BlendShapeIndex{
		sil = 0, PP = 1, FF = 2, TH = 3, DD = 4, kk = 5, CH = 6, SS = 7, nn = 8, RR = 9, aa = 10,
		E = 11, ih = 12, oh = 13, ou = 14, smile = 15, frown = 16
	};

	// Use this for initialization
	void Awake () {
		if(GameObject.Find("Mouth") != null && Featurer.lipFeed){
			mouth = GameObject.Find("Mouth").GetComponent<Mouth>();
			settings = GameObject.Find("Mouth").GetComponent<MouthSettings>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer && settings != null){
			float verticalOpeness = ((int)mouth.points[5] - mouth.points[1]);
			float horizontalOpeness = ((int)mouth.points[8] - mouth.points[0]);
			float verticalMagnitude = verticalOpeness / settings.maxVOpeness * 100;

			RpcUpdateMouth(verticalOpeness, horizontalOpeness, verticalMagnitude, settings.smileVOpeness, settings.normalHOpeness);
		}
	}

	/// <summary>
	/// 	Updates the mouth on the client.
	/// </summary>
	/// <param name="verticalOpeness"> The vertical openess. </param>
	/// <param name="horizontalOpeness"> The horizontal openess. </param>
	/// <param name="verticalMagnitude"> The vertical magnitude. </param>
	[ClientRpc]
	void RpcUpdateMouth(float verticalOpeness, float horizontalOpeness, float verticalMagnitude, float smileVOpeness, float normalHOpeness){
		if(verticalOpeness > smileVOpeness){				// Mouth opened
			lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, 0);
			lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, 0);
			lips.SetBlendShapeWeight((int)BlendShapeIndex.aa, verticalMagnitude);
			if(horizontalOpeness > normalHOpeness){ 		// Smiling opening
				lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, 0);
			}
			else{
				lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, (horizontalOpeness - normalHOpeness) * 2);
			}
		}	
		else{														// Mouth closed
			lips.SetBlendShapeWeight((int)BlendShapeIndex.aa, 0);
			if(horizontalOpeness > normalHOpeness){		// Smiling opening
				lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, (horizontalOpeness - normalHOpeness) * 3);
				lips.SetBlendShapeWeight((int)BlendShapeIndex.oh, 0);
			}
			else{													// Normal opening	
				lips.SetBlendShapeWeight((int)BlendShapeIndex.oh, (normalHOpeness - horizontalOpeness) * 3);
				lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, 0);
			}
		}
	} 
}

}