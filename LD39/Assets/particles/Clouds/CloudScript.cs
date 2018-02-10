using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

    public float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.rotation *= Quaternion.Euler(0, 0, -rotationSpeed*Time.deltaTime);

	}
}
