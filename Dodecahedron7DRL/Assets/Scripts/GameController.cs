using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {
    public bool DebugOn = true;
    public List<int[]> tileLinks = new List<int[]>();
    public List<GameObject> faces = new List<GameObject>();
    List<GameObject> toads = new List<GameObject>();
    public GameObject playerObject;
    public GameObject toadObject;
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

        // test distance check
        for (int st = 0; st < 12; st++)
        {
            for (int tt = 0; tt < 12; tt++)
            {
                int res = ProximityCheck(st, tt);
                Debug.Log("Target tile " + (tt + 1) + " is " + res + " steps away from the source tile " + (st + 1));
            }
        }

        SpawnToads(2);
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


    int ProximityCheck(int sourceTile, int targetTile)
    {
        // check how far the target tile is from the source tile

        // source and target are identical
        if (sourceTile == targetTile)
        {
            return 0;
        }

        foreach(int tile in tileLinks[sourceTile])
        {
            if(targetTile == (tile - 1))
            {
                return 1;
            }
        }

        foreach(int tile in tileLinks[sourceTile])
        {
            foreach(int secondTile in tileLinks[(tile-1)])
            {
                if (targetTile == (secondTile - 1))
                {
                    return 2;
                }
            }
        }

        // if it's nothing else, then it has to be on the opposite side
        return 3;

        
    }

    // Spawn toads
    void SpawnToads(int amount)
    {
        // first level spawn
        // don't spawn next to player, don't spawn on top of each other
        int[] tilesSpawnedOn = new int [amount + 1];
        // player should be on tile 1
        tilesSpawnedOn[0] = 0;


        int targetTile = Random.Range(0,11);
        int created = 0;
        while (created < amount)
        {

            while (ProximityCheck(0, targetTile) < 2 || tilesSpawnedOn.Contains(targetTile))
            {
                targetTile = Random.Range(0, 11);
            }

            GameObject newToad = Instantiate(toadObject, transform.position, Quaternion.identity) as GameObject;

            newToad.transform.position = (faces[targetTile].transform.position);
            newToad.transform.Translate(Vector3.forward * 0.1f);
            newToad.transform.SetParent(faces[targetTile].transform);
            newToad.transform.rotation = faces[targetTile].transform.rotation;
            newToad.transform.eulerAngles += new Vector3(0, 0, 180);

            tilesSpawnedOn[created + 1] = targetTile;
            Debug.Log("Creating toad on tile " + targetTile);

            created += 1;
            
        }



    }


}
