using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookController : MonoBehaviour
{
    //public
    public float step; //distancia que vai mudar por update do grappling hook

    public float maxRange; //distancia maxima do grappling hook

    //private
    private GameObject player; //usuario da arma
    private DistanceJoint2D distanceJoint; //pegar distance joint do player

    //private bool canAttach; //verifica se pode "grudar" no objeto para ser "grapplingHook"
    //private bool isAttached; //verifica se o "grapplingHook" ja esta "grudado" em algum objeto

    private string target; //quem podera ser acertado
    private int collisionMask; //mascara para saber com quem a "bala" colide / raycast

    // Use this for initialization
    void Start()
    {
        player = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)
        distanceJoint = player.GetComponent<DistanceJoint2D>(); //pegar distance joint do player
        distanceJoint.enabled = false;

        SetTargetAndCollisionMask();

        //isAttached = false;
    }

    public void SetTargetAndCollisionMask()//setar o source e a collisionMask
    {
        //source = "Character"
        target = "Enemy"; //caso player --> target inimigo
        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)

        //collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("Enemy"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)    
    }

    // Update is called once per frame
    void Update()
    {
        if (distanceJoint.distance > 0.5f)
        { //"puxar"
            distanceJoint.distance -= step;
        }
        else
        {
            //distanceJoint.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Q)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
        {
            TryLaunchGrapplingHook();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            //isAttached = false;
            distanceJoint.enabled = false; //desativa distance joint
        }
    }

    public void TryLaunchGrapplingHook() //tenta fixar o grappling hook para puxar
    {
        //isAttached = true;
        Vector3 actualPlayerPosition = gameObject.transform.position; //pega posicao do player
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pegar "target" transform
        Vector3[] pontoColisao = DoLaser(targetPosition); //tentar pegar no objeto proximo

        if (pontoColisao[1].x == 0)
        { //nao colidiu em nada
            //Vector3 finalPoint =
        }
        else if (pontoColisao[1].x == -1)
        { //colidiu em um inimigo
            //Vector3 finalPoint = pontoColisao[0];
        }
        else if (pontoColisao[1].x == -2)
        { //colidiu em uma parede
            distanceJoint.connectedAnchor = pontoColisao[0];
            distanceJoint.distance = Vector3.Distance(pontoColisao[0], actualPlayerPosition);

            distanceJoint.enabled = true; //ativa distance joint
        }

        //distanceJoint.anchor = playerPosition;
        //distanceJoint.connectedAnchor = actualHandPosition;
    }

    public Vector3[] DoLaser(Vector3 targetPosition) //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pegar "target" transform

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Vector3 direction = (targetPosition - gameObject.transform.position).normalized; //direcao para verificar
        Ray2D ray = new Ray2D(gameObject.transform.position, direction); //criar raio da posicao na direcao do mouse 

        pontoColisao = Physics2D.Raycast(ray.origin, direction, maxRange, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character
        Debug.DrawLine(ray.origin, pontoColisao.point, new Color(10f, 10f, 10f), 100f);

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //if (pontoColisao.transform.gameObject == target) return true;
            if (pontoColisao.transform.gameObject.layer == LayerMask.NameToLayer(target)) //se estiver colidindo com um inimigo
            {
                return new Vector3[] { pontoColisao.point, new Vector3(-1, -1) }; //retorna -1 + ponto de colisao
            }
            else if ((collisionMask & (1 << pontoColisao.transform.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
            {
                return new Vector3[] { pontoColisao.point, new Vector3(-2, -2) }; //retorna -2 + ponto de colisao
            }
        }
        //else //se nao ocorreu colisao
        //{
        //}

        return new Vector3[] { Vector3.zero, Vector3.zero }; //nao acertou nada
    }
}
