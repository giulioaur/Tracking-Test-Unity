using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SMT.Common;

namespace SMT.Offline{
public class MoveMouth : MonoBehaviour {
	// Mouth stuffs.
	Mouth mouth;
	public MouthSettings settings;
	SkinnedMeshRenderer lips;
	
	enum BlendShapeIndex{
		sil = 0, PP = 1, FF = 2, TH = 3, DD = 4, kk = 5, CH = 6, SS = 7, nn = 8, RR = 9, aa = 10,
		E = 11, ih = 12, oh = 13, ou = 14, smile = 15, frown = 16
	};

	// Use this for initialization
	void Start () {
		if(GameObject.Find("Mouth") != null){
			lips = transform.Find("Lips").GetComponent<SkinnedMeshRenderer>();
			mouth = GameObject.Find("Mouth").GetComponent<Mouth>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		float verticalOpeness = ((int)mouth.points[5] - mouth.points[1]);
		float horizontalOpeness = ((int)mouth.points[8] - mouth.points[0]);
		float verticalMagnitude = verticalOpeness / settings.maxVOpeness * 100;

		if(verticalOpeness > settings.smileVOpeness){				// Mouth opened
			lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, 0);
			lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, 0);
			lips.SetBlendShapeWeight((int)BlendShapeIndex.aa, verticalMagnitude);
			if(horizontalOpeness > settings.normalHOpeness){ 		// Smiling opening
				lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, 0);
			}
			else{
				lips.SetBlendShapeWeight((int)BlendShapeIndex.ou, (horizontalOpeness - settings.normalHOpeness) * 2);
			}
		}	
		else{														// Mouth closed
			lips.SetBlendShapeWeight((int)BlendShapeIndex.aa, 0);
			if(horizontalOpeness > settings.normalHOpeness){		// Smiling opening
				lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, (horizontalOpeness - settings.normalHOpeness) * 3);
				lips.SetBlendShapeWeight((int)BlendShapeIndex.oh, 0);
			}
			else{													// Normal opening	
				lips.SetBlendShapeWeight((int)BlendShapeIndex.oh, (settings.normalHOpeness - horizontalOpeness) * 3);
				lips.SetBlendShapeWeight((int)BlendShapeIndex.smile, 0);
			}
		}
	}
}
}