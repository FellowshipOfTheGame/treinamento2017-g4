using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialValues : MonoBehaviour
{
    private bool isPlayerP1;
    private bool isPlayerP2;
    private bool isMultiplayer;

    private int numberOfMatches;

    private float valueMusicVolumeMenu;

    //Toggle game objects and slider
    public Toggle multiplayer;
    public GameObject toggleGroupMultiplayer;
    public Toggle P1, P2;
    public Toggle n1, n3, n5, n10; //1, 3, 5, 10
    public Slider musicVolumeMenu;

    private void Awake()
    {
        //Player 1
        if (PlayerPrefs.HasKey("isPlayerP1")) //pegar dos prefs iniciais
        {
            isPlayerP1 = PlayerPrefs.GetInt("isPlayerP1") == 1 ? true : false; //Se 1 true se zero false
        }
        else
        {
            Debug.Log("Non Existing Key: isPlayerP1");
            PlayerPrefs.SetInt("isPlayerP1", 1);

            isPlayerP1 = true;
        }

        //Player 2
        if (PlayerPrefs.HasKey("isPlayerP2")) //pegar dos prefs iniciais
        {
            isPlayerP2 = PlayerPrefs.GetInt("isPlayerP2") == 1 ? true : false; //Se 1 true se zero false
        }
        else
        {
            Debug.Log("Non Existing Key: isPlayerP2");
            PlayerPrefs.SetInt("isPlayerP2", 0);

            isPlayerP2 = false;
        }

        //Number of Matches
        if (PlayerPrefs.HasKey("NumberOfMatches")) //pegar dos prefs iniciais
        {
            numberOfMatches = PlayerPrefs.GetInt("NumberOfMatches");
        }
        else
        {
            Debug.Log("Non Existing Key: NumberOfMatches");
            PlayerPrefs.SetInt("NumberOfMatches", 3);

            numberOfMatches = 3; //default
        }

        //Multiplayer?
        if (PlayerPrefs.HasKey("isPlayerP1") && PlayerPrefs.HasKey("isPlayerP2")) //pegar dos prefs iniciais
        {
            bool isPlayerP1 = PlayerPrefs.GetInt("isPlayerP1") == 1 ? true : false;
            bool isPlayerP2 = PlayerPrefs.GetInt("isPlayerP2") == 1 ? true : false;

            if (!isPlayerP1 || !isPlayerP2) //se um dos 2 nao for player
            {
                isMultiplayer = false;
            }
            else
            {
                isMultiplayer = true;
            }
        }
        else
        {
            Debug.Log("Non Existing Key: isPlayerP1 or isPlayerP2");
            PlayerPrefs.SetInt("isPlayerP1", 1);
            PlayerPrefs.SetInt("isPlayerP2", 0);

            isMultiplayer = false; //default
        }

        //MusicVolumeMenu
        if (PlayerPrefs.HasKey("MusicVolumeMenu")) //pegar dos prefs iniciais
        {
            valueMusicVolumeMenu = PlayerPrefs.GetFloat("MusicVolumeMenu"); //Pegar volume anterior
        }
        else
        {
            Debug.Log("Non Existing Key: MusicVolumeMenu");
            PlayerPrefs.SetFloat("MusicVolumeMenu", 0.5f);

            valueMusicVolumeMenu = 0.5f;
        }

        //presetar as settings (para possivelmente carregar ultimos dados)
        if (isMultiplayer)
        {
            multiplayer.isOn = true;
            toggleGroupMultiplayer.GetComponent<CanvasGroup>().interactable = false;

            P1.isOn = true;
            P2.isOn = false;
        }
        else
        {
            multiplayer.isOn = false;
            toggleGroupMultiplayer.GetComponent<CanvasGroup>().interactable = true;

            if (isPlayerP1)
            {
                P1.isOn = true;
                P2.isOn = false;
            }
            else if (isPlayerP2)
            {
                P1.isOn = false;
                P2.isOn = true;
            }
        }

        switch (numberOfMatches)
        {
            case 1:
                n1.isOn = true;
                n3.isOn = false;
                n5.isOn = false;
                n10.isOn = false;
                break;
            case 3:
                n1.isOn = false;
                n3.isOn = true;
                n5.isOn = false;
                n10.isOn = false;
                break;
            case 5:
                n1.isOn = false;
                n3.isOn = false;
                n5.isOn = true;
                n10.isOn = false;
                break;
            case 10:
                n1.isOn = false;
                n3.isOn = false;
                n5.isOn = false;
                n10.isOn = true;
                break;
            default:
                n1.isOn = false;
                n3.isOn = true;
                n5.isOn = false;
                n10.isOn = false;
                PlayerPrefs.SetInt("NumberOfMatches", 3); //default
                break;
        }

        musicVolumeMenu.value = valueMusicVolumeMenu; //valor do slider na musica
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
