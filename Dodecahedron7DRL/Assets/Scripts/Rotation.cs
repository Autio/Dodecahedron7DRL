using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
    public bool autoRotate = false;
    float rotX, rotY, rotZ;
    public float speed = 1.0f;

    float hackCounter = 1.0f;// making rotation less annoying with a workaround

    float angleOffset = 0.0f;
    float angleStart = 0.0f;
    float camDist;

    // manual rotation 
    /*
    private void OnMouseDrag()
    {
        Vector3 mousePos = (new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDist));
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
        float angleT = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        angleStart = angleT + angleOffset;
        transform.rotation = Quaternion.AngleAxis(angleStart, Vector3.forward); 

    }
    */
    private void OnMouseOver()
    {

        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            if (hackCounter > 0)
            {
                hackCounter -= Time.deltaTime;
                float rotX = Input.GetAxis("Mouse X") * speed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * speed * Mathf.Deg2Rad;
                float rotZ = transform.rotation.z;
                //            transform.Rotate(Vector3.up, -rotX);
                transform.Rotate(rotY, -rotX, rotZ, Space.World);
                //           transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, rotZ));          
            }
  
        }
        else
        {
            hackCounter = 1.0f;
        }
    }

    
    
    // Use this for initialization
	void Start () {

        camDist = transform.position.z - Camera.main.transform.position.z;
        RotationReset();
	}
	
	// Update is called once per frame
	void Update () {
        if (autoRotate)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                RotationReset();
            }
            transform.Rotate(new Vector3(rotX, rotY, rotZ) * Time.deltaTime * speed);
        } 
	}
    void RotationReset()
    {
        rotX = Random.Range(-1f, 1f);
        rotY = Random.Range(-1f, 1f);
        rotZ = Random.Range(-1f, 1f);
    }



}
