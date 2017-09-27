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

        weapon1Text.text = "Nenhum";
        weapon2Text.text = "Nenhum";
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (float)playerHealth.health / (float)maxHealth;

        if (player.gameObject.transform.childCount > 0)
        {
            playerWeapon1 = GetWeapon1();
            weapon1Text.text = playerWeapon1;
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
                    //name = "Grappling Hook";
                    if (player.gameObject.transform.childCount > 1)
                    {
                        name = GetWeapon2();
                        //playerWeapon2 = GetWeapon2();
                        //weapon2Text.text = playerWeapon2;
                    }
                    else
                    {
                        weapon2Text.text = "Nenhum";
                    }

                    break;
                case "GrenadeLauncher":
                    name = "Granada"; //Lancador de Granada
                    GetWeaponColor(playerWeap);
                    break;
                case "Hand":
                    name = "Mao";
                    break;
                case "LaserCannon":
                    name = "Laser";
                    GetWeaponColor(playerWeap);
                    break;
                case "Pistol":
                    name = "Pistola";
                    GetWeaponColor(playerWeap);
                    break;
                case "MachineGun":
                    name = "Metralhadora";
                    GetWeaponColor(playerWeap);
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
                case "GrenadeLauncher":
                    name = "Granada"; //Lancador de Granada
                    break;
                case "Hand":
                    name = "Mao";
                    break;
                case "LaserCannon":
                    name = "Laser";
                    break;
                case "Pistol":
                    name = "Pistola";
                    break;
                case "MachineGun":
                    name = "Metralhadora";
                    break;
                default:
                    name = "Nenhum";
                    break;
            }

            GetWeaponColor(playerWeap);
            //name = playerWeap.name;
        }

        return name;
    }

    public string GetWeaponColor(GameObject playerWeap)
    {
        string name = "";
        Debug.Log("Arma da cor: " + playerWeap.name);
        GameObject playerWeapChild = playerWeap.transform.GetChild(0).gameObject; //arma do player / Gun com scripts
        if (playerWeapChild != null)
        {
            if (playerWeapChild.GetComponent<GunControllerColorGray>() != null && playerWeapChild.GetComponent<GunControllerColorGray>().enabled)
            {
                name = " Cinza";
            }
            else if (playerWeapChild.GetComponent<GunControllerColorGreen>() != null && playerWeapChild.GetComponent<GunControllerColorGreen>().enabled)
            {
                name = " Verde";
            }
            else if (playerWeapChild.GetComponent<GunControllerColorBlue>() != null && playerWeapChild.GetComponent<GunControllerColorBlue>().enabled)
            {
                name = " Azul";
            }
            else if (playerWeapChild.GetComponent<GunControllerColorRed>() != null && playerWeapChild.GetComponent<GunControllerColorRed>().enabled)
            {
                name = " Vermelho";
            }
            else if (playerWeapChild.GetComponent<GunControllerColorYellow>() != null && playerWeapChild.GetComponent<GunControllerColorYellow>().enabled)
            {
                name = " Amarelo";
            }
            else
            {
                name = "Nenhum";
                Debug.Log("No Color Found Hud!");
            }
        }

        weapon2Text.text = name;
        return name;
    }
}
