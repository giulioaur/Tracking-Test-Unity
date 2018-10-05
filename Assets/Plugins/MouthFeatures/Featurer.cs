using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MouthFeatures {
public class Featurer {
	/// <summary>
	/// 	The struct to store the images of the lip tracking.
	/// </summary>
	public struct ImageSet{
		public byte[] image, normImg, lipImg;

		public ImageSet(int width, int height){
			image = new byte[width * height * 3];
			normImg = new byte[width * height * 3];
			lipImg = new byte[width * height * 3];
		}
	};
	/// <summary>
	/// 	The struct to store all the internet related stuffs.
	/// </summary>
	[Serializable]
	struct IpAddresses{
		public string localAddress, remoteAddress;
		public int localPort, remotePort;
	}

	static Featurer instance = null;
	static bool _lipFeed = false;
	int _width, _height;
	ImageSet images;

	// True if there is lip feedback from local, false otherwise.
	public static bool lipFeed{ get{ return _lipFeed; } }
	// Returns image information.
	public int width{ get{ return _width;} }
	public int height{ get{ return _height;} }

	/// <summary>
	/// 	Private constructor.
	/// </summary>
	/// <param name="height"> The height of the images. </param>
	/// <param name="width"> The width of the images. </param>
	private Featurer(int height, int width){
		_width = width;
		_height = height;
		images = new ImageSet(width, height);
	}

	/// <summary>
	/// 	Returns the singleton of the featurer object.
	/// 	It also init the dll on the first call.
	/// </summary>
	/// <returns> The singleton of the featurer class. </returns>
	public static Featurer GetMouthFeatures(){
		// Create script files.
		string folder = Application.dataPath + "/CameraScripts/";
		
		if(!File.Exists(folder + "openCam.txt")) {
			Directory.CreateDirectory(folder);
			File.WriteAllText(folder + "openCam.txt", "raspivid -w " + 800 + " -h " + 600 + " -b 2600000 -fps 30 -o video.h264 -t 0 -pf main -v");
		}

		if(!File.Exists(folder + "closeCam.txt")) 
			File.WriteAllText(folder + "closeCam.txt", "killall -9 raspivid");

		// Try initialize tracking component.
		if(instance == null){
			IpAddresses addresses = JsonUtility.FromJson<IpAddresses>(File.ReadAllText(Application.dataPath + "/ipconfig.json"));
			int status = API.init(addresses.localAddress, addresses.localPort, addresses.remoteAddress, addresses.remotePort, folder);

			if(status == (int)Errors.OK || status == (int)Errors.NO_RASP){
				instance = new Featurer(580, 660);
				_lipFeed = status != (int)Errors.NO_RASP;
			}
		}
			
		return instance;
	}

	/// <summary>
	/// 	Returns the lip's feature points.
	/// </summary>
	/// <param name="status"> The returned status. </param>
	/// <returns> The retrieved feature points from the lip image. </returns>
	public unsafe ushort[] GetFeaturePoints(out int status){            
		ushort[] data = new ushort[12];
		
		IntPtr dataPtr;

		// Retrive the feature points array.
		fixed (ushort* bdec = data) {
			dataPtr = new IntPtr((void*)bdec);

			status = API.getFeatures(dataPtr);
		}

		return data;
	}

	/// <summary>
	/// 	Sets the new color upper bounds for filtering image.
	/// </summary>
	/// <param name="green"> The green upper bound. </param>
	/// <param name="blue"> The blue upper bound. </param>
	public void SetColorFilter(int green, int blue){
		API.setColorFilter(green, blue);
	}

	/// <summary>
	/// 	Moves the line between the lip to the desired height.
	/// </summary>
	/// <param name="height"> The new height of the middle line. </param>
	public void MoveMiddleLine(int height){
		API.setMiddleLine(height);
	}

	/// <summary>
	/// 	Starts the calibration phase.
	/// </summary>
	public void StartCalibration(){
		API.startCalibration();
	}

	/// <summary>
	/// 	Ends the calibration phase.
	/// </summary>
	public void StopCalibration(){
		API.stopCalibration();
	}

	/// <summary>
	/// 	Gets the processed image from the video get by raspberry.
	/// </summary>
	/// <returns></returns>
	public unsafe ImageSet GetImages(){		
		IntPtr imgPtr, normPtr, lipPtr;

		// Retrive the feature points array.
		fixed (byte* img = images.image) {
		fixed (byte* nImg = images.normImg){
		fixed (byte* lImg = images.lipImg){
			imgPtr = new IntPtr((void*)img);
			normPtr = new IntPtr((void*)nImg);
			lipPtr = new IntPtr((void*)lImg);

			API.getImages(imgPtr, normPtr, lipPtr);
		}
		}
		}

		return images;
	}

	/// <summary>
	/// 	Returns the video from the camera in the server.
	/// </summary>
	/// <returns> The current frame received by other application. </returns>
	public unsafe byte[] GetVideo(out int status){
		byte []frame = new byte[_width * _height * 3];

		fixed (byte *frm = frame){
			IntPtr framePtr = new IntPtr((void*)frm);

			status = API.getVideo(framePtr);
		}

		return frame;
	}

	/// <summary>
	/// 	Sets the flag for video feedback sending. 
	/// </summary>
	/// <param name="shouldSend"> True if the video should be sent, false otherwise. </param>
	public void SendVideo(bool shouldSend){
		API.setSendFlag(shouldSend);
	}

	/// <summary>
	/// 	Stops all the dll components.
	/// </summary>
	public void Stop(){
		if(instance != null){
			int status = API.end();

			if((Errors)status != Errors.OK)
				throw new Exception(((Errors)status).ToString());

			instance = null;
		}
		
	}


}
}