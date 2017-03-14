using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public bool DebugOn = true;
    bool gameOver =false;
    enum gamestates { inBetween, playerTurn, opponentTurn };
    gamestates gamestate = gamestates.playerTurn;
    enum enemyTypes { toad, eagle, pelican, lion, gold };
    public List<int[]> tileLinks = new List<int[]>();
    public List<GameObject> faces = new List<GameObject>();
    List<pentagon> pentagons = new List<pentagon>();
    List<GameObject> toads = new List<GameObject>();
    List<GameObject> eagles = new List<GameObject>();
    List<GameObject> pelicans = new List<GameObject>();
    List<GameObject> lions = new List<GameObject>();
    List<GameObject> allOpponents = new List<GameObject>();
    public GameObject playerObject;
    public GameObject toadObject;
    public GameObject eagleObject;
    public GameObject pelicanObject;
    public GameObject lionObject;
    public GameObject goldObject;
    public GameObject retortObject;
    // effects
    public GameObject sprayEffect;

    int[] enemyStrengths = { 1, 2, 3, 4 }; // toad, eagle, pelican, lion
    int index = 0;
    int turnCounter = 0;
    int turnLastSpawned = 0;
    int enemyTypeCounter = 0;
    int hp = 20; // when 0 die
    int fullHP = 20;
    int gold = 1;
    int retorts = 3;
    public GameObject mainEventLog;
    string log = "";

    // Use this for initialization
    void Start() {
        int startPhrase = Random.Range(0, 10);
        if (startPhrase < 5)
        {
            Log("The old unperishable urge overtakes you once more so you find yourself here.");
        } else
        {
            Log("You've made time for this so it had better count.");
        }
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

        SpawnEnemy(enemyTypes.toad, 2);
        SpawnEnemy(enemyTypes.gold, 2);
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
            if(Input.GetKeyDown(KeyCode.O))
            {
                BuyDrillThrough();
            }
        }

        // Reset stage
        if(Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(0);
        }

        // movement on the grid
        // Q E
        // A D
        //  X

        // S for wait / stand still

        if(gameOver)
        {
            gamestate = gamestates.inBetween;
        }


        // player turn
        if (gamestate == gamestates.playerTurn)
        {
            if (DebugOn)
            {
                if (Input.GetKeyDown(KeyCode.B))
                {
                    SpawnEnemy(enemyTypes.eagle, 2);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SpawnEnemy(enemyTypes.pelican, 2);
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    SpawnEnemy(enemyTypes.lion, 2);
                }
            }

            // Item purchasing
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                BuyDrillThrough();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BuyHealing();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                BuyAttack();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
//                DropRetort();
            }


            // go fetch new move from player input
            int newMove = Movement(index);
         

            // Regular moves
            if (newMove != -1)
            {
                index = newMove - 1;
               

                // move player object
                if (MovePlayerObject(newMove))
                {
                    gamestate = gamestates.opponentTurn;
                    // increment turn
                    turnCounter += 1;
                    // When turns pass, it's time to spawn new enemies
                    // check how many tiles are occupied already, don't spawn if more than x are occupied
                    int tilesOccupied = 0;
                    foreach(pentagon p in pentagons)
                    {
                        if(p.GetOccupied())
                        {
                            tilesOccupied++;
                        }
                    }
                    if(tilesOccupied < 8)
                    {
                        if ((turnCounter - turnLastSpawned) > 5)
                        {
                            int r = Random.Range(0, 10);
                            if(r < 5)
                            {
                                // spawn next enemy
                                turnLastSpawned = turnCounter;
                                // either spawn more toads, or the next in the wave
                                int s = Random.Range(0, 10);
                                if (s < 4)
                                {
                                    // spawn toads, but only if the total sum of live toads
                                    // and tiles that are at status one or more is less than 11
                                    int toadTotal = 0;
                                    foreach (pentagon p in pentagons)
                                    {
                                        try
                                        {
                                            if (p.GetOccupier().transform.tag == "BlackToad")
                                            {
                                                toadTotal++;
                                            }
                                            if (p.GetState() > 0)
                                            {
                                                toadTotal++;
                                            }
                                        }
                                        catch
                                        {
                                            Debug.Log("Could not get occupier of pentagon " + p.GetIndex());
                                        }
                                    }
                                    if (toadTotal < 11)
                                    {
                                        SpawnEnemy(enemyTypes.toad, 2);
                                    }
                                }
                                else
                                { 
                                    // spawn something else, and do it in sequence
                                    enemyTypeCounter += 1;
                                    if (enemyTypeCounter > 3)
                                    {
                                        enemyTypeCounter = 0;
                                    }
                                    if (enemyTypeCounter == 1)
                                    {
                                        SpawnEnemy(enemyTypes.eagle, 2);
                                    } else if(enemyTypeCounter == 2)
                                    {
                                        SpawnEnemy(enemyTypes.pelican, 2);

                                    } else if(enemyTypeCounter == 3)
                                    {
                                        SpawnEnemy(enemyTypes.lion, 2);
                                        enemyTypeCounter = 1;
                                    }
                                }

                                // sometimes also spawn loot
                                if(Random.Range(0,10) < 5)
                                {
                                    SpawnEnemy(enemyTypes.gold, 1);
                                }
                            }    
                        }
                    }

                    GameObject.Find("MovesTaken").GetComponent<Text>().text = "Moves taken: " + turnCounter.ToString();
                    GameObject.Find("HP").GetComponent<Text>().text = "HP: " + hp.ToString();
                    GameObject.Find("Gold").GetComponent<Text>().text = "Gold: " + gold.ToString();

                    // Check for win condition
                    // All tiles golden
                    CheckEnd();

                    // AI moves
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
                        if (pelicans.Count > 0)
                        {
                            for (int i = 0; i < pelicans.Count; i++)
                            {
                                BasicAIMove(pelicans[i]);
                            }
                        }
                        if (lions.Count > 0)
                        {
                            for (int i = 0; i < lions.Count; i++)
                            {
                                BasicAIMove(lions[i]);
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

    bool MovePlayerObject(int targetTile, float offset = -0.1f, bool attackOnly = false)
    {
        bool validMove = true;
        bool waiting = false;
        pentagon p = pentagons[targetTile - 1];
        if (p.GetOccupied())
        {
            validMove = false;
            // attack and destroy opponent if it can be destroyed
            GameObject o = p.GetOccupier();
            // Check tile status
            // blanks can come on blanks
            try
            {
                // pickup gold
                if (o.transform.tag == "GoldPiece")
                {
                    gold += 1;
                    validMove = true;
                    p.SetOccupier(null);
                    allOpponents.Remove(o);
                    Destroy(o, 0.1f);
                    Log("You pick up some gold.");
                }

                else if (o.transform.tag == "BlackToad")
                {
                    if (p.GetState() == 0)
                    {
                        validMove = true;
                        p.SetOccupier(null);
                        p.SetState(1);
                        toads.Remove(o);
                        allOpponents.Remove(o);
                        Destroy(o, 0.1f);
                        // colour the tile black
                        SetPentagonColour(Color.black, p.GetTile());
                        Log("You stomp on the wretched black little thing and smush it to pieces.");

                    }
                    else
                    {
                        Log("You cannot best the toad there");
                    }
                }

                else if (o.transform.tag == "WhiteEagle")
                {
                    if (p.GetState() == 1)
                    {
                        validMove = true;
                        p.SetOccupier(null);
                        p.SetState(2);
                        allOpponents.Remove(o);
                        eagles.Remove(o);

                        Destroy(o, 0.1f);

                        // colour the tile bright white
                        SetPentagonColour(Color.white, p.GetTile());
               
                    }
                    else
                    {
                        Log("You cannot trap the eagle on that side.");
                    }
                }

                else if (o.transform.tag == "RedPelican")
                {
                    if (p.GetState() == 2)
                    {
                        validMove = true;
                        p.SetOccupier(null);
                        p.SetState(3);
                        allOpponents.Remove(o);
                        pelicans.Remove(o);
                        Destroy(o, 0.1f);
                        // colour the tile black
                        SetPentagonColour(Color.red, p.GetTile());
       
                    }
                    else
                    {
                        Log("The pelican needs to be on a white tile to be tamed.");
                    }

                }

                else if (o.transform.tag == "GreenLion")
                {
                    if (p.GetState() == 3)
                    {
                        validMove = true;
                        p.SetOccupier(null);
                        p.SetState(3);
                        allOpponents.Remove(o);
                        lions.Remove(o);

                        Destroy(o, 0.1f);
                        // colour the tile yellow
                        SetPentagonColour(Color.yellow, p.GetTile());

                        // Player gets gold
                        Log("The lion had swallowed gold! You pick up what's left.");
                        gold += 2;
                        GameObject.Find("Gold").GetComponent<Text>().text = "Gold " + gold.ToString();

                    }
                    else
                    {
                        Log("The lion will yield only when stood upon a red base.");
                    }
                }
                // staying still
                else if (p.GetOccupier() == playerObject)
                {
                    Log("You stand still.");
                    validMove = true;
                    waiting = true;
                } else 
                {
                    // if the opponent isn't standing on a proper tile then you can't move there
                    Debug.Log("Can't move there, wrong tile and opponent combination");
                    validMove = false;
                }
            }
            catch
        {
            Debug.Log("Couldn't access tile occupier");
                validMove = false;
        }

    }
        if (validMove)
        {
            pentagon source = null;
            foreach (pentagon p2 in pentagons)
            {
                if (p2.GetOccupier() == playerObject)
                {
                    source = p2;
                }
            }

            if (!attackOnly)
            {
                MoveObject(source, p);
            } else
            {
                if (!waiting)
                {
                    GameObject FX = Instantiate(sprayEffect, p.GetTile().transform);
                    FX.transform.position = p.GetTile().transform.position;
                    FX.transform.LookAt(-GameObject.Find("Dodecahedron").transform.position);
                    Destroy(FX, 2.0f);
                }
            }
        /*    playerObject.transform.position = (p.GetTile().transform.position);
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
            */
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
        if (proximity == 1)
        {
            AttackPlayer(enemyTile);
        }
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

    int Movement(int sourceFace = 0)
    {
        int[] sourceLinks = new int[5];
        // get sourceFace from the tile the player is occupying
        foreach (pentagon p in pentagons)
        {
            try
            {
                if (p.GetOccupier().transform.tag == "Player")
                {
                    sourceLinks = p.GetLinks();
                }
            }
            catch
            {
              
            }
        } 

        int targetFace = -1;
        // manual movement
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            targetFace = sourceLinks[0];
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            targetFace = sourceLinks[1];
        }
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            targetFace = sourceLinks[2];
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            targetFace = sourceLinks[3];
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            targetFace = sourceLinks[4];
        }
        // wait
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            targetFace = sourceFace + 1;
        }

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
        PlaceOnFace(playerObject, 0, -0.1f);
        pentagons[0].SetOccupied(true);
        pentagons[0].SetOccupier(playerObject);
    }

    void SpawnEnemy(enemyTypes e, int amount)
    {
        // Also applicable to any item
        GameObject enemyObject = null;
        if(e == enemyTypes.toad)
        {
            enemyObject = toadObject;
        } else if (e == enemyTypes.eagle)
        {
            enemyObject = eagleObject;
        } else if (e == enemyTypes.pelican)
        {
            enemyObject = pelicanObject;
        } else if (e == enemyTypes.lion)
        {
            enemyObject = lionObject;
        } else if (e == enemyTypes.gold)
        {
            enemyObject = goldObject;
        }

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
        
        // reroll if necessary
        while (created < amount)
        {
            while (ProximityCheck(0, targetTile) < 1 || occupiedTiles.Contains(targetTile))
            {
                attempts--;
                targetTile = Random.Range(1, 11);
            }

            GameObject newEnemy = Instantiate(enemyObject, pentagons[targetTile].GetTile().transform.position, Quaternion.identity) as GameObject;

            PlaceOnFace(newEnemy, targetTile, -0.05f);
            occupiedTiles.Add(targetTile);
            pentagons[targetTile].SetOccupied(true);
            pentagons[targetTile].SetOccupier(newEnemy);
            
            allOpponents.Add(newEnemy);

            created += 1;

            if (e == enemyTypes.toad)
            {
                toads.Add(newEnemy);
                Debug.Log("Creating toad on tile " + targetTile);
                string[] creationMessages =
                {
                    "A black toad appears.",
                    "Another black toad appears.",
                    "You feel the presence of a black toad nearby.",
                    "A black toad appears. You resolve to not fear that darkness.",
                    "Dark foamy spawn has grown into a full toad. It stares at you fixedly.",
                    "Some crack in the floor has brought forth a black toad.",
                    "You hear the bristly ribbit of a nearby black toad.",
                    "There is no shortage of black toads in this world and here comes another one.",
                    "A black toad takes its allotted place and lets out a mournful croak.",
                    "A loud, lustful song lets you know a black toad has appeared."
                };

                Log(creationMessages[Random.Range(0, creationMessages.Count())]);

            }
            if (e == enemyTypes.eagle)
            {
                eagles.Add(newEnemy);
                Debug.Log("Creating eagle on tile " + targetTile);
                string[] creationMessages =
                {
                    "A white eagle swoops down near you.",
                    "A wild eagle plumed in brilliant white lands nearby."
                };

                Log(creationMessages[Random.Range(0, creationMessages.Count())]);

            }
            if (e == enemyTypes.pelican)
            {
                pelicans.Add(newEnemy);
                Debug.Log("Creating pelican on tile " + targetTile);
                string[] creationMessages =
                {
                    "You notice a pelican snorting loudly nearby, its chest covered in red entrails.",
                    "You suddenly smell fish and vomit. A red, flightless pelican is stepping towards you."
                };

                Log(creationMessages[Random.Range(0, creationMessages.Count())]);

            }
            if (e == enemyTypes.lion)
            {
                lions.Add(newEnemy);
                Debug.Log("Creating lion on tile " + targetTile);
                string[] creationMessages =
                {
                    "You see a green lion. Its strange figure is all the more terrifying for being somehow familiar.",
                    "A green lion is here, growling. It roars with bottomless hunger as if it could eat the sun.",
                    "The sound of heavy paws on stone alert you to the presence of a green lion"
                };
            }

            if (attempts<0)
            {
                break;
            }
        }
    }

    void PlaceOnFace(GameObject g, int targetTile, float offset = 0.1f)
    {
        g.transform.position = (faces[targetTile].transform.position);
        g.transform.SetParent(faces[targetTile].transform);
        g.transform.rotation = faces[targetTile].transform.rotation;
        g.transform.eulerAngles += new Vector3(0, 0, 180);
        g.transform.Translate(Vector3.forward * offset); // translate last
    }


    class pentagon
    {
        GameObject tile;
        GameObject occupierObject;
        GameObject trapObject = null;
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
        public GameObject GetTrapped()
        {
            return trapObject;
        }
        public void SetTrapped(GameObject trap)
        {
            trapObject = trap;
        }
        public void ClearTrap()
        {
            trapObject = null;
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

    void AttackPlayer(pentagon enemyTile)
    {
        int damage = 0;

        // Fire off an effect?
        GameObject newEffect = Instantiate(sprayEffect, enemyTile.GetTile().transform);
        float effectOffset = -0.8f;
        newEffect.transform.position = new Vector3(enemyTile.GetTile().transform.position.x, enemyTile.GetTile().transform.position.y, enemyTile.GetTile().transform.position.z + effectOffset);
        newEffect.transform.LookAt(playerObject.transform);
        Destroy(newEffect, 2.0f);

        List<string> damageMessages = new List<string>();
        // derive attack power from enemy type
        GameObject enemy = enemyTile.GetOccupier();

        if(enemy.transform.tag == "BlackToad")
        {
            damage = enemyStrengths[0];
            damageMessages.Add("The black toad spits a dark liquid at you and burns you for " + damage + " damage.");
            damageMessages.Add("The black toad attacks you for " + damage + " damage.");
            damageMessages.Add("The black toad jumps at you and before you can raise your hand it has dealt " + damage + " damage.");

        }
        if(enemy.transform.tag == "WhiteEagle")
        {
            damage = enemyStrengths[1];
            damageMessages.Add("The white eagle shrieks and tears at you causing " + damage + " damage.");
            damageMessages.Add("The eagle's cry makes your ears ring. You take " + damage + " damage.");
            damageMessages.Add("The quickened eagle gnaws at your face. You suffer " + damage + " damage.");

        }
        if (enemy.transform.tag == "RedPelican")
        {
            damage = enemyStrengths[2];
            damageMessages.Add("The pelican smashes its beak against you. " + damage + " damage.");
            damageMessages.Add("The red pelican swoops down at your face. You take " + damage + " damage.");
            damageMessages.Add("Angered, the red pelican takes a swipe at you. You take " + damage + " damage.");
        }
        if (enemy.transform.tag == "GreenLion")
        {
            damage = enemyStrengths[3];
            damageMessages.Add("The green lion leaps at you with overwhelming force. You take " + damage + " damage.");

        }

        damageMessages.Add("You take " + damage + " damage!");
        Log(damageMessages[Random.Range(0, damageMessages.Count())]);
        
        hp -= damage;
        GameObject.Find("HP").GetComponent<Text>().text = "HP: " + hp.ToString();

        // if player HP < 1
        if (hp < 1)
        {
            
            EndGame();
        }
    }
    // Items
    public void BuyDrillThrough()
    {
        int cost = 1;
        // effect: transport to other side of the map, if you can
        // Check first before buying
        int[] opposites = {7,12,11,10,9,8,1,2,3,4,5,6};
        pentagon player = null;
        pentagon opposite;
        foreach(pentagon p in pentagons)
        {
            try
            {
                if(p.GetOccupier().transform.tag == "Player")
                {
                    player = p;
                }
            }
            catch
            {
                Debug.Log("Had an issue accessing the occupier");
            }
        }
        foreach(pentagon p in pentagons)
        {
            try
            {
                if(p.GetIndex() == opposites[player.GetIndex()] - 1)
                {
                    Debug.Log("The opposite of your tile is tile " + (opposites[player.GetIndex()]).ToString());
                    try
                    {

                    }
                    catch
                    {

                    }
                }
            }
            catch
            {
                Debug.Log("Couldn't find the index of the opposite side tile");
            }
        }

        if (gold >= cost)
        {
            if (MovePlayerObject(opposites[player.GetIndex()]))
            {
                gold -= cost;
                Log("You spend some gold to travel further");
                GameObject.Find("Gold").GetComponent<Text>().text = "Gold " + gold.ToString();
            }
            else
            {
                Log("You cannot travel far because the destination is blocked.");
            }

        } else
        {
            Log("You do not have enough gold to travel far.");
        }
    }
    public void BuyHealing()
    {
        int cost = 2;
        if(gold >= cost)
        {
            gold -= cost;
            // effect: restore full health
            hp = fullHP;

            Log("You feel better.");

            GameObject.Find("HP").GetComponent<Text>().text = "HP " + hp.ToString();
            GameObject.Find("Gold").GetComponent<Text>().text = "Gold: " + gold.ToString();
        } else
        {
            Log("You do not have enough gold to replenish yourself.");
        }
    }
    public void BuyAttack()
    {
        int cost = 3;
        if (gold >= cost)
        {
            gold -= cost;
            // effect: attack all adjacent tiles
            pentagon player = null;
            foreach (pentagon p in pentagons)
            {
                try
                {
                    if (p.GetOccupier().transform.tag == "Player")
                    {
                        player = p;
                    }
                }
                catch
                {

                }
            }

            int[] adjacentTiles = new int[5];
            adjacentTiles = player.GetLinks();
            for(int i = 0; i < 5; i++)
            {
                MovePlayerObject(tileLinks[player.GetIndex()][i], -0.1f, true);
            }

            GameObject.Find("HP").GetComponent<Text>().text = "HP " + hp.ToString();
            GameObject.Find("Gold").GetComponent<Text>().text = "Gold: " + gold.ToString();

        }
        else
        {
            Log("You do not have enough gold to be so vigorous and stir everything nearby.");
        }
    }


    void CheckEnd()
    {
        int goldenSides = 0;
        foreach(pentagon p in pentagons)
        {
            try
            {
                if(p.GetState() >= 4)
                {
                    goldenSides++;
                }
            }
            catch
            {
                
            }
        }
        if(goldenSides >= 12)
        {
            // Player wins
            Debug.Log("YOU WIN");
            gamestate = gamestates.inBetween;
            Log("Congratulations. You have covered all your sides in gold. Now, take that perfection and smash it into pieces.");
            Log("You've won the game. Play more? Press N for a new game.");
        }
    }

    public void DropRetort(int dropTileIndex = 0)
    {
        pentagon dropTile = null;
        foreach (pentagon p in pentagons)
        {
            if (p.GetIndex() == dropTileIndex)
            {
                dropTile = p;
            }
        }

        // check if any aludels remain in the inventory
        if (retorts > 0)
        {
            // check player tile for existing trap
            if (dropTile.GetTrapped() != null)
            {
                // drops a trap where the player is and if an opponent steps on the trap then they blow up
                retorts--;
                GameObject retort = Instantiate(retortObject, transform);
                //PlaceOnFace(aludel, )
                GameObject.Find("RetortLabel").GetComponent<Text>().text = "[R] Drop a retort. (" + retorts + " remain)";

            } else
            {
                Log("There already is a retort on this face!");
            }
        }
        else
        {
            Log("You don't have any retorts left!");
        }

        
        
        
        // implement stepping onto aludel for enemies
        
    }

    void EndGame()
    {
        // possible end messages
        List<string> endMessages = new List<string>();
        endMessages.Add("Your powers are spent for now.");
        endMessages.Add("You have been exhausted and the world blurs.");
        endMessages.Add("The struggle has overpowered you this time. You need some deep rest.");
        Log(endMessages[Random.Range(0, endMessages.Count())]);
        // how many golden sides?
        int goldenSides = 0;
        foreach(pentagon p in pentagons)
        {
            if(p.GetState() >= 4)
            {
                goldenSides++;
            }
        }

        Log("You took " + turnCounter + " turns, picked up " + gold + " pieces of gold and turned " + goldenSides + " sides into gold.");
        //   Log("This struggle has overcome you for now");
        gameOver = true;
        Log("Do you want to try again? Press N to start the new attempt");
        Debug.Log("GAME OVER");
    }

    void Log(string s)
    {
        string output = s + "\n" + log;
        log = output;
        mainEventLog.GetComponent<Text>().text = log;
    }
}
