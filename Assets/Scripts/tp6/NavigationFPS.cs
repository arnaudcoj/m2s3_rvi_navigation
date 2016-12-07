using UnityEngine;
using System.Collections;

public class NavigationFPS : MonoBehaviour {
	
	public float rotateSpeed;
	public float frontSpeed;
	public float backSpeed;
	public float sideSpeed;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			
			
			float rotateX = Input.GetAxis ("Mouse X") * rotateSpeed;
			float rotateY = Input.GetAxis ("Mouse Y") * rotateSpeed;

			if (rotateX != 0) {
				transform.Rotate (rotateX * Vector3.up, Space.World);
			}
			if (rotateY != 0) {
				transform.Rotate (-rotateY * Vector3.right, Space.Self);
			}

		}
		
		
		/**/ if (Input.GetKey("up"   )) {moveFront();}
		else if (Input.GetKey("down" )) {moveBack ();}
		
		/**/ if (Input.GetKey("left" )) {stepLeft ();}
		else if (Input.GetKey("right")) {stepRight();}

	}
	
	void moveFront() {
		transform.Translate (Vector3.forward*frontSpeed);
	}
	
	void moveBack() {
		transform.Translate (Vector3.back * backSpeed);
	}
	
	void stepLeft() {
		transform.Translate (Vector3.left * sideSpeed);
	}
	
	void stepRight() {
		transform.Translate (Vector3.right * sideSpeed);
	}

}
