using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {

	public GameObject floorTarget;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPosition(Vector3 position) {
		this.transform.position = position;
		position.y = 0;
		this.floorTarget.transform.position = position;
	}
}
