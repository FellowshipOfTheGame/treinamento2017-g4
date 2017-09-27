using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupMultiplayerController : MonoBehaviour
{

    //Toggle game objects
    public Toggle P1, P2;

    //Check active Toggle
    public void ActiveToggle()
    {
        if (P1.isOn) //jogar na direita
        {
            PlayerPrefs.SetInt("isPlayerP1", 1);
            PlayerPrefs.SetInt("isPlayerP2", 0);
        }
        else if (P2.isOn) //jogar na esquerda
        {
            PlayerPrefs.SetInt("isPlayerP1", 0);
            PlayerPrefs.SetInt("isPlayerP2", 1);
        }
    }
}
