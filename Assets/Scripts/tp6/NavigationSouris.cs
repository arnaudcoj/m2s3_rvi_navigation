using UnityEngine;
using System.Collections;

public class NavigationSouris : MonoBehaviour {

	protected bool mouseDownFlag;
	protected Vector3 previousPosition;
	
	protected float refX, refY;
	protected Vector2 refMouse;
	protected float rotateX, moveY;
	public float XSensibility = 0.02f;
	public float YSensibility = 0.05f;
	public float inertia = 0.05f;

	// Use this for initialization
	void Start () {
		//plane = new Plane(Vector3.up , 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0)) { //if left mouse button is down during this frame
			if(!mouseDownFlag) { //if mouse was not held down
				mouseDown();
			} else if(previousPosition != Input.mousePosition) { //if mouse was held down and a movement occured
				mouseDrag();
				previousPosition = Input.mousePosition;
			}
			applyMovement();
		} else if(mouseDownFlag) { //if left mouse button is up during this frame and was held down on the previous frame
			mouseUp();
		}


	}

	void applyMovement() {
		//interpolate reference point everytime for inertia
		refMouse = Vector2.Lerp (refMouse, Input.mousePosition, inertia);

		//new rotation and movement calculation
		rotateX = (Input.mousePosition.x - refMouse.x) * XSensibility;
		moveY = (Input.mousePosition.y - refMouse.y) * YSensibility;

		//apply new rotation and movement
		transform.Rotate(rotateX * Vector3.up);
		transform.Translate(-moveY * Vector3.back);

	}
	
	//Called on the first frame the left mouse button is held down
	void mouseDown () {
		mouseDownFlag = true;
		previousPosition = Input.mousePosition;
		refMouse = new Vector2 (previousPosition.x, previousPosition.y);
	}
	
	//Called every time a movement is noticed while the left mouse button is held down
	void mouseDrag() {

	}
	
	//Called every time the left mouse button is released
	void mouseUp() {
		mouseDownFlag = false;
		// immediate stop on release
		rotateX = 0;
		moveY = 0;

	}





















}
