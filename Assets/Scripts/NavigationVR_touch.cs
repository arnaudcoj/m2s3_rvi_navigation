using UnityEngine;
using System.Collections;

public class NavigationVR_touch : Agent
{
	public GameObject pointer;
	// bookkeep x rotation because Unity uses quaternions and behavior is not adapted to the algorithm
	// no gimbal lock here because body and torso rotations are separated
	Vector3 previousEulerAngles;
	Vector3 eulerAngles;

	float rayCastMinLimit = 2f;
	float rayCastMaxLimit = 30f;

	float pointerRange;
	float pointerMinRange;
	float pointerMaxRange;

	Ray targetRay;

	int firstFingerID = 0;
	bool secondFingerMode;
	Vector2 secondFingerInitialPosition;

	Vector3 velocity;

	// Use this for initialization

	public override void afterStart () {
		resetPointerRange ();
		previousEulerAngles = transform.rotation.eulerAngles;
		eulerAngles = transform.rotation.eulerAngles;
		pointer.SetActive (false);
		secondFingerMode = false;
	}

	void startDash ()
	{
		if (pointerRange >= pointerMinRange) {
			Vector3 target = targetRay.GetPoint (pointerRange);
			velocity = transform.InverseTransformDirection(target - transform.position);
			velocity.y = 0;
		}
	}

	void updateRaycast ()
	{
		targetRay = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
		RaycastHit hit;
		if (Physics.Raycast (targetRay, out hit)) {
			pointerMaxRange = Mathf.Min (hit.distance, rayCastMaxLimit);
		} else {
			pointerMaxRange = rayCastMaxLimit;
		}
	}

	void movePointer ()
	{
		Vector3 point = targetRay.GetPoint (Mathf.Clamp (pointerRange, pointerMinRange, pointerMaxRange));
		pointer.transform.position = point;
	}

	void RotateTorso ()
	{
		Vector2 firstTouchOnScreen = Input.GetTouch (0).position;
		float seuil = 0.5f;
		Vector2 screen_center = new Vector2 (Screen.width, Screen.height) / 2f;

		//left-right			
		float lr_rotation = (firstTouchOnScreen.x - screen_center.x) / screen_center.x;
		lr_rotation = Mathf.Sign (lr_rotation) * ((Mathf.Max (seuil, Mathf.Abs (lr_rotation))) - seuil) * (1f / (1f - seuil));
		lr_rotation = Mathf.Sign (lr_rotation) * Mathf.Pow (lr_rotation, 2f);
		eulerAngles.y = eulerAngles.y + lr_rotation;
	}

	void initSecondFingerMode ()
	{
		secondFingerInitialPosition = Input.GetTouch ((1 + firstFingerID) % 2).position;
	}

	void updatePointerRange ()
	{
		float d = Input.GetTouch ((1 + firstFingerID) % 2).position.y / secondFingerInitialPosition.y;
		//Debug.Log (d);
		pointerRange = Mathf.Min (d * pointerMaxRange, pointerMaxRange);
		if (pointerRange < pointerMinRange) {
			if (pointer.activeSelf)
				pointer.SetActive (false);
		} else {
			if (!pointer.activeSelf)
				pointer.SetActive (true);
		}
	}

	void resetPointerRange ()
	{
		pointerRange = rayCastMaxLimit;
		pointerMaxRange = rayCastMaxLimit;
		pointerMinRange = rayCastMinLimit;
	}

	public static float computeAngle (Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2 (
			Vector3.Dot (n, Vector3.Cross (v1, v2)),
			Vector3.Dot (v1, v2)) * Mathf.Rad2Deg;
	}





	public override bool isTryingToGetKey ()
	{
		return true; //always trying to get keys
	}

	public override bool isTryingToOpenDoor ()
	{
		return true; //always trying to open the door
	}

	public override void beforeUpdate ()
	{
		switch (Input.touchCount) {
		case 0:
			if (secondFingerMode) {
				secondFingerMode = false;
				startDash ();
				resetPointerRange ();
			}

			if (pointer.activeSelf) {
				pointer.SetActive (false);
				resetPointerRange ();
			}
			break;
		case 1:
			if (secondFingerMode) {
				secondFingerMode = false;
				if (firstFingerID != Input.GetTouch (0).fingerId) {
					firstFingerID = Input.GetTouch (0).fingerId;
				} else {
					startDash ();
				}
				resetPointerRange ();
			}

			if (!pointer.activeSelf) {
				pointer.SetActive (true);
			}
			updateRaycast ();
			movePointer ();
			RotateTorso ();
			break;
		case 2:
			if (!secondFingerMode) {
				secondFingerMode = true;
				initSecondFingerMode ();
			}
			updatePointerRange ();
			movePointer ();
			break;
		default:
			break;
		}
	}

	public override void afterUpdate ()
	{
		previousEulerAngles = eulerAngles;
		velocity = Vector3.zero;
	}

	public override Vector3 getCurrentMovement ()
	{
		return transform.TransformDirection (velocity);
	}

	public override Vector3 getCurrentRotation ()
	{
		return eulerAngles - previousEulerAngles ;
	}
}
