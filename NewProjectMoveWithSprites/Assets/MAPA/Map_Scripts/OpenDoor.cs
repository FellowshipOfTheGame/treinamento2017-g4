using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {
    //public bool open;

    public static bool trigger;
    private int maxSpawns;
    private bool canGenerate;
    //private float xDim, yDim;

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

    // Use this for initialization
    void Start () {
        maxSpawns = transform.GetChild((int)Children.Spawners).childCount;
        trigger = false;
        canGenerate = false;
        //xDim = gameObject.GetComponent<Renderer>().bounds.size.x;
        //yDim = gameObject.GetComponent<Renderer>().bounds.size.y;
    }
	
	// Update is called once per frame
	void Update () {
        checkTrigger();
        if (trigger) {
            for (int i = 0; i < 4; i++)
                gameObject.transform.GetChild((int)Children.doorsClosed).GetChild(i).gameObject.SetActive(false);
            trigger = false;
            if (gameObject.transform.childCount == (int)Children.Teleporter + 1)
                gameObject.transform.GetChild((int)Children.Teleporter).gameObject.SetActive(true);
            //enabled = false;
            canGenerate = true;
        }
        insideCheck();
    }

    private void checkTrigger() {
        int counter;
        for (counter = 0; counter < maxSpawns && !gameObject.transform.GetChild((int)Children.Spawners).GetChild(counter).gameObject.activeSelf; counter++) ;
        if (counter == maxSpawns)
            trigger = true;
    }

    private void insideCheck() {
        if (canGenerate && MapUpdate.insideMainRoom()) {
            enabled = false;
            GenWorldPt.canGenerate = true;
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && canGenerate) {
            GenWorldPt.canGenerate = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        canGenerate = false;
        gameObject.GetComponent<Renderer>().bounds.size.x;
    }*/
}
