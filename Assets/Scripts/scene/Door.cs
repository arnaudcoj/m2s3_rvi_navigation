using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	protected bool isOpen;
	public Key red;
	public Key green;
	public Agent agent;
	public GameObject raycaster;
	protected float maxDistance;
	protected bool objectiveComplete;

	// Use this for initialization
	void Start () {
		isOpen = false;
		maxDistance = 4.0f;
	}

	public bool isObjectiveComplete() {
		return objectiveComplete;
	}

	public bool reset() {
		objectiveComplete = false;
		close();
		red.reset ();
		green.reset ();
		return true;
		/*if (objectiveComplete) {
		} else {
			print ("Wrong way or you're cheating !");
			return false;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		//print ("door");
		if (!isOpen && agent.isTryingToOpenDoor ()) {
			//print ("trying");
			if(red.isTaken() && green.isTaken()) {
				//print ("key ok");
				RaycastHit hit;
				Ray ray = new Ray(raycaster.transform.position, raycaster.transform.forward);
				Debug.DrawRay(raycaster.transform.position, raycaster.transform.forward*maxDistance, Color.blue, 0, true);
				if (Physics.Raycast(ray, out hit, maxDistance)) {
					if (hit.collider.gameObject.transform.parent.gameObject.Equals(gameObject)) {
						open();
						objectiveComplete = true;
					}
				}
			}
		}
	}

	void open() {
		if (!isOpen) {
			GetComponent<Animation>().Play ("doorOpen");

			if (GetComponent<Animation>() ["doorOpen"].normalizedTime == 0.0) {
				GetComponent<Animation>() ["doorOpen"].time = 0;
			}
			
			GetComponent<Animation>() ["doorOpen"].speed = 1.0f;
			isOpen = true;
		}
	}

	void close() {
		if (isOpen) {
			GetComponent<Animation>().Play ("doorOpen");
		
			if (GetComponent<Animation>() ["doorOpen"].normalizedTime == 0.0) {
				GetComponent<Animation>() ["doorOpen"].time = GetComponent<Animation>() ["doorOpen"].length;
			}
		
			GetComponent<Animation>() ["doorOpen"].speed = -1.0f;
			this.isOpen = false;
		}
	}

}
