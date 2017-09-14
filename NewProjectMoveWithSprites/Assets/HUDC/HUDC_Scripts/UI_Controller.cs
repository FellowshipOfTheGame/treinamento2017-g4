using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Controller : MonoBehaviour {

    public int maxLife;
    public Sprite Meteor;
    public Sprite Sword;
    public Image Heart;
    public Image Weapon1;
    public Text Weapon1Name;
    private int Weapon1Type = 1;
    public Image Weapon2;
    public Text Weapon2Name;
    private int Weapon2Type = 2;
    public string credits_scene;

    public void setLife(int amount)
    {
        /*if (amount <= 4 && amount >= 0)
        {
            Heart.fillAmount = amount / maxLife;
        }*/
        Heart.fillAmount += (float)amount / maxLife;
        if (Heart.fillAmount > 1) Heart.fillAmount = 1;
        else if (Heart.fillAmount < 0) Heart.fillAmount = 0;
    }

    public void setWeapon(int number, int type, string name)
    {
        if (number == 1)
        {
            if (type == 1)
            {
                Weapon1.sprite = Meteor;
                Weapon1Type = 1;
            }
            else if (type == 2)
            {
                Weapon1.sprite = Sword;
                Weapon1Type = 2;
            }
            Weapon1Name.text = name;
        }
        else if (number == 2)
        {
            if (type == 1)
            {
                Weapon2.sprite = Meteor;
                Weapon2Type = 1;
            }
            else if (type == 2)
            {
                Weapon2.sprite = Sword;
                Weapon2Type = 2;
            }
            Weapon2Name.text = name;
        }
    }

    public void switchWeapon()
    {
        string name_aux;
        int type_aux;

        name_aux = Weapon1Name.text;
        type_aux = Weapon1Type;
        Weapon1Name.text = Weapon2Name.text;
        Weapon1Type = Weapon2Type;
        if (Weapon1Type == 1) Weapon1.sprite = Meteor;
        else if (Weapon1Type == 2) Weapon1.sprite = Sword;
        Weapon2Name.text = name_aux;
        Weapon2Type = type_aux;
        if (Weapon2Type == 1) Weapon2.sprite = Meteor;
        else if (Weapon2Type == 2) Weapon2.sprite = Sword;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.UpArrow))
        //{
        //    setLife(1);
        //}
        //if (Input.GetKeyUp(KeyCode.DownArrow))
        //{
        //    setLife(-1);
        //}
        //if (Input.GetKeyUp(KeyCode.Z))
        //{
        //    switchWeapon();
        //}
        //if (Input.GetKeyUp(KeyCode.Return))
        //{
        //    Application.LoadLevel(credits_scene);
        //}
    }

    /*// Use this for initialization
    void Start () {
		
	}*/
}
