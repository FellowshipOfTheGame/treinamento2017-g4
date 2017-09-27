using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityLifeToggleController : MonoBehaviour
{
    //Toggle game objects
    public Toggle infinityLife;

    private void Start()
    {
        PlayerPrefs.SetInt("infinityLife", 1);
        infinityLife.GetComponent<Toggle>().isOn = true;
    }

    //Check active Toggle
    public void ActiveToggle()
    {
        if (infinityLife.isOn)
        { //habilitar multiplayer
            PlayerPrefs.SetInt("infinityLife", 1);
        }
        else
        {
            PlayerPrefs.SetInt("infinityLife", 0); //desabilitar vida infinita
        }
    }
}
