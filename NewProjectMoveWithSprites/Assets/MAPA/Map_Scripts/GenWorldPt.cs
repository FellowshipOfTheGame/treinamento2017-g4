using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenWorldPt : MonoBehaviour {
    [SerializeField]
    private GameObject[] MapVector;

    [SerializeField]
    private GameObject[] BossVector;

    private GameObject Map;
    private GameObject Boss;
    private int Stage;
    public static bool canReboot;
    public static bool gameHasEnded;
    private bool levelHasEnded;

    [SerializeField]
    private int numberOfRooms;

    public static bool canGenerate;
    public static Vector3 centerOfMainRoom;

    private float x, y, z;
    public static float vertical, horizontal;
    private bool[,] M;
    private bool[] doorsUsed;
    private int MPosx, MPosy;
    private int total;

    private enum Children {
        colliders,
        doors,
        doorsClosed,
        collidersMiddle,
        collidersMiddleTrigger,
        Spawners,
        Objects,
        Teleporter
    };

    private enum Directions {
        north,
        east,
        south,
        west
    };

    void debug_M() {
        string texto = "";
        for (int i = 0; i < numberOfRooms; i++) {
            for (int j = 0; j < numberOfRooms; j++)
                if (M[j, i])
                    texto += "1 ";
                else
                    texto += "0 ";
            texto += ("\n");
        }
        print(texto);
    }


    // Use this for initialization
    void Start () {
        levelHasEnded = false;
        canReboot = false;
        gameHasEnded = false;
        Stage = 0;
        Map = MapVector[Stage];
        Boss = BossVector[Stage];
        M = new bool[numberOfRooms, numberOfRooms];
        doorsUsed = new bool[4];
        MPosx = MPosy = numberOfRooms / 2;
        x = y = z = 0;
        vertical = Map.GetComponent<Renderer>().bounds.size.y;
        horizontal = Map.GetComponent<Renderer>().bounds.size.x;
        total = 0;
        canGenerate = false;

        int numDoors = (int)Random.Range(1, (int)Mathf.Min(maxDirections(), numberOfRooms - total));
        createRoom(Map, numDoors, -1, true);
        //debug_M();
    }

    private void nextLevel() {
        if (Stage + 1 == MapVector.Length) {
            gameHasEnded = true;
            enabled = false;
        } else {
            foreach (GameObject kill in Object.FindObjectsOfType<GameObject>())
                if (kill.name.Contains("Map:-> "))
                    Destroy(kill);
            levelHasEnded = false;
            canReboot = false;
            Stage++;
            Map = MapVector[Stage];
            Boss = BossVector[Stage];
            M = new bool[numberOfRooms, numberOfRooms];
            doorsUsed = new bool[4];
            MPosx = MPosy = numberOfRooms / 2;
            x = y = z = 0;
            vertical = Map.GetComponent<Renderer>().bounds.size.y;
            horizontal = Map.GetComponent<Renderer>().bounds.size.x;
            total = 0;
            canGenerate = false;

            int numDoors = (int)Random.Range(1, (int)Mathf.Min(maxDirections(), numberOfRooms - total));
            createRoom(Map, numDoors, -1, true);
        }
    }

    // Update is called once per frame
    void Update () {
        //print("num = " + total + "\t of: " + numberOfRooms);
        if (levelHasEnded) {
            if (canReboot)
                nextLevel();
            else
                return;
        }
		if (canGenerate) {
            int[] doors = new int[4];
            int numDoors = 0;
            for (int i = 0; i < 4; i++) {
                if (doorsUsed[i])
                    doors[numDoors++] = i;
            }
            int way = (int)Random.Range(0, numDoors);
            for (int i = 0; i < numDoors; i++) {
                if (i != way) {
                    movePos(doors[i]);
                    createRoom(Map, 1, (doors[i] + 2) % 4, false);  //-> (porta + 2) % 4 = porta oposta 
                    movePos((doors[i] + 2) % 4);        //volta a posicao
                }
            }
            movePos(doors[way]);
            int maxDir = (int)Mathf.Min(maxDirections(), numberOfRooms - total);

            if (maxDir > 0)
                createRoom(Map, (int)Random.Range(2, maxDir + 2), (doors[way] + 2) % 4, true);
            else {
                //enabled = false;    //nao eh mais possivel gerar direcoes
                createRoom(Boss, 1, (doors[way] + 2) % 4, true);
                levelHasEnded = true;
            }

            canGenerate = false;
            //debug_M();
        }
	}

    private void createRoom(GameObject creation, int num, int origin, bool mainRoom) {
        M[MPosx, MPosy] = true;
        GameObject clone;
        clone = GameObject.Instantiate(creation, new Vector3(x, y, z), transform.rotation);
        clone.name = "Map:-> " + creation.name + " " + total.ToString();
        total++;
        for (int i = 0; i < 4; i++)
            doorsUsed[i] = false;

        if (clone.transform.childCount == (int)Children.Teleporter + 1)
            clone.transform.GetChild((int)Children.Teleporter).gameObject.SetActive(false);

        for (int i = 0; i < clone.transform.GetChild((int)Children.Spawners).childCount; i++)
            if(!clone.name.Contains("boss"))
                clone.transform.GetChild((int)Children.Spawners).GetChild(i).gameObject.SetActive((int)Random.Range(0, 2) == 0);
        for (int i = 0; i < clone.transform.GetChild((int)Children.Objects).childCount; i++)
            clone.transform.GetChild((int)Children.Objects).GetChild(i).gameObject.SetActive((int)Random.Range(0, 2) == 0);

        if (mainRoom)//-------------------------------------------------------------------
            centerOfMainRoom = clone.GetComponent<Renderer>().bounds.center;

        if (origin != -1) {
            //clone.SetActive(false);
            clone.GetComponent<SpriteRenderer>().enabled = false;
            clone.GetComponent<OpenDoor>().enabled = false;
            for (int i = 0; i < 4; i++)
                clone.transform.GetChild(i).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.Spawners).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.Objects).gameObject.SetActive(false);
            //if (creation.name.Contains("Boss"))
            //    clone.transform.GetChild((int)Children.Teleporter).gameObject.SetActive(false);
        }

        if (num == 4) {
            for (int i = 0; i < 4; i++)
                clone.transform.GetChild((int)Children.collidersMiddle).GetChild(i).gameObject.SetActive(false);
            if (origin != -1)
                clone.transform.GetChild((int)Children.doorsClosed).GetChild(origin).gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
                if (i != origin)
                    doorsUsed[i] = true;
        } else if (num == 3) {
            int toBeRemoved = (int)Random.Range(0, 4);
            if (origin != -1) {

                if (!checkDir((int)Directions.north) && (int)Directions.north != origin)
                    toBeRemoved = (int)Directions.north;
                else if (!checkDir((int)Directions.east) && (int)Directions.east != origin)
                    toBeRemoved = (int)Directions.east;
                else if (!checkDir((int)Directions.south) && (int)Directions.south != origin)
                    toBeRemoved = (int)Directions.south;
                else if (!checkDir((int)Directions.west) && (int)Directions.west != origin)
                    toBeRemoved = (int)Directions.west;
                else if (toBeRemoved == origin)
                    toBeRemoved = (toBeRemoved + 1) % 4;

                clone.transform.GetChild((int)Children.doorsClosed).GetChild(origin).gameObject.SetActive(false);
            }
            clone.transform.GetChild((int)Children.doors).GetChild(toBeRemoved).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.collidersMiddleTrigger).GetChild(toBeRemoved).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.doorsClosed).GetChild(toBeRemoved).gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
                if (i != toBeRemoved) {
                    clone.transform.GetChild((int)Children.collidersMiddle).GetChild(i).gameObject.SetActive(false);
                    if (i != origin)
                        doorsUsed[i] = true;
                }
        } else if (num == 2) {
            int toBeRemoved1 = (int)Random.Range(0, 4);
            int toBeRemoved2 = (int)Random.Range(0, 4);
            if (origin != -1) {

                if (!checkDir((int)Directions.north) && (int)Directions.north != origin)
                    toBeRemoved1 = (int)Directions.north;
                else if (!checkDir((int)Directions.east) && (int)Directions.east != origin)
                    toBeRemoved1 = (int)Directions.east;
                else if (!checkDir((int)Directions.south) && (int)Directions.south != origin)
                    toBeRemoved1 = (int)Directions.south;
                else if (!checkDir((int)Directions.west) && (int)Directions.west != origin)
                    toBeRemoved1 = (int)Directions.west;
                else if (toBeRemoved1 == origin)
                    toBeRemoved1 = (toBeRemoved1 + 1) % 4;

                if (!checkDir((int)Directions.north) && (int)Directions.north != origin && (int)Directions.north != toBeRemoved1)
                    toBeRemoved2 = (int)Directions.north;
                else if (!checkDir((int)Directions.east) && (int)Directions.east != origin && (int)Directions.east != toBeRemoved1)
                    toBeRemoved2 = (int)Directions.east;
                else if (!checkDir((int)Directions.south) && (int)Directions.south != origin && (int)Directions.south != toBeRemoved1)
                    toBeRemoved2 = (int)Directions.south;
                else if (!checkDir((int)Directions.west) && (int)Directions.west != origin && (int)Directions.west != toBeRemoved1)
                    toBeRemoved2 = (int)Directions.west;
                else if (toBeRemoved2 == origin || toBeRemoved2 == toBeRemoved1)
                    while (toBeRemoved2 == origin || toBeRemoved2 == toBeRemoved1)
                        toBeRemoved2 = (int)Random.Range(0, 4);

                clone.transform.GetChild((int)Children.doorsClosed).GetChild(origin).gameObject.SetActive(false);
            }
            clone.transform.GetChild((int)Children.doors).GetChild(toBeRemoved1).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.collidersMiddleTrigger).GetChild(toBeRemoved1).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.doorsClosed).GetChild(toBeRemoved1).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.doors).GetChild(toBeRemoved2).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.collidersMiddleTrigger).GetChild(toBeRemoved2).gameObject.SetActive(false);
            clone.transform.GetChild((int)Children.doorsClosed).GetChild(toBeRemoved2).gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
                if (i != toBeRemoved1 && i != toBeRemoved2) {
                    clone.transform.GetChild((int)Children.collidersMiddle).GetChild(i).gameObject.SetActive(false);
                    if (i != origin)
                        doorsUsed[i] = true;
                }
        } else if (num == 1) {
            bool firstRoom = false;
            if (origin == -1) {
                origin = (int)Random.Range(0, 4);
                firstRoom = true;
                doorsUsed[origin] = true;
            }
            clone.transform.GetChild((int)Children.collidersMiddle).GetChild(origin).gameObject.SetActive(false);
            if (!firstRoom)
                clone.transform.GetChild((int)Children.doorsClosed).GetChild(origin).gameObject.SetActive(false);
            for (int i = 0; i < 4; i++) {
                if (i != origin) {
                    clone.transform.GetChild((int)Children.doors).GetChild(i).gameObject.SetActive(false);
                    clone.transform.GetChild((int)Children.collidersMiddleTrigger).GetChild(i).gameObject.SetActive(false);
                    clone.transform.GetChild((int)Children.doorsClosed).GetChild(i).gameObject.SetActive(false);
                }
            }
        } else
            GameObject.Destroy(clone);
    }

    private bool validPos(int posX, int posY) {
        if (posX < numberOfRooms && posY < numberOfRooms && posX >= 0 && posY >= 0 && !M[posX, posY])
            return true;
        return false;
    }

    private bool checkDir(int doorPosition) {
        if (doorPosition == (int)Directions.north)
            return validPos(MPosx, MPosy - 1);
        if (doorPosition == (int)Directions.east)
            return validPos(MPosx + 1, MPosy);
        if (doorPosition == (int)Directions.south)
            return validPos(MPosx, MPosy + 1);
        if (doorPosition == (int)Directions.west)
            return validPos(MPosx - 1, MPosy);
        return false;
    }

    private void movePos(int newPos) {
        if (newPos == (int)Directions.north) {
            MPosy--;
            y += vertical;
        } else if (newPos == (int)Directions.east) {
            MPosx++;
            x += horizontal;
        } else if (newPos == (int)Directions.south) {
            MPosy++;
            y -= vertical;
        } else if (newPos == (int)Directions.west) {
            MPosx--;
            x -= horizontal;
        }  
    }

    private int maxDirections() {
        int max = 0;
        for (int i = 0; i <= (int)Directions.west; i++)
            if (checkDir(i))
                max++;
        return max;
    }
}
