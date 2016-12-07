using UnityEngine;
using System.Collections;

public class NavigationCible : MonoBehaviour {

	public float rotateSpeed;
	
	protected Collider currentCollider;
	protected bool mouseDownFlag;
	protected Plane plane;
	protected Vector3 grab;
	protected Vector3 previousPosition;
	protected Vector3 start, end;
	protected Ray ray;

	protected float startP, endP, refP;
	
	// Use this for initialization
	void Start () {
		mouseUp ();
	}
	
	// Update is called once per frame
	void Update () {
		
		plane = new Plane(Vector3.forward, Camera.main.transform.position);

		if (Input.GetMouseButton(0)) { //if left mouse button is down during this frame
			if(!mouseDownFlag) { //if mouse was not held down
				mouseDown();
			} else if(previousPosition != Input.mousePosition) { //if mouse was held down and a movement occured
				mouseDrag();
				previousPosition = Input.mousePosition;
			}
		} else {
			if(mouseDownFlag) { //if left mouse button is up during this frame and was held down on the previous frame
				mouseUp();
			} else if(previousPosition != Input.mousePosition) { //if mouse was held down and a movement occured
				mouseMove();
				previousPosition = Input.mousePosition;
			}
		}

		//Debug.DrawRay (start, ray.direction, Color.green);
		/*Debug.DrawRay (
			Camera.main.transform.position,
			ray.direction, 
			Color.green, 0, false);*/
		Debug.DrawRay (
			start,
			end-start, 
			Color.green, 0, false);

		//print (ray.direction);


		//Debug.DrawRay (new Vector3(0, 0, 0), new Vector3(1, 1, 1), Color.green, 2, false);
		//Vector3 forward = transform.TransformDirection(Vector3.forward) * 100;
		//Debug.DrawRay(transform.position, forward, Color.green, 20, true);
	}
	
	//Called on the first frame the left mouse button is held down
	void mouseDown () {
		mouseDownFlag = true;
		previousPosition = Input.mousePosition;
		
		RaycastHit hit;
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		start = Camera.main.transform.position;
		end = ray.direction;
		
		if (Physics.Raycast (ray, out hit, 10000f)) {
			//currentCollider = hit.collider;
			end = hit.point;

			//top = end.y;
			//bottom = end.y - 50;
			
			float dist;
			plane.Raycast (ray, out dist);
			grab = ray.GetPoint (dist);

			
			endP = Camera.main.WorldToScreenPoint (end).y;
			startP = Camera.main.WorldToScreenPoint (start).y;
			refP = endP;

			
			//plane = new Plane(Vector3.forward , hit.point); //fixed depth
			/*plane = new Plane(Vector3.up , hit.point); //fixed height
			
			float dist;
			plane.Raycast (ray, out dist);
			grab = ray.GetPoint (dist) - currentCollider.transform.position;
			
			textInput.transform.position = Camera.main.WorldToViewportPoint(hit.point);
			textInput.text = currentCollider.name+"\n"+currentCollider.transform.position;*/
		}

	}
	
	//Called every time a movement is noticed while the left mouse button is held down
	void mouseDrag() {

		/*Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		float dist;
		plane.Raycast (ray, out dist);
		float f = ray.GetPoint (dist).y;*/

		float f;
		if (endP < startP) {
			f = startP;
			startP = endP;
			endP = f;
		}

		f = Input.mousePosition.y;
			print (f+" "+startP+" "+endP);
		if (f > endP)
			f = endP;
		if (f < startP)
			f = startP;

		float trf = endP - startP;
		if (trf == 0)
			trf = 1;

		float ff = (refP - f)  / (endP - startP);
		
		Camera.main.transform.position = 
			Vector3.Lerp (start, end, ff);


		
	}
	
	//Called every time the left mouse button is released
	void mouseUp() {
		mouseDownFlag = false;
		start = Camera.main.transform.position - new Vector3(0, 10, 0);
		end = start;
	}

	void mouseMove() {
		float rotateX = Input.GetAxis ("Mouse X") * rotateSpeed;
		float rotateY = Input.GetAxis ("Mouse Y") * rotateSpeed;
		
		if (rotateX != 0) {
			transform.Rotate (rotateX * Vector3.up, Space.World);
		}
		if (rotateY != 0) {
			transform.Rotate (-rotateY * Vector3.right, Space.Self);
		}
	}
	
}
















