using UnityEngine;
using System.Collections;

public class NavigationVR_touch : Agent
{
	public Pointer pointer;
	// bookkeep x rotation because Unity uses quaternions and behavior is not adapted to the algorithm
	// no gimbal lock here because body and torso rotations are separated
	Vector3 previousEulerAngles;
	Vector3 eulerAngles;

	float rayCastMinLimit = 2f;
	float rayCastMaxLimit = 15f;

	float pointerRange;
	float pointerMinRange;
	float pointerMaxRange;

	Ray targetRay;

	int firstFingerID = 0;
	bool secondFingerMode;
	Vector2 secondFingerInitialPosition;


	bool dash = false;
	float dashTime = 0.2f; //in seconds
	float timeElapsed = 0f;
	float dashCoeff = 0f;
	Vector3 dashStart;
	Vector3 dashTarget;
	Vector3 velocity;

	// Use this for initialization

	public override void afterStart () {
		resetPointerRange ();
		previousEulerAngles = transform.rotation.eulerAngles;
		eulerAngles = transform.rotation.eulerAngles;
		pointer.gameObject.SetActive (false);
		secondFingerMode = false;
	}


	public override void beforeUpdate ()
	{
		if (dash) {
			updateDash ();
		} else {
			switch (Input.touchCount) {
			case 0:
				if (secondFingerMode) {
					secondFingerMode = false;
					startDash ();
					resetPointerRange ();
				}

				if (pointer.gameObject.activeSelf) {
					pointer.gameObject.SetActive (false);
					resetPointerRange ();
					eulerAngles.z = 0;
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
				} else {
					if (!pointer.gameObject.activeSelf) {
						pointer.gameObject.SetActive (true);
					}
					updateRaycast ();
					movePointer ();
					RotateTorso ();
					RotateHead ();
				}
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
	}


	void startDash ()
	{
		if (pointerRange >= pointerMinRange) {
			dash = true;
			timeElapsed = 0f;
			dashCoeff = 0f;
			dashStart = transform.position;
			dashTarget = targetRay.GetPoint (pointerRange);
			updateDash ();
		}
	}

	void updateDash() {
		if (dashCoeff < 1f) {
			timeElapsed += Time.deltaTime;
			dashCoeff = Mathf.Min(timeElapsed / dashTime, 1f);
			dashCoeff = Mathf.Pow (Mathf.Sin(Mathf.PI * dashCoeff / 2f), 2f);
			Vector3 interPosition = dashTarget * dashCoeff + dashStart * (1f - dashCoeff);
			velocity = interPosition - transform.position;
			velocity.y = 0;
		} else {
			dash = false;
			if (Input.touchCount > 0) {
				updateRaycast ();
			}
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
		pointer.setPosition(point);
	}

	void RotateTorso ()
	{
		Vector2 firstTouchOnScreen = Input.GetTouch (0).position;
		float seuil = 0.4f;
		Vector2 screen_center = new Vector2 (Screen.width, Screen.height) / 2f;

		//left-right			
		float lr_rotation = (firstTouchOnScreen.x - screen_center.x) / screen_center.x;
		lr_rotation = Mathf.Sign (lr_rotation) * ((Mathf.Max (seuil, Mathf.Abs (lr_rotation))) - seuil) * (0.85f / (0.85f - seuil));
		lr_rotation = Mathf.Clamp (lr_rotation, -1f, 1f);
		//lr_rotation = Mathf.Sign(lr_rotation) * Mathf.Pow (lr_rotation, 2f);
		eulerAngles.y = eulerAngles.y + lr_rotation * 4f;
	}

	void RotateHead ()
	{
		Vector2 firstTouchOnScreen = Input.GetTouch (0).position;
		float seuil = 0.4f;
		Vector2 screen_center = new Vector2 (Screen.width, Screen.height) / 2f;

		//up-down

		float ud_rotation = (firstTouchOnScreen.y - screen_center.y) / screen_center.y;
		ud_rotation = Mathf.Sign (ud_rotation) * ((Mathf.Max (seuil, Mathf.Abs (ud_rotation))) - seuil) * (1f / (1f - seuil));
		//ud_rotation = Mathf.Sign (ud_rotation) * Mathf.Pow (ud_rotation, 2f);
		//eulerAngles.z = Mathf.Clamp(eulerAngles.z + ud_rotation, -30, 30);
		eulerAngles.z = 40 * ud_rotation;

	}

	void initSecondFingerMode ()
	{
		secondFingerInitialPosition = Input.GetTouch ((1 + firstFingerID) % 2).position;
	}

	void updatePointerRange ()
	{
		float d = Input.GetTouch ((1 + firstFingerID) % 2).position.y / secondFingerInitialPosition.y;
		pointerRange = Mathf.Min (d * pointerMaxRange, pointerMaxRange);
		if (pointerRange < pointerMinRange) {
			if (pointer.gameObject.activeSelf)
				pointer.gameObject.SetActive (false);
		} else {
			if (!pointer.gameObject.activeSelf)
				pointer.gameObject.SetActive (true);
		}
	}

	void resetPointerRange ()
	{
		pointerRange = rayCastMaxLimit;
		pointerMaxRange = rayCastMaxLimit;
		pointerMinRange = rayCastMinLimit;
		movePointer ();
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

	public override void afterUpdate ()
	{
		previousEulerAngles = eulerAngles;
		velocity = Vector3.zero;
	}

	public override Vector3 getCurrentMovement ()
	{
		return velocity;
	}

	public override Vector3 getCurrentRotation ()
	{
		return eulerAngles - previousEulerAngles ;
	}
}
