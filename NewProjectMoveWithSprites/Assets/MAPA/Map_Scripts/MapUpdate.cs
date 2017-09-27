using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUpdate : MonoBehaviour {

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

    //private float vertical, horizontal;
    private static GameObject player;
    private bool isColliding;

    // Use this for initialization
    void Start() {
        //vertical = GenWorldPt.vertical;
        //horizontal = GenWorldPt.horizontal;
        player = /*this.gameObject.*/GameObject.Find("Player");
        isColliding = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
            GenWorldPt.canGenerate = true;
        isColliding = false;
    }

    //Funcao de teste, deletar depois
    /*private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Contains("Enemy"))
            //collision.gameObject.SetActive(false);
            GameObject.Destroy(collision.gameObject);
    }*/

    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("COLIDE: " + collision.gameObject.name + " " + collision.gameObject.activeSelf);
        //print(collision.name);

        if (isColliding) return;
        isColliding = true;

        string name = collision.name;
        if (name.Contains("collideMiddleTrigger")) {
            GameObject gm = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject;

            //for (int i = 0; i < 4; i++) =============================================================
                //gm.transform.GetChild(i).gameObject.SetActive(!gm.transform.GetChild(i).gameObject.activeSelf);
            gm.transform.GetChild((int)Children.Spawners).gameObject.SetActive(!gm.transform.GetChild((int)Children.Spawners).gameObject.activeSelf);
            //gm.transform.GetChild((int)Children.Objects).gameObject.SetActive(!gm.transform.GetChild((int)Children.Objects).gameObject.activeSelf);
            //gm.GetComponent<SpriteRenderer>().enabled = !gm.GetComponent<SpriteRenderer>().enabled;
            gm.GetComponent<OpenDoor>().enabled = gm.GetComponent<SpriteRenderer>().enabled;
        } else if (name.Contains("Teleporter")) {
            GenWorldPt.canReboot = true;
            player.transform.position = Vector3.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        string name = collision.name;
        if (name.Contains("collideMiddleTrigger")) {
            GameObject gm = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject;

            if (gm.GetComponent<OpenDoor>().enabled)
                closeEntrance(gm, name);
        }
    }

    private void closeEntrance(GameObject gameObject, string nameCollider) {
        if (gameObject.GetComponent<SpriteRenderer>().enabled) {
            if (nameCollider == "collideMiddleTriggerUp")
                gameObject.transform.GetChild((int)Children.doorsClosed).GetChild((int)Directions.north).gameObject.SetActive(true);
            else if (nameCollider == "collideMiddleTriggerRight")
                gameObject.transform.GetChild((int)Children.doorsClosed).GetChild((int)Directions.east).gameObject.SetActive(true);
            else if (nameCollider == "collideMiddleTriggerDown")
                gameObject.transform.GetChild((int)Children.doorsClosed).GetChild((int)Directions.south).gameObject.SetActive(true);
            else if (nameCollider == "collideMiddleTriggerLeft")
                gameObject.transform.GetChild((int)Children.doorsClosed).GetChild((int)Directions.west).gameObject.SetActive(true);
        }
    }

    public static bool insideMainRoom() {
        float xMain = GenWorldPt.centerOfMainRoom.x;
        float yMain = GenWorldPt.centerOfMainRoom.y;
        if (player.GetComponent<Renderer>().bounds.center.x < xMain + GenWorldPt.horizontal &&
            player.GetComponent<Renderer>().bounds.center.x > xMain - GenWorldPt.horizontal &&
            player.GetComponent<Renderer>().bounds.center.y < yMain + GenWorldPt.vertical &&
            player.GetComponent<Renderer>().bounds.center.y > yMain - GenWorldPt.vertical)
            return true;
        return false;
    }
}
