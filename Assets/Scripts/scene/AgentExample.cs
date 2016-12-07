using UnityEngine;
using System.Collections;

public class AgentExample : Agent {

	public override bool isTryingToGetKey() {
		return true; //always trying to get keys
	}
	
	public override bool isTryingToOpenDoor() {
		return true; //always trying to open the door
	}
	
	public override Vector3 getCurrentMovement() {
		return transform.TransformDirection( new Vector3(0, 0, 0.2f)); //always moving forward by 0.2 units
	}
	
	public override Vector3 getCurrentRotation() {
		return new Vector3(0, 0.5f, 0); //always rotation 0.5 units
	}
	
	public override void beforeUpdate () {
		// nothing to prepare;
	}
	
	public override void afterUpdate () {
		// nothing to do after update;
	}

	public override void afterStart () {

	}
}
