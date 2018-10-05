using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMT.Online {

public class InteractionLaser : MonoBehaviour {
	// The color of the ray.
	public Color color = Color.red;
	// The thickness of the ray.
    public float thickness = 0.002f;
	// The laser and its father.
	public GameObject laserPrefab;
	GameObject laser, holder, hitObject;
	Material laserMaterial;

	public GameObject hit{ get { return hitObject; }}
	
	/// <summary>
	/// 	Start is called on the frame when a script is enabled just before
	/// 	any of the Update methods is called the first time.
	/// </summary>
	void Start () {
		// Create laser as thick cube and hide it.
		holder = new GameObject();
        holder.transform.SetParent(transform);
        holder.transform.localPosition = Vector3.zero;
		holder.transform.localRotation = Quaternion.identity;

		laser = Instantiate(laserPrefab);
        laser.transform.SetParent(holder.transform);
        laser.transform.localScale = new Vector3(thickness, thickness, 100f);
        laser.transform.localPosition = new Vector3(0f, 0f, 50f);
		laser.transform.localRotation = Quaternion.identity;
		Destroy(laser.GetComponent<BoxCollider>());

		laserMaterial = new Material(Shader.Find("Unlit/Color"));
        laserMaterial.SetColor("_Color", color);
        laser.GetComponent<MeshRenderer>().material = laserMaterial;
		laser.SetActive(false); // Hide the laser.
	}

	/// <summary>
	/// 	Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update () {
		Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
		bool bHit = Physics.Raycast(raycast, out hit);

		// Show the laser if it is colliding with interactable object.
		if(bHit) {
			laser.SetActive(true);
            laser.transform.localScale = new Vector3(thickness, thickness, hit.distance);
        	laser.transform.localPosition = new Vector3(0f, 0f, hit.distance / 2f);
			hitObject = hit.transform.gameObject;
        }
	}

	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable() {
		laser.SetActive(false);
	}
}

}

