using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {
    public bool DebugOn = true;
    enum gamestates { inBetween, playerTurn, opponentTurn };
    gamestates gamestate = gamestates.playerTurn;
    public List<int[]> tileLinks = new List<int[]>();
    public List<GameObject> faces = new List<GameObject>();
    List<pentagon> pentagons = new List<pentagon>();
    List<GameObject> toads = new List<GameObject>();
    public GameObject playerObject;
    public GameObject toadObject;
    int index = 0;
    // Use this for initialization
    void Start() {
        // define tile network
        tileLinks.Add(new int[] { 2, 3, 4, 5, 6 }); // face 1
        tileLinks.Add(new int[] { 9, 3, 1, 6, 10 }); // face 2
        tileLinks.Add(new int[] { 8, 4, 1, 2, 9 }); // face 3
        tileLinks.Add(new int[] { 12, 5, 1, 3, 8 }); // face 4
        tileLinks.Add(new int[] { 11, 6, 1, 4, 12 }); // face 5
        tileLinks.Add(new int[] { 10, 2, 1, 5, 11 }); // face 6
        tileLinks.Add(new int[] { 8, 9, 10, 11, 12 }); // face 7
        tileLinks.Add(new int[] { 3, 9, 7, 12, 4 }); // face 8
        tileLinks.Add(new int[] { 2, 10, 7, 8, 3 }); // face 9
        tileLinks.Add(new int[] { 6, 11, 7, 9, 2 }); // face 10
        tileLinks.Add(new int[] { 5, 12, 7, 10, 9 }); // face 11
        tileLinks.Add(new int[] { 4, 8, 7, 11, 5 }); // face 12

        for(int i = 0; i < tileLinks.Count; i++)
        {
            pentagon p = new pentagon();
            p.SetTile(faces[i]);
            p.SetIndex(i);
            p.SetLinks(tileLinks[i]);
            p.SetState(0);
            p.SetOccupied(false);
            

            pentagons.Add(p);
        }

        foreach(pentagon p in pentagons)
        {
            p.PrintAll();
        }

        
        // Initialise player
        SpawnPlayer();

        SpawnToads(5);
    }

    class pentagon
    {
        GameObject tile;
        GameObject occupierObject;
        int index; // 1 - 12
        int[] links; // which five other pentagons are adjacent
        bool occupied;
        int state; // 0 blank, 1 black, 2 white, 3 red, 4 gold


        public GameObject GetTile()
        {
            return tile;
        } 
        public void SetTile(GameObject g)
        {
            tile = g;
        }
        public GameObject GetOccupier()
        {
            return occupierObject;
        }
        public void SetOccupier(GameObject occupier)
        {
            occupierObject = occupier;
        }
        public int GetIndex()
        {
            return index;
        }
        public void SetIndex(int i)
        {
            i = Mathf.Clamp(i, 0, 11);
            index = i;
        }
        public int[] GetLinks()
        {
            return links;
        }
        public void SetLinks(int[] l)
        {
            links = l;
        }
        public bool GetOccupied()
        {
            return occupied;
        }
        public void SetOccupied(bool isOccupied = false)
        {
            occupied = isOccupied;
        }
        public int GetState()
        {
            return state;
        }
        public void SetState(int st = 0)
        {
            state = Mathf.Clamp(st, 0, 4);
        }
        
        public void PrintAll()
        {
            Debug.Log(string.Format("Pentagon index: {0} Occupied: {1} State: {2}", index, occupied, state));
            for(int i = 0; i < links.Length; i++)
            {
                Debug.Log("link " + i + " : " + links[i]);
            }
        }
    }

    // Update is called once per frame
    void Update() {
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

        // player turn
        if (gamestate == gamestates.playerTurn)
        {
            int newMove = Movement(index);

            if (newMove != -1)
            {
                index = newMove - 1;
                // move player object
                MovePlayerObject(newMove);
            }
            UpdateMoveOptions(index);
        }
    }

    void MovePlayerObject(int targetTile, float offset = -0.1f)
    {
        if (pentagons[targetTile - 1].GetOccupied())
        {
            // attack and destroy opponent if it can be destroyed
            GameObject o = pentagons[targetTile - 1].GetOccupier();
            // Check tile status
            // blanks can be black

            // black can be next 

            // black
            Destroy(o, 0.1f);
            

        }

        playerObject.transform.position = (pentagons[targetTile - 1].GetTile().transform.position);
            playerObject.transform.SetParent(pentagons[targetTile - 1].GetTile().transform);
            playerObject.transform.rotation = pentagons[targetTile - 1].GetTile().transform.rotation;
            playerObject.transform.eulerAngles += new Vector3(0, 0, 180);
            playerObject.transform.Translate(Vector3.forward * offset);
        
    }

    void UpdateMoveOptions(int index)
    {
        // wipe all colouration
        foreach (GameObject g in faces)
        {
            g.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            foreach (Transform t in g.transform)
            {
                if (t.gameObject.tag == "Pentagon")
                {
                    t.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                }
            }
        }
        // colour the selected tile
        faces[index].GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        // colour tiles adjacent to selected
        foreach (int i in tileLinks[index])
        {
            faces[i - 1].GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            foreach (Transform t in faces[i - 1].transform)
            {

                if (t.gameObject.tag == "Pentagon")
                {
                    t.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                }
            }

        }
    }

    int Movement(int sourceFace)
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

//        Debug.Log(targetFace);
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

        foreach (int tile in tileLinks[sourceTile])
        {
            if (targetTile == (tile - 1))
            {
                return 1;
            }
        }

        foreach (int tile in tileLinks[sourceTile])
        {
            foreach (int secondTile in tileLinks[(tile - 1)])
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

    void SpawnPlayer()
    {
        PlaceOnFace(playerObject, 0, -0.2f);
    }

    // Spawn toads
    void SpawnToads(int amount)
    {
        // first level spawn
        // don't spawn next to player, don't spawn on top of each other
        int[] tilesSpawnedOn = new int[amount + 1];
        // player should be on tile 1
        tilesSpawnedOn[0] = 0;


        int targetTile = Random.Range(0, 11);
        int created = 0;
        while (created < amount)
        {

            while (ProximityCheck(0, targetTile) < 2 || tilesSpawnedOn.Contains(targetTile))
            {
                targetTile = Random.Range(0, 11);
            }

            GameObject newToad = Instantiate(toadObject, transform.position, Quaternion.identity) as GameObject;

            PlaceOnFace(newToad, targetTile);

            tilesSpawnedOn[created + 1] = targetTile;

            pentagons[targetTile].SetOccupied(true);
            pentagons[targetTile].SetOccupier(newToad);
            Debug.Log("Creating toad on tile " + targetTile);


            created += 1;

        }

    }

   void PlaceOnFace(GameObject g, int targetTile, float offset = 0.2f)
    {
       
        g.transform.position = (faces[targetTile].transform.position);
        g.transform.Translate(Vector3.forward * offset);
        g.transform.SetParent(faces[targetTile].transform);
        g.transform.rotation = faces[targetTile].transform.rotation;
        g.transform.eulerAngles += new Vector3(0, 0, 180);
    }


}
