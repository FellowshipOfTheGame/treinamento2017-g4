using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectController : MonoBehaviour {
    //public EventSystem eventsystem;
    //public GameObject obj;
    //private bool selected;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetAxisRaw("Vertical") != 0 && selected == false)
        //{
        //    eventsystem.SetSelectedGameObject(obj);
        //    selected = true;
        //}
    }

    private void OnDisable()
    {
        //selected = false;
    }
}
