using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //publico
    public GameObject player;

    //privado
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.transform.position; //pegar posicao inicial da camera em relacao ao player
        }
    }

    // Update is called once per frame
    void Update()
    {

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
            transform.position = player.transform.position + new Vector3(0f, 0f, offset.z); //colocar a camera na nova posicao conforme o player se mexe
        }
    }
}
