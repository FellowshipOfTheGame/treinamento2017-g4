using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupNoMatchesController : MonoBehaviour {

    //Toggle game objects
    public Toggle n1, n3, n5, n10; //1, 3, 5, 10

    //Check active Toggle
    public void ActiveToggle()
    {
        if (n1.isOn) //1
        {
            PlayerPrefs.SetInt("NumberOfMatches", 1);
        }
        else if (n3.isOn) //3
        {
            PlayerPrefs.SetInt("NumberOfMatches", 3);
        }
        else if (n5.isOn) //5
        {
            PlayerPrefs.SetInt("NumberOfMatches", 5);
        }
        else if (n10.isOn) //10
        {
            PlayerPrefs.SetInt("NumberOfMatches", 10);
        }
    }
}
