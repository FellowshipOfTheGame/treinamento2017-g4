using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColScript : MonoBehaviour
{
    //public
    public bool canMoveLeft; //pode ir pra esquerda?
    public bool canMoveRight; //pode ir pra direita?
    public bool canMoveUp; //pode ir pra cima?
    public bool canMoveDown; //pode ir pra baixo?

    //private
    private float cameraHeight; //altura da camera
    private float cameraWidth; //largura da camera

    private int collisionMask; //colisoes possiveis do raycast

    private GameObject parentCamera; //camera que tem o collider

    // Use this for initialization
    void Start()
    {
        parentCamera = gameObject.transform.parent.gameObject; //pegar camera, parent

        cameraHeight = 2 * Camera.main.orthographicSize; //largura da camera
        cameraWidth = cameraHeight * Camera.main.aspect; //altura da camera

        //Debug.Log("Camera Height: " + cameraHeight + "  Camera Width: " + cameraWidth);

        gameObject.transform.localScale = new Vector2(cameraWidth, cameraHeight); //acertar tamanho do collider da camera

        canMoveLeft = true; //setar flags
        canMoveRight = true;
        canMoveUp = true;
        canMoveDown = true;

        collisionMask = 1 << LayerMask.NameToLayer("CameraWal"); //soh pode pegar com a CameraWal
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void VerifyIfOutOfCamera(GameObject player)
    {
        if (player.transform.position == Vector3.zero)
        { //ajustar camera qdo voltar pro comeco
            parentCamera.transform.position = player.transform.position;
            canMoveLeft = true;
            canMoveRight = true;
            canMoveUp = true;
            canMoveDown = true;
        }

        float aux = 0; // distancia auxiliar do player ate a camera em alguma direcao (esq, dir, cima, baixo)
        if (!canMoveLeft)
        {
            aux = Mathf.Abs(parentCamera.transform.position.x - player.transform.position.x); //distancia na vertical
            //Debug.Log("Aux: " + aux + "Width/2: " + (cameraWidth / 2));
            if (aux > (cameraWidth / 2))
            {
                //parentCamera.transform.position -= new Vector3(cameraWidth + 3f, 0f, 0f); //puxar a camera por um tamanho
                player.transform.position -= new Vector3(2.5f, 0f, 0f); //puxar a camera por um tamanho
                parentCamera.transform.position = new Vector3(player.transform.position.x - cameraWidth / 2, player.transform.position.y, parentCamera.transform.position.z); //puxar a camera por um tamanho
                //parentCamera.transform.position -= new Vector3(cameraWidth + 3f, 0f, 0f); //puxar a camera por um tamanho
                canMoveLeft = true;
                canMoveRight = false;
            }
        }
        else if (!canMoveRight)
        {
            aux = Mathf.Abs(parentCamera.transform.position.x - player.transform.position.x); //distancia na vertical
            if (aux > (cameraWidth / 2))
            {
                //parentCamera.transform.position += new Vector3(cameraWidth + 3f, 0f, 0f); //puxar a camera por um tamanho
                player.transform.position += new Vector3(2.5f, 0f, 0f); //puxar a camera por um tamanho
                parentCamera.transform.position = new Vector3(player.transform.position.x + cameraWidth / 2, player.transform.position.y, parentCamera.transform.position.z); //puxar a camera por um tamanho
                canMoveRight = true;
                canMoveLeft = false;
            }
        }

        if (!canMoveUp)
        {
            aux = Mathf.Abs(parentCamera.transform.position.y - player.transform.position.y); //distancia na vertical
            if (aux > (cameraHeight / 2))
            {
                //parentCamera.transform.position += new Vector3(0f, cameraHeight + 3f, 0f); //puxar a camera por um tamanho
                player.transform.position += new Vector3(0f, 2.5f, 0f); //puxar a camera por um tamanho
                parentCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + cameraHeight / 2, parentCamera.transform.position.z); //puxar a camera por um tamanho
                canMoveUp = true;
                canMoveDown = false;
            }
        }
        else if (!canMoveDown)
        {
            aux = Mathf.Abs(parentCamera.transform.position.y - player.transform.position.y); //distancia na vertical
            if (aux > (cameraHeight / 2))
            {
                //parentCamera.transform.position -= new Vector3(0f, cameraHeight + 3f, 0f); //puxar a camera por um tamanho
                player.transform.position -= new Vector3(0f, 2.5f, 0f); //puxar a camera por um tamanho
                parentCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - cameraHeight / 2, parentCamera.transform.position.z); //puxar a camera por um tamanho
                canMoveDown = true;
                canMoveUp = false;
            }
        }
    }

    public void VerifyCameraDistance(GameObject player) //verifica se o player saiu de perto das paredes pra reativar as flags
    {
        VerifyIfOutOfCamera(player);

        Vector3 playerToWallPoint; //ponto direto do player na parede que a camera esta colidindo
        Vector3 cameraToWallPoint; //ponto direto da camera na parede que esta colidindo

        Vector3 cameraPos = new Vector3(parentCamera.transform.position.x, parentCamera.transform.position.y, 0f); //posicao atual da camera
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, 0f); //posicao atual do player

        if (!canMoveLeft) //se nao puder mover pra esquerda
        {
            playerToWallPoint = DoLaser(player, -player.transform.right);
            cameraToWallPoint = DoLaser(parentCamera, -parentCamera.transform.right);
            //Debug.Log("PlayerToWall: " + Vector3.Distance(playerToWallPoint, player.transform.position) + "CameraToWall: " + Vector3.Distance(cameraToWallPoint, parentCamera.transform.position));

            if (Vector3.Distance(playerToWallPoint, playerPos) >= Vector3.Distance(cameraToWallPoint, cameraPos)) //se a distancia do player da parede for maior que a da camera, retomar movimentacao
            {
                //Debug.Log("Camera Return: wLeft");
                canMoveLeft = true;
            }
        }
        else if (!canMoveRight) //se nao puder mover pra direita
        {
            playerToWallPoint = DoLaser(player, player.transform.right);
            cameraToWallPoint = DoLaser(parentCamera, parentCamera.transform.right);

            if (Vector3.Distance(playerToWallPoint, playerPos) >= Vector3.Distance(cameraToWallPoint, cameraPos)) //se a distancia do player da parede for maior que a da camera, retomar movimentacao
            {
                //Debug.Log("Camera Return: wRight");
                canMoveRight = true;
            }
        }

        if (!canMoveUp) //se nao puder mover pra cima
        {
            playerToWallPoint = DoLaser(player, player.transform.up);
            cameraToWallPoint = DoLaser(parentCamera, parentCamera.transform.up);

            if (Vector3.Distance(playerToWallPoint, playerPos) >= Vector3.Distance(cameraToWallPoint, cameraPos)) //se a distancia do player da parede for maior que a da camera, retomar movimentacao
            {
                //Debug.Log("Camera Return: wUp");
                canMoveUp = true;
            }
        }
        else if (!canMoveDown) //se nao puder mover pra baixo
        {
            playerToWallPoint = DoLaser(player, -player.transform.up);
            cameraToWallPoint = DoLaser(parentCamera, -parentCamera.transform.up);

            if (Vector3.Distance(playerToWallPoint, playerPos) >= Vector3.Distance(cameraToWallPoint, cameraPos)) //se a distancia do player da parede for maior que a da camera, retomar movimentacao
            {
                //Debug.Log("Camera Return: wDown");
                canMoveDown = true;
            }
        }

        Debug.Log(canMoveLeft + " : " + canMoveRight + " : " + canMoveUp + " : " + canMoveDown);
    }

    public Vector3 DoLaser(GameObject origin, Vector3 direction) //faz um raycast na direcao da requisitada pra ver se acerta algo
    {
        Vector3 endPoint = Vector3.zero;
        Vector3 startPoint = new Vector3(origin.transform.position.x, origin.transform.position.y, 0f);
        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Ray2D ray = new Ray2D(startPoint, direction); //criar raio da posicao na direcao da seta vermelha (do mundo) 

        pontoColisao = Physics2D.Raycast(ray.origin, direction, 100, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character

        //if (origin.name.Equals("Player")) Debug.DrawLine(ray.origin, pontoColisao.point, Color.white, 100f);
        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //endPoint = pontoColisao.point;
            endPoint = ray.origin + (ray.direction * (Vector3.Distance(pontoColisao.point, startPoint))); //pegar ponto final e arrumar distancia maxima
        }
        else //se nao ocorreu colisao
        {
            endPoint = ray.origin + (ray.direction * (100)); //arrumar distancia maxima
        }

        return endPoint; //retornar ponto de colisao/final
    }


    private void OnTriggerStay2D(Collider2D other)//se estiver colidindo
    {
        //Debug.Log("Collision: " + other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("CameraWal")) //desativar flags caso colidindo/em trigger
        {
            //Debug.Log("Camera Collision: " + other.name);

            if (other.gameObject.name.Contains("Left"))
            {
                canMoveLeft = false;
            }
            else if (other.gameObject.name.Contains("Right"))
            {
                canMoveRight = false;
            }
            else if (other.gameObject.name.Contains("Up"))
            {
                canMoveUp = false;
            }
            else if (other.gameObject.name.Contains("Down"))
            {
                canMoveDown = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ////Debug.Log("Collision: " + other.name);
        //if (other.gameObject.layer == LayerMask.NameToLayer("CameraWal"))
        //{
        //    Debug.Log("Camera Collision Exit: " + other.name);
        //    canMove = true;
        //}
    }
}
