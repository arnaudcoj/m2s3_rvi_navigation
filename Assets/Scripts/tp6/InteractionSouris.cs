using UnityEngine;
using System.Collections;

public class InteractionSouris : MonoBehaviour {
	
	public GUIText textInput;
	
	protected Collider currentCollider;
	protected bool mouseDownFlag;
	protected Plane plane;
	protected Vector3 grab;
	protected Vector3 previousPosition;
	
	// Use this for initialization
	void Start () {

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
		} else if(mouseDownFlag) { //if left mouse button is up during this frame and was held down on the previous frame
			mouseUp();
		}
	}
	
	//Called on the first frame the left mouse button is held down
	void mouseDown () {
		mouseDownFlag = true;
		previousPosition = Input.mousePosition;
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		if (Physics.Raycast (ray, out hit)) {
			currentCollider = hit.collider;
			
			//plane = new Plane(Vector3.forward , hit.point); //fixed depth
			plane = new Plane(Vector3.up , hit.point); //fixed height
			
			float dist;
			plane.Raycast (ray, out dist);
			grab = ray.GetPoint (dist) - currentCollider.transform.position;

			textInput.transform.position = Camera.main.WorldToViewportPoint(hit.point);
			textInput.text = currentCollider.name+"\n"+currentCollider.transform.position;
		} else {
			textInput.text = "";
		}
	}
	
	//Called every time a movement is noticed while the left mouse button is held down
	void mouseDrag() {
		if (currentCollider != null) {
			float dist;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (plane.Raycast (ray, out dist)) {
				Vector3 point = ray.GetPoint (dist);
				currentCollider.transform.position = point - grab;
				textInput.transform.position = Camera.main.WorldToViewportPoint (point);
				textInput.text = currentCollider.name + "\n" + currentCollider.transform.position;
			}

		}
	}
	
	//Called every time the left mouse button is released
	void mouseUp() {
		textInput.text = "";
		mouseDownFlag = false;
		currentCollider = null;
	}
}
















