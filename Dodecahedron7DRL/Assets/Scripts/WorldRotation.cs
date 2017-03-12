using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotation : MonoBehaviour {
    public float speed = 100f;
	// Use this for initialization
	void Start () {
		
	}

    void update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
          //  Ray ray = .ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
            }
        }
    }
}
