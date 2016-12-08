using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
	
	public Agent agent;
	public GameObject raycaster;
	public GameObject lamp;

	protected bool taken;
	protected float maxDistance;
	protected float fov;
	protected float takeSize;

	// Use this for initialization
	void Start () {
		maxDistance = 2.5f;
		takeSize = 1f;
		fov = 1f;
		reset ();
	}

	public void reset() {
		taken = false;
		lamp.GetComponent<Light>().enabled = false;
		gameObject.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {

		//Custom condition : your device must implement this predicate. See class "Agent.cs"
		if (agent.isTryingToGetKey()) {

			//Global conditions : no matter what device you use, these conditions have to be met to succesfully complete your action.
			if (taken == false) {
				
				//First way to get a key : being close enough to it. (no y axis)
				if(Vector2.Distance (
					new Vector2(
					gameObject.transform.position.x,
					gameObject.transform.position.z),
					new Vector2(
					agent.transform.position.x,
					agent.transform.position.z)
					) <= takeSize) {
					take();
				}

				//Second way to get a key : looking directly at it from a moderate distance.
				//RaycastHit hit;
				Ray ray = new Ray(raycaster.transform.position, raycaster.transform.forward);
				Debug.DrawRay(raycaster.transform.position, raycaster.transform.forward*maxDistance, Color.green, 0, true);

				foreach(RaycastHit hit in Physics.SphereCastAll(ray, fov, maxDistance)) {
					if (hit.collider.gameObject.Equals(gameObject)) {
						take();
					}
				}



				/*if (Physics.SphereCast(ray, fov, out hit, maxDistance)) {
					if (hit.collider.gameObject.Equals(gameObject)) {
						take();
					}
				}*/
			}
		}
	}

	void OnCollisionEnter(Collision col) {
		if(col.gameObject == raycaster) {
			take();
		}
	}

	protected void take() {
		print ("Key taken");
		taken = true;
		lamp.GetComponent<Light>().enabled = true;
		gameObject.SetActive (false);
	}

	public bool isTaken() {
		return taken;
	}
}
