using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControls : MonoBehaviour
{

    float heightMax = 2.5f;
    float heightMin = -8.5f;
    float scrollSpeed = 1.5f;
    float height;
    void Update()
    {
        height += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Mathf.Clamp(height, heightMin, heightMax));
        if(this.transform.position.z < heightMin)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, heightMin+ 0.2f);
        }
    }

    // Use this for initialization
}
