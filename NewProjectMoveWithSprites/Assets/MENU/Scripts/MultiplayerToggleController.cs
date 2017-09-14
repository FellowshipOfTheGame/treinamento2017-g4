using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerToggleController : MonoBehaviour
{

    //Toggle game objects
    public Toggle multiplayer;
    public GameObject toggleGroupMultiplayer;

    //Check active Toggle
    public void ActiveToggle()
    {
        if (multiplayer.isOn)
        { //habilitar multiplayer
            toggleGroupMultiplayer.GetComponent<CanvasGroup>().interactable = false;
            PlayerPrefs.SetInt("isPlayerP1", 1);
            PlayerPrefs.SetInt("isPlayerP2", 1);
        }
        else
        {
            toggleGroupMultiplayer.GetComponent<CanvasGroup>().interactable = true;
            PlayerPrefs.SetInt("isPlayerP1", 1); //habilitar jogador + IA
            PlayerPrefs.SetInt("isPlayerP2", 0);
        }
    }
}
