using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

	public float speed;

    float yaw = 0.0f;
    float pitch = 0.0f;

	void Update () {		
		if(Input.GetKey(KeyCode.W)){
			pitch = Mathf.Min(pitch + speed * Time.deltaTime, 15);
		}
		else if(Input.GetKey(KeyCode.S)){
			pitch = Mathf.Max(pitch - speed * Time.deltaTime, 0);
		}
		else if(Input.GetKey(KeyCode.A)){
			yaw -= speed * Time.deltaTime;
		}
		else if(Input.GetKey(KeyCode.D)){
			yaw += speed * Time.deltaTime;
		}

		transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
	}
}
