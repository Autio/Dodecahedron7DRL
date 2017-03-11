using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {
    public bool DebugOn = true;
    public List<int[]> tileLinks = new List<int[]>();
    public List<GameObject> faces = new List<GameObject>();
    public GameObject playerObject;

    int index = 0;
	// Use this for initialization
	void Start () {
        // define tile network
        tileLinks.Add(new int[] { 2, 3, 4, 5, 6}); // face 1
        tileLinks.Add(new int[] { 9, 3, 1, 6, 10}); // face 2
        tileLinks.Add(new int[] { 8, 4, 1, 2, 9}); // face 3
        tileLinks.Add(new int[] { 12, 5, 1, 3, 8}); // face 4
        tileLinks.Add(new int[] { 11, 6, 1, 4, 12}); // face 5
        tileLinks.Add(new int[] { 10, 2, 1, 5, 11}); // face 6
        tileLinks.Add(new int[] { 8, 9, 10, 11, 12}); // face 7
        tileLinks.Add(new int[] { 3, 9, 7, 12, 4}); // face 8
        tileLinks.Add(new int[] { 2, 10, 7, 8, 3}); // face 9
        tileLinks.Add(new int[] { 6, 11, 7, 9, 2}); // face 10
        tileLinks.Add(new int[] { 5, 12, 7, 10, 9}); // face 11
        tileLinks.Add(new int[] { 4, 8, 7, 11, 5}); // face 12

    }

    // Update is called once per frame
    void Update () {
        if (DebugOn)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                UpdateMoveOptions(index);
                index += 1;
                if (index > 11) index = 0;
            }
        }

        // movement on the grid
        // Q E
        // A D
        //  X

        int newMove = movement(index);
        if (newMove != -1)
        {
            index = newMove - 1;
            // move player object
            MovePlayerObject();
        }
        UpdateMoveOptions(index);
    }

    void MovePlayerObject()
    {
        playerObject.transform.position = (faces[index].transform.position);
        playerObject.transform.Translate(Vector3.forward * -0.075f);
        playerObject.transform.SetParent(faces[index].transform);
        playerObject.transform.rotation = faces[index].transform.rotation;
        playerObject.transform.eulerAngles += new Vector3(0, 0, 180);
        


    }

    void UpdateMoveOptions(int index)
    {
        // wipe all colouration
        foreach (GameObject g in faces)
        {
            g.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        // colour the selected tile
        faces[index].GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        // colour tiles adjacent to selected
        foreach (int i in tileLinks[index])
        {
            faces[i - 1].GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }

    }

    int movement(int sourceFace)
    {
        int targetFace = -1;
        // manual movement
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            targetFace = tileLinks[sourceFace][0];
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            targetFace = tileLinks[sourceFace][1];
        }
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            targetFace = tileLinks[sourceFace][2];
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            targetFace = tileLinks[sourceFace][3];
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            targetFace = tileLinks[sourceFace][4];
        }

        Debug.Log(targetFace);
        return targetFace;
    }
}
