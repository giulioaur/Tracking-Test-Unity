using System;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace SMT.Common{
[RequireComponent(typeof(SushiNetworkManager))]
public class NetManager : MonoBehaviour {
	[Serializable]
	public struct IpAddresses{
		public string localAddress, remoteAddress;
		public int localPort, remotePort;
	}

	// The info about the ip addresses of server and client.
	IpAddresses _ipinfo;
	// Network manager.
	NetworkManager manager;

	/// <summary>
	/// 	Returns the info about ip addresses of applications.
	/// </summary>
	/// <value> The stuct with all addresses. </value>
	public IpAddresses ipInfo{
		get{ 
			IpAddresses ipInfo;
			ipInfo.localAddress = _ipinfo.localAddress; ipInfo.localPort = _ipinfo.localPort;	
			ipInfo.remoteAddress = _ipinfo.remoteAddress; ipInfo.remotePort = _ipinfo.remotePort;	
			return ipInfo;
		}
	}

	// Use this for initialization
	void Awake(){
		manager = GetComponent<NetworkManager>();
		_ipinfo = JsonUtility.FromJson<IpAddresses>(File.ReadAllText(Application.dataPath + "/ipconfig.json"));
	}

	/// <summary>
	/// 	Change the address to which connect.
	/// </summary>
	/// <param name="value"> The new address. </param>
	public void ChangeAddress(InputField field){
		IPAddress address;
    	if (IPAddress.TryParse(field.text, out address))
			manager.networkAddress = field.text;
	}	

	/// <summary>
	/// 	Change the port to which connect.
	/// </summary>
	/// <param name="value"> The new port. </param>
	public void ChangePort(InputField field){
		int port = 0;

		if (Int32.TryParse(field.text, out port))
			manager.networkPort = port;
	}
	
	/// <summary>
	/// 	Start the game as host.
	/// </summary>
	public void StartHost(){
		manager.StartHost();
	} 

	/// <summary>
	/// 	Start the game as client.
	/// </summary>
	public void StartClient(){
		manager.networkAddress = _ipinfo.remoteAddress;
		manager.StartClient();
	}
}
}