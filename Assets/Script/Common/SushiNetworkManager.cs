using UnityEngine;
using UnityEngine.Networking;

using MouthFeatures;

namespace SMT.Common{
public class SushiNetworkManager : NetworkManager {

	public override void OnServerConnect(NetworkConnection connection)
    {
        if(Featurer.GetMouthFeatures() != null && numPlayers > 0){
            print("OK");
        	Featurer.GetMouthFeatures().SendVideo(true);
        }
    }

    //Detect when a client connects to the Server
    public override void OnServerDisconnect(NetworkConnection connection)
    {
        if(Featurer.GetMouthFeatures() != null)
			Featurer.GetMouthFeatures().SendVideo(false);
    }
}
}