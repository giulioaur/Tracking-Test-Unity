using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MouthFeatures;

namespace SMT.Common{
public class Mouth : MonoBehaviour {

	static Texture2D blackTexture;
	// The DLL wrapper.
	Featurer featurer;
	// The texture to update.
	Texture2D image, normalImage, lipImage;
	byte components;
	// The feature points and images.
	public ushort[] points = new ushort[12];

	public enum ImageType{
		STANDARD, NORMALIZED, LIP
	}

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
		if ((featurer = Featurer.GetMouthFeatures()) == null)
			gameObject.SetActive(false);
		else{
			points = new ushort[12];
			blackTexture = new Texture2D(featurer.width, featurer.height, TextureFormat.RGB24, false);
			image = new Texture2D(featurer.width, featurer.height, TextureFormat.RGB24, false);
			normalImage = new Texture2D(featurer.width, featurer.height, TextureFormat.RGB24, false);
			lipImage = new Texture2D(featurer.width, featurer.height, TextureFormat.RGB24, false);
			components = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Get feature points.
		int status;
		ushort[] newPoints = featurer.GetFeaturePoints(out status);

		if (status == (int)Errors.OK){
			// Update points
			points = newPoints;

			// If the frame has been updates and images are needed, get them.
			if (components > 0){
				Featurer.ImageSet set = featurer.GetImages();
				
				// Apply the image to all the textures.
				image.LoadRawTextureData(set.image);
				image.Apply();

				normalImage.LoadRawTextureData(set.normImg);
				normalImage.Apply();

				lipImage.LoadRawTextureData(set.lipImg);
				lipImage.Apply();
			}
		}

		
	}

	/// <summary>
	/// 	Set a mouth texture for a material.
	/// 	For a clean removal of the texture use RemoveTexture() method.
	/// </summary>
	/// <param name="type"> The type of image to print on the texture. </param>
	/// <param name="material"> The material on which set the texture. </param>
	/// <returns> The new mouth texture. </returns>
	public void SetMouthTexture(ImageType type, Material material){
		Texture2D texture;

		// Choose the type of texture.
		switch(type){
			case ImageType.STANDARD:
				texture = image; break;
			case ImageType.NORMALIZED:
				texture = normalImage; break;
			case ImageType.LIP:	
				texture = lipImage;	break;
			default:
				texture = null; break;
		}

		// Apply it.
		material.mainTextureScale = new Vector2(-1, -1);
		material.SetTexture("_MainTex", texture);
		++components;
	}

	/// <summary>
	/// 	Remove the mouth texture and apply a black one.
	/// </summary>
	/// <param name="material"> The material on which remove the texture. </param>
	public void RemoveTexture(Material material){
		// Apply black texture.
		material.mainTextureScale = new Vector2(1, 1);
		material.SetTexture("_MainTex", blackTexture);
		--components;
	}

	void OnDestroy(){
		if(featurer != null)
			featurer.Stop();
	}
}
}