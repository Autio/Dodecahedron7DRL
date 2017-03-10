using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    float rotX, rotY, rotZ;
    public float speed = 1.0f;
	// Use this for initialization
	void Start () {
        RotationReset();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.R))
        {
            RotationReset();
        }
        transform.Rotate(new Vector3(rotX, rotY, rotZ) * Time.deltaTime * speed); 
	}
    void RotationReset()
    {
        rotX = Random.Range(-1f, 1f);
        rotY = Random.Range(-1f, 1f);
        rotZ = Random.Range(-1f, 1f);
    }
}
