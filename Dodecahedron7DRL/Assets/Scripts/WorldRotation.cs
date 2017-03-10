using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotation : MonoBehaviour {
    public float speed = 100f;
	// Use this for initialization
	void Start () {
		
	}

    private void OnMouseDrag()
    {
        float rotX = 0;
        rotX += Input.GetAxis("Mouse X") * speed * Mathf.Deg2Rad;
        //float rotY = Input.GetAxis("Mouse Y") * speed * Mathf.Deg2Rad;
    //    float rotZ = transform.rotation.z;
        transform.eulerAngles = new Vector3(rotX, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
