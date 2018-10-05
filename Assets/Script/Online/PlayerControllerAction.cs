using UnityEngine;
using UnityEngine.Networking;

namespace SMT.Online{

public class PlayerControllerAction : NetworkBehaviour {
	// Two controllers.
	public ControllerAction right, left;
	// Commands.
	public enum commands{grab, release, showLaser, hideLaser, grabCompanion};

	/// <summary>
	/// 	Recalls the command for a specific action.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	public void CallCommand(commands command, bool isRight, Vector3 velocity, Vector3 angularVelocity) {
		// Calls the command for the server and both command and local method for clients.
		switch (command){
			case commands.grab:
				CmdGrab(isRight); 
				if(!isServer)	(isRight ? right : left).Grab();
				break;
			case commands.release:
				CmdRelease(isRight, velocity, angularVelocity); 
				if(!isServer)	(isRight ? right : left).Release(velocity, angularVelocity);
				break;
			case commands.showLaser:
				CmdShowLaser(isRight);
				if(!isServer)	(isRight ? right : left).ShowLaser();
				break;
			case commands.hideLaser:
				CmdHideLaser(isRight);
				if(!isServer)	(isRight ? right : left).HideLaser();
				break;
			case commands.grabCompanion:
				CmdGrabCompanion(isRight);
				if(!isServer)	(isRight ? right : left).GrabCompanion();
				break;
		}
		
	}

	/// <summary>
	/// 	Recalls the grab action on the server copy of the controller.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	[Command]
	void CmdGrab(bool isRight) {
		(isRight ? right : left).Grab();
	}

	/// <summary>
	/// 	Recalls the release action on the server copy of the controller.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	[Command]
	void CmdRelease(bool isRight, Vector3 velocity, Vector3 angularVelocity) {
		(isRight ? right : left).Release(velocity, angularVelocity);
	}

	/// <summary>
	/// 	Recalls the action to show the controller laser on the server copy of the controller.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	[Command]
	void CmdShowLaser(bool isRight) {
		(isRight ? right : left).ShowLaser();
	}

	/// <summary>
	/// 	Recalls the action to hide the controller laser on the server copy of the controller.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	[Command]
	void CmdHideLaser(bool isRight) {
		(isRight ? right : left).HideLaser();
	}

	/// <summary>
	/// 	Recalls the action to hide the controller laser on the server copy of the controller.
	/// </summary>
	/// <param name="isRight"> true if the method is invoked by right controller, false otherwise. </param>
	[Command]
	void CmdGrabCompanion(bool isRight) {
		(isRight ? right : left).GrabCompanion();
	}
}

}
