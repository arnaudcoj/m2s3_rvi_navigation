using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour {
	
	public float spinAngle = 2.0f;
	public float frontSpeed = 0.10f;
	public float backSpeed = 0.04f;
	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;
	
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		bool input = false;
		
		/**/ if (Input.GetKey("up"   )) {moveFront(); input=true;}
		else if (Input.GetKey("down" )) {moveBack (); input=true;}
		
		/**/ if (Input.GetKey("left" )) {turnLeft (input); input=true;}
		else if (Input.GetKey("right")) {turnRight(input); input=true;}
		
		if(!input)
			idle();  

		if (controller.isGrounded)
			moveDirection.y -= 1 * Time.deltaTime;
		else
			moveDirection.y = -1;
		
		controller.Move(moveDirection);

	}
	
	void turnLeft(bool keepAnimation) {
		/*if (!keepAnimation) {
			animation.CrossFade ("soldierSpinLeft");
		}*/
		transform.Rotate (new Vector3 (0, spinAngle, 0));
	}
	
	void turnRight(bool keepAnimation) {
		/*if(!keepAnimation) {
			animation.CrossFade("soldierSpinRight");
		}*/
		transform.Rotate (new Vector3 (0, -spinAngle, 0));
	}
	
	void moveFront() {
		//animation.CrossFade("soldierRun");
		//transform.Translate (Vector3.forward*frontSpeed);
		
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= frontSpeed;

	}
	
	void moveBack() {
		//animation.CrossFade("soldierWalk");
		//animation ["soldierWalk"].speed = -1.0f;
		//transform.Translate (Vector3.back * backSpeed);

		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= backSpeed;

	}
	
	void idle() {
		//animation.CrossFade("soldierIdle");
	}
}
