using UnityEngine;
using System.Collections;

/*
 * This class is an API for the actions of the soldier.
 * It must be inherited by a custom class for defining behaviors according to your device's inputs.
 */
abstract public class Agent : MonoBehaviour {

	public GameObject head;
	private CharacterController controller;
	protected Quaternion baseRotation;
	protected Vector3 basePosition;
	protected GameObject startLineTrigger;
	protected bool gameOver;
	protected int waiting;

	/*
	 * A predicate for checking if the user is currently using the device in such a way that a key card should be taken.
	 * Exemple : The press of the "A" button on a controller.
	 */
	public abstract bool isTryingToGetKey ();

	/*
	 * A predicate for checking if the user is currently using the device in such a way that the door should be opened.
	 * Exemple : The press of the "B" button on a controller.
	 */
	public abstract bool isTryingToOpenDoor ();
	
	/*
	 * Must return the RELATIVE movement the soldier should ideally make during this frame according to the device. No need to check for collisions.
	 * Exemple : Vector3(1, 0, 0) if the forward arrow is pushed on a controller.
	 */
	public abstract Vector3 getCurrentMovement ();
	
	/*
	 * Must return the RELATIVE head rotation the soldier should make during this frame according to the device.
	 * Exemple : The angle difference between two frames for an Oculus Rift.
	 */
	public abstract Vector3 getCurrentRotation ();


	public abstract void afterStart ();

	/*
	 * Is called each frame before trying to update the soldier.
	 * This method should be used for INTERNAL changes. Is is not intended to make this soldier move or rotate.
	 */
	public abstract void beforeUpdate ();
	
	/*
	 * Is called each frame after trying to update the soldier.
	 * This method should be used for INTERNAL changes. Is is not intended to make this soldier move or rotate.
	 */
	public abstract void afterUpdate ();


	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		afterStart ();
	}

	// Update is called once per frame
	void Update () {
		if (isWaiting()) {
			return;
		}

		if (gameOver)
			return;

		beforeUpdate ();

		Vector3 moveDirection = getCurrentMovement ();
		if (controller.isGrounded)
			moveDirection.y -= 1 * Time.deltaTime;
		else
			moveDirection.y = -1;

		Vector3 q = getCurrentRotation ();
		
		transform.Rotate(new Vector3(0f, q.y, 0f));

		head.transform.Rotate(new Vector3(q.x, 0f, q.z));

		//head.transform.Rotate(q);
		controller.Move(moveDirection);

		


		afterUpdate ();
	}

	/*
	 * Returns a checkpoint object if this actor passing said checkpoint is yet to be acknowledged. Null otherwise.
	 */
	public GameObject acknowledgeTrigger () {
		if (startLineTrigger != null) {
			GameObject trigger = startLineTrigger;
			startLineTrigger = null;
			return trigger;
		}
		return startLineTrigger;
	}

	public void stop() {
		this.gameOver = true;
	}

	public bool isGameOver() {
		return gameOver;
	}

	public bool isWaiting() {
		return waiting > 0;
	}

	public int getWaitingTime() {
		return waiting;
	}

	public bool waitOnce() {
		if (waiting > 0) {
			--waiting;
			return false;
		}
		return true;
	}

	public void reset(int waitingTime) {
		if (basePosition.Equals(new Vector3(0, 0, 0))) {
			print (transform.position);
			baseRotation = transform.rotation;
			basePosition = transform.position;
		} else {
			transform.rotation = baseRotation;
			transform.position = basePosition;
		}
		this.waiting = waitingTime;
		this.gameOver = false;
	}

	void OnTriggerEnter(Collider col) {
		startLineTrigger = col.gameObject;
	}



}
