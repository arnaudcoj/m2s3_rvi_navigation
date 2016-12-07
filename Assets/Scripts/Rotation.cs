using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

	public float rotationSpeed = 100f;

	// Use this for initialization
	void Start () {
		//Debug.Log (gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate (Vector3.up, Time.deltaTime * rotationSpeed);
	}
}
