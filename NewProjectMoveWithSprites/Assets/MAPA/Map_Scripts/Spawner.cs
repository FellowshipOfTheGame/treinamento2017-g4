using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private GameObject[] entities;
    private GameObject clone;
    private int size;

	// Use this for initialization
	void Start () {
        size = entities.Length;
        if (size != 0) {
            clone = GameObject.Instantiate(entities[(int)Random.Range(0, size)], transform.position, transform.rotation);
            clone.name = "Enemy";
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (clone == null || !clone.activeSelf) {
            this.gameObject.SetActive(false);
            enabled = false;
        }
	}
}
