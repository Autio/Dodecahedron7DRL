using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControls : MonoBehaviour
{

    float heightMax = 0f;
    float heightMin = -5f;
    float scrollSpeed = 2f;
    float height;
    void Update()
    {
        height += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Mathf.Clamp(height, heightMin, heightMax));
    }

    // Use this for initialization
}
