using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Serialization;
using UnityEngine;

using MouthFeatures;

namespace SMT.Common{
public class MouthSettings : MonoBehaviour {
	[Serializable]
	class Settings{
		// The height of the line between lips.
		public int lineHeight;
		// Value used for calibration.
		public byte green = 80, blue = 120;
		
		// Value used for training.
		public float normalVOpeness, normalHOpeness,
					smileVOpeness, smileHOpeness,
					maxVOpeness; 
	};
	Settings settings = null;

	// The height of the line between lips.
	public int lineHeight{
		get{ return settings.lineHeight; }
		set{ settings.lineHeight = value; }
	}

	// Value used for calibration.
	public byte green{
		get { return settings.green; }
		set { settings.green = value; }
	}
	public byte blue{
		get { return settings.blue; }
		set { settings.blue = value; }
	}
	
	// Value used for training.
	public float normalVOpeness{
		get { return settings.normalVOpeness; }
		set { settings.normalVOpeness = value; }
	}
	public float normalHOpeness{
		get { return settings.normalHOpeness; }
		set { settings.normalHOpeness = value; }
	}
	public float smileVOpeness{
		get { return settings.smileVOpeness; }
		set { settings.smileVOpeness = value; }
	}
	public float smileHOpeness{
		get { return settings.smileHOpeness; }
		set { settings.smileHOpeness = value; }
	}
	public float maxVOpeness{
		get { return settings.maxVOpeness; }
		set { settings.maxVOpeness = value; }
	}
				 
	void Awake(){ 
		string path = Application.persistentDataPath + "/DefaultMouthSettings.json";
		// If there are previous settings, use it.
		if(File.Exists(path)){
			this.settings = JsonUtility.FromJson<MouthSettings.Settings>(File.ReadAllText(path));
			// Set the color calibration.
			if(Featurer.GetMouthFeatures()	!= null && settings != null){
				Featurer.GetMouthFeatures().SetColorFilter(this.settings.green, this.settings.blue);
				Featurer.GetMouthFeatures().MoveMiddleLine(this.settings.lineHeight);
			}
		}
		// Else create json file.
		else{
			settings = new Settings();
			File.Create(path);
		}
	}

	void OnDestroy(){
		// Save mouth settings.
		string path = Application.persistentDataPath + "/DefaultMouthSettings.json";
		File.WriteAllText(path, JsonUtility.ToJson(this.settings));
	}
}
}