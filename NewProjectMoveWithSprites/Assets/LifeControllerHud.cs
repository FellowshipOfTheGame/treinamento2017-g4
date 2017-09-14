using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeControllerHud : MonoBehaviour
{
    //public
    public GameObject player;

    public Text weapon1Text;
    public Text weapon2Text;

    //private
    private int maxHealth;
    private Image image;
    private PlayerHealth playerHealth;

    private string playerWeapon1;
    private string playerWeapon2;

    // Use this for initialization
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        image = gameObject.GetComponent<Image>();

        maxHealth = playerHealth.health;
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (float)playerHealth.health / (float)maxHealth;

        if (player.gameObject.transform.childCount > 0)
        {
            playerWeapon1 = GetWeapon1();
            weapon1Text.text = playerWeapon1;
            if (player.gameObject.transform.childCount > 1)
            {
                playerWeapon2 = GetWeapon2();
                weapon2Text.text = playerWeapon2;
            }
            else
            {
                weapon2Text.text = "Nenhum";
            }
        }
        else
        {
            weapon1Text.text = "Nenhum";
            weapon2Text.text = "Nenhum";
        }
    }

    public string GetWeapon1()
    {
        string name = "Nenhum";
        GameObject playerWeap = player.gameObject.transform.GetChild(0).gameObject; //arma do player
        if (playerWeap != null)
        {
            switch (playerWeap.name)
            {
                case "GrapplingHookv3":
                    name = "Grappling Hook";
                    break;
                case "GrenadeLauncher":
                    name = "Lancador de Granada" + GetWeaponColor(playerWeap);
                    break;
                case "Hand":
                    name = "Mao";
                    break;
                case "LaserCannon":
                    name = "Laser" + GetWeaponColor(playerWeap); ;
                    break;
                case "Pistol":
                    name = "Pistola" + GetWeaponColor(playerWeap); ;
                    break;
                case "MachineGun":
                    name = "Metralhadora" + GetWeaponColor(playerWeap); ;
                    break;
                default:
                    name = "Nenhum";
                    break;
            }


            //name = playerWeap.name;
        }

        return name;
    }

    public string GetWeapon2()
    {
        string name = "Nenhum";
        GameObject playerWeap = player.gameObject.transform.GetChild(1).gameObject; //arma do player
        if (playerWeap != null)
        {
            switch (playerWeap.name)
            {
                case "GrapplingHookv3":
                    name = "Grappling Hook";
                    break;
                case "GrenadeLauncher":
                    name = "Lancador de Granada" + GetWeaponColor(playerWeap);
                    break;
                case "Hand":
                    name = "Mao";
                    break;
                case "LaserCannon":
                    name = "Laser" + GetWeaponColor(playerWeap); ;
                    break;
                case "Pistol":
                    name = "Pistola" + GetWeaponColor(playerWeap); ;
                    break;
                case "MachineGun":
                    name = "Metralhadora" + GetWeaponColor(playerWeap); ;
                    break;
                default:
                    name = "Nenhum";
                    break;
            }


            //name = playerWeap.name;
        }

        return name;
    }

    public string GetWeaponColor(GameObject playerWeap)
    {
        string name = "";
        //GameObject playerWeap = player.gameObject.transform.GetChild(0).gameObject; //arma do player
        if (playerWeap != null)
        {
            if (playerWeap.GetComponent<GunControllerColorGray>() != null && playerWeap.GetComponent<GunControllerColorGray>().enabled)
            {
                name = " Cinza";
            }
            else if (playerWeap.GetComponent<GunControllerColorGreen>() != null && playerWeap.GetComponent<GunControllerColorGreen>().enabled)
            {
                name = " Verde";
            }
            else if (playerWeap.GetComponent<GunControllerColorBlue>() != null && playerWeap.GetComponent<GunControllerColorBlue>().enabled)
            {
                name = " Azul";
            }
            else if (playerWeap.GetComponent<GunControllerColorRed>() != null && playerWeap.GetComponent<GunControllerColorRed>().enabled)
            {
                name = " Vermelho";
            }
            else if (playerWeap.GetComponent<GunControllerColorYellow>() != null && playerWeap.GetComponent<GunControllerColorYellow>().enabled)
            {
                name = " Amarelo";
            }
            else
            {
                Debug.Log("No Color Found Hud!");
            }
        }

        return name;
    }
}
