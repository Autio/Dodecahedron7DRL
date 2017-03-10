using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour {
    public GameObject marker;
    public float distance = 1f;
    public float zOffset = 0f;
	// Use this for initialization
	void Start () {
        GameObject newMarker = GameObject.Instantiate(marker, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, this.transform) as GameObject;
        newMarker.transform.Translate(Vector3.up * -distance);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
