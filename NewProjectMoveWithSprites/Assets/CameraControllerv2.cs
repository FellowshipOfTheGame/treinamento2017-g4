using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerv2 : MonoBehaviour
{
    //publico
    public GameObject player;

    //privado
    private Vector3 offset;
    private CameraColScript cameraCol; //collider da camera

    // Use this for initialization
    void Start()
    {
        //Debug.Log("PlayerToWall: " + Vector3.Distance(GameObject.Find("wLeft").transform.position, player.transform.position) + "CameraToWall: " + Vector3.Distance(GameObject.Find("wLeft").transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f)));

        cameraCol = gameObject.transform.GetChild(0).gameObject.GetComponent<CameraColScript>(); //pegar filho, collider da camera
        if (player != null)
        {
            offset = transform.position - player.transform.position; //pegar posicao inicial da camera em relacao ao player
        }
    }

    // Update is called once per frame
    void Update()
    {
        //cameraCol.VerifyCameraDistance(player);
    }

    private void FixedUpdate()
    {
        //teste pq o personagem pode estar sofrendo mais alteracoes que a camera e aparenta dar um "bug" na img
        //transform.position = player.transform.position + offset; //colocar a camera na nova posicao conforme o player se mexe
    }

    //pos movimentacao/cena
    private void LateUpdate()
    {
        if (player != null)
        {
            cameraCol.VerifyCameraDistance(player); //reativar ou nao flags

            Vector3 playerPos = player.transform.position; //posicao do player

            if (!cameraCol.canMoveLeft || !cameraCol.canMoveRight) //se nao puder se mexer na horizontal
            {
                //Debug.Log("PlayerToWall: " + Vector3.Distance(GameObject.Find("wLeft").transform.position, player.transform.position) + "CameraToWall: " + Vector3.Distance(GameObject.Find("wLeft").transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f)));

                playerPos = new Vector3(transform.position.x, player.transform.position.y, 0f);
            }

            if (!cameraCol.canMoveUp || !cameraCol.canMoveDown) //se nao puder se mexer na vertical
            {
                playerPos = new Vector3(player.transform.position.x, transform.position.y, 0f);
            }

            if ((!cameraCol.canMoveLeft || !cameraCol.canMoveRight) && (!cameraCol.canMoveUp || !cameraCol.canMoveDown)) //se nao puder se mexer nem na vertical nem na horizontal
            {
                playerPos = new Vector3(transform.position.x, transform.position.y, 0f);
            }

            //transform.position = player.transform.position + new Vector3(0f, 0f, offset.z); //colocar a camera na nova posicao conforme o player se mexe
            transform.position = playerPos + new Vector3(0f, 0f, offset.z + player.transform.position.z); //colocar a camera na nova posicao conforme o player se mexe
        }
    }
}
