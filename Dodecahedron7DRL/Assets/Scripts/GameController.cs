using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public bool DebugOn = true;
    enum gamestates { inBetween, playerTurn, opponentTurn };
    gamestates gamestate = gamestates.playerTurn;
    public List<int[]> tileLinks = new List<int[]>();
    public List<GameObject> faces = new List<GameObject>();
    List<pentagon> pentagons = new List<pentagon>();
    List<GameObject> toads = new List<GameObject>();
    List<GameObject> eagles = new List<GameObject>();
    List<GameObject> allOpponents = new List<GameObject>();
    public GameObject playerObject;
    public GameObject toadObject;
    public GameObject eagleObject;
    int index = 0;
    int turnCounter = 0;
    public GameObject mainEventLog;
    string log = "";

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
        tileLinks.Add(new int[] { 5, 12, 7, 10, 6 }); // face 11
        tileLinks.Add(new int[] { 4, 8, 7, 11, 5 }); // face 12

        for (int i = 0; i < tileLinks.Count; i++)
        {
            pentagon p = new pentagon();
            p.SetTile(faces[i]);
            p.SetIndex(i);
            p.SetLinks(tileLinks[i]);
            p.SetState(0);
            p.SetOccupied(false);
            pentagons.Add(p);
        }

        foreach (pentagon p in pentagons)
        {
            p.PrintAll();
        }


        // Initialise player
        SpawnPlayer();

        SpawnToads(2);
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

            if(Input.GetKeyDown(KeyCode.B))
            {
                SpawnEagles(2);
            }

            if (newMove != -1)
            {
                index = newMove - 1;
                // move player object
                if (MovePlayerObject(newMove))
                {
                    gamestate = gamestates.opponentTurn;
                    // increment turn
                    turnCounter += 1;
                    GameObject.Find("MovesTaken").GetComponent<Text>().text = "Moves taken: " + turnCounter.ToString();

                    try
                    {
                        if (toads.Count > 0)
                        {
                            for (int i = 0; i < toads.Count; i++)
                            {
                                BasicAIMove(toads[i]);
                            }
                        }
                        if (eagles.Count > 0)
                        {
                            for (int i = 0; i < eagles.Count; i++)
                            {
                                BasicAIMove(eagles[i]);
                            }
                        }
                        
                    }
                    catch
                    {
                        Debug.Log("No opponents left");
                
                    }
                    gamestate = gamestates.playerTurn;
                }
            }
            //     UpdateMoveOptions(index);
        }
    }

    void MoveObject(pentagon sourceTile, pentagon targetTile, float offset = -0.1f)
    {
        GameObject s = sourceTile.GetOccupier();

        // move transform
        s.transform.position = (targetTile.GetTile().transform.position);
        s.transform.SetParent(targetTile.GetTile().transform);
        s.transform.rotation = targetTile.GetTile().transform.rotation;
        s.transform.eulerAngles += new Vector3(0, 0, 180);
        s.transform.Translate(Vector3.forward * offset);

        // do tile handling
        sourceTile.SetOccupier(null);
        targetTile.SetOccupier(s);
        sourceTile.SetOccupied(false);
        targetTile.SetOccupied(true);
    }

    bool MovePlayerObject(int targetTile, float offset = -0.1f)
    {
        bool validMove = true;
        pentagon p = pentagons[targetTile - 1];
        if (p.GetOccupied())
        {
            // attack and destroy opponent if it can be destroyed
            GameObject o = p.GetOccupier();
            // Check tile status
            // blanks can come on blanks
            try
            {
                validMove = true;
                if (o.transform.tag == "BlackToad")
                {
                    if (p.GetState() == 0)
                    {
                        p.SetOccupier(null);
                        p.SetState(1);
                        toads.Remove(o);
                        allOpponents.Remove(o);
                        Destroy(o, 0.1f);
                        // colour the tile black
                        SetPentagonColour(Color.black, p.GetTile());
                        Log("You stomp on the wretched black little thing and smush it to pieces");

                    }
                }

                else if (o.transform.tag == "WhiteEagle")
                {
                    if (p.GetState() == 1)
                    {
                        p.SetOccupier(null);
                        p.SetState(1);
                        allOpponents.Remove(o);

                        Destroy(o, 0.1f);

                        // colour the tile black
                        SetPentagonColour(Color.cyan, p.GetTile());
               

                    }
                }

                else if (o.transform.tag == "RedPelican")
                {
                    if (p.GetState() == 2)
                    {
                        p.SetOccupier(null);
                        p.SetState(3);
                        allOpponents.Remove(o);
                        Destroy(o, 0.1f);
                        // colour the tile black
                        SetPentagonColour(Color.red, p.GetTile());
       
                    }


                }

                else if (o.transform.tag == "GreenLion")
                {
                    if (p.GetState() == 4)
                    {
                        p.SetOccupier(null);
                        p.SetState(3);
                        allOpponents.Remove(o);

                        Destroy(o, 0.1f);
                        // colour the tile black
                        SetPentagonColour(Color.yellow, p.GetTile());
       
                    }


                }
                else
                {
                    // if the opponent isn't standing on a proper tile then you can't move there
                    Debug.Log("Can't move there, wrong tile and opponent combination");
                    validMove = false;
                }



            }
            catch
        {
            Debug.Log("Couldn't access tile occupier");

        }

        // black can be next 

        // black


    }
        if (validMove)
        {
            playerObject.transform.position = (p.GetTile().transform.position);
            playerObject.transform.SetParent(p.GetTile().transform);
            playerObject.transform.rotation = p.GetTile().transform.rotation;
            playerObject.transform.eulerAngles += new Vector3(0, 0, 180);
            playerObject.transform.Translate(Vector3.forward * offset);
            // remove player from current tile and move them to the next tile in the pentagon list
            foreach(pentagon p2 in pentagons)
            {
                if(p2.GetOccupier() == playerObject)
                {
                    p2.SetOccupier(null);
                    Debug.Log("Setting player to tile index " + p.GetIndex());
                    p.SetOccupier(playerObject);

                }
            }
            

        }
        return validMove;
    }

    void BasicAIMove(GameObject e)
    {
        // pick an enemy -> pass it in

        // check proximity to player
        int enemyTileIndex = 0;
        int playerTileIndex = 0;
        pentagon enemyTile = null;
        pentagon targetTile = null;
        
        foreach(pentagon p in pentagons)
        {
            if(p.GetOccupier() == e)
            {
                enemyTileIndex = p.GetIndex();
                enemyTile = p;
            }
            if(p.GetOccupier() == playerObject)
            {
                playerTileIndex = p.GetIndex();
            }
        }

        int proximity = ProximityCheck(enemyTileIndex, playerTileIndex);
        // if player is next door, attack them and they die
        EndGame();

        // if player is two steps away, check the proximity to the player from each neighbouring tile
        int[] possibleMoves = new int[5] { -1, -1, -1, -1, -1 };

        if (proximity == 2)
        {
            
            for (int i = 0; i < 5; i++)
            {
                int s = tileLinks[enemyTileIndex][i] - 1;
                if (ProximityCheck(s, playerTileIndex) == 1)
                {
                    foreach(pentagon p in pentagons)
                    {
                        if(p.GetIndex() == s)
                        {
                            if(p.GetOccupier() == null)
                            {
                                possibleMoves[i] = s;
                            }
                        }
                    }

                }
            }
        }
        int chosenMove = -1;

        // Choose random move and make it
        if (Random.Range(0, 10) > 5)
        {
            for (int i = 0; i < 5; i++)
            {
                if(possibleMoves[i] != -1)
                {
                    chosenMove = possibleMoves[i];
                    break;
                }
            }
        }
        else
        {
            for (int i = 4; i >= 0; i--)
            {
                if (possibleMoves[i] != -1)
                {
                    chosenMove = possibleMoves[i];
                    break;
                }
            }

        }
        if (chosenMove != -1)
        {
            foreach(pentagon p in pentagons)
            {
                if(p.GetIndex() == chosenMove)
                {
                    targetTile = p;
                }
            }

            MoveObject(enemyTile, targetTile, -0.1f);
        }


        // if player is three steps away, plot course there - maybe it could just wait?

        int x = 1;
    }


    void SetPentagonColour(Color c, GameObject p)
    {
        p.GetComponent<Renderer>().material.SetColor("_Color", c);
        foreach (Transform t in p.transform)
        {

            if (t.gameObject.tag == "Pentagon")
            {
                t.GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }
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
        pentagons[0].SetOccupied(true);
        pentagons[0].SetOccupier(playerObject);
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
                targetTile = Random.Range(1, 11);
            }

            GameObject newToad = Instantiate(toadObject, transform.position, Quaternion.identity) as GameObject;

            PlaceOnFace(newToad, targetTile);
            tilesSpawnedOn[created + 1] = targetTile;
            pentagons[targetTile].SetOccupied(true);
            pentagons[targetTile].SetOccupier(newToad);
            Debug.Log("Creating toad on tile " + targetTile);
            toads.Add(newToad);
            allOpponents.Add(newToad);
            created += 1;

            int r = Random.Range(0, 10);
            if(r < 3)
            {
                Log("A black toad appears");

            } else if (r < 6)
            {
                Log("Some crack in the floor has brought forth a black toad");
            } else
            {
                Log("You hear the bristly ribbit of a nearby black toad");
            }

            

        }

    }

    void SpawnEagles(int amount)
    {
        // second level spawn
        // don't spawn next to player, don't spawn on top of each other
        List<int> occupiedTiles = new List<int>();
        for (int i = 0; i < 12; i++)
        {
            if(pentagons[i].GetOccupier() != null)
            {
                occupiedTiles.Add(i);
            }
        }

        int attempts = 40;
        int targetTile = Random.Range(0, 11);
        int created = 0;
        
        while (created < amount)
        {

            while (ProximityCheck(0, targetTile) < 1 || occupiedTiles.Contains(targetTile))
            {
                attempts--;
                targetTile = Random.Range(1, 11);
            }

            GameObject newEagle = Instantiate(eagleObject, pentagons[targetTile].GetTile().transform.position, Quaternion.identity) as GameObject;

            PlaceOnFace(newEagle, targetTile, -0.2f);
            occupiedTiles.Add(targetTile);
            pentagons[targetTile].SetOccupied(true);
            pentagons[targetTile].SetOccupier(newEagle);
            
            Debug.Log("Creating eagle on tile " + targetTile);
         
            eagles.Add(newEagle);
            allOpponents.Add(newEagle);

            created += 1;

            if(attempts<0)
            {
                break;
            }
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
            for (int i = 0; i < links.Length; i++)
            {
                Debug.Log("link " + i + " : " + links[i]);
            }
        }
    }


    void EndGame()
    {
        Debug.Log("GAME OVER");
    }

    void Log(string s)
    {
        string output = s + "\n" + log;
        log = output;
        mainEventLog.GetComponent<Text>().text = log;
    }
}
