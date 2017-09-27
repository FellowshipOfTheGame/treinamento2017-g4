using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject PauseMenu = transform.Find("PauseMenu").gameObject;
        GameObject OptionsMenu = transform.Find("OptionsMenu").gameObject;
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (PauseMenu.GetComponent<CanvasGroup>().alpha == 1)
            {
                PauseMenu.GetComponent<Animator>().SetTrigger("Close");
            }
            else if (PauseMenu.GetComponent<CanvasGroup>().alpha == 0)
            {
                if (OptionsMenu.GetComponent<CanvasGroup>().alpha != 0) OptionsMenu.GetComponent<Animator>().SetTrigger("Close");
                PauseMenu.GetComponent<Animator>().SetTrigger("Open");
            }
        }
    }
}
