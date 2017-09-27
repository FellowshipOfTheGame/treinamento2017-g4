using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    //public (iniciais p/ lembrar)
    public Color corLaser; //cor do laser   (Color.green)
    public int maxDistLaser; //distancia maxima do laser (50)
    public float larguraInicial; // (0.02f)
    public float larguraFinal; // (0.1f)

    //public GameObject player;
    //public GameObject gun;

    //private
    private GameObject lightCollider; //objeto na posicao onde a luz ira colidir
    private Vector3 posLuz; //direcao pra onde o objeto da posicao final ira
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lightCollider = new GameObject("lightCollider"); //criar objeto vazio com o nome "lightCollider"
        Light light = lightCollider.AddComponent<Light>(); //adicionar componente luz ao objeto

        light.intensity = 8; //alterar valores do componenente
        light.bounceIntensity = 8; //8 = valor maximo
        light.range = larguraFinal * 2;
        light.color = corLaser;

        posLuz = new Vector3(larguraFinal, 0, 0);

        lineRenderer = gameObject.AddComponent<LineRenderer>(); //criar uma linha no objeto do script (cria linha entre duas posicoes)
        lineRenderer.material = new Material(Shader.Find("Particles/Additive")); //adicionar um material a linha parecido com um "laser"
        lineRenderer.startColor = corLaser; // cor inicial e cor final //setColors(inicial, final) obsoleto
        lineRenderer.endColor = corLaser;
        lineRenderer.startWidth = larguraInicial; // largura inicial e largura final //setWidth(inicial, final) obsoleto
        lineRenderer.endWidth = larguraFinal;
        lineRenderer.positionCount = 2; //quantia de pontos da linha //SetVertexCount(2) e numPositions obsoleto

        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = true;

        //SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log(SortingLayer.NameToID("Player"));
        lineRenderer.sortingLayerID = SortingLayer.NameToID("Player"); // spriteRenderer.sortingLayerID;
        lineRenderer.sortingOrder = 0; // spriteRenderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pontoFinalLaser = transform.position + transform.right * maxDistLaser; //pra frente vermelho ate distancia maxima

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Ray2D ray = new Ray2D(transform.position, transform.right); //criar raio da posicao na direcao da seta vermelha (do mundo) 
        pontoColisao = Physics2D.Raycast(ray.origin, transform.right, maxDistLaser, 9); //com ponto de colisao e distancia e ignorar layer 9 do character
        //new ContactFilter2D();

        Debug.Log(pontoColisao.collider);
        if (pontoColisao.collider) //verificar hit com algo
        //&& pontoColisao.collider != player.GetComponent<Collider2D>()
        //&& pontoColisao.collider != gun.GetComponent<Collider2D>()) 
        {
            lineRenderer.SetPosition(0, transform.position); //linha entre origem e request da colisao/ponto onde aconteceu a colisao
            lineRenderer.SetPosition(1, pontoColisao.point); //ponto onde o raio colidiu

            lightCollider.transform.position = pontoColisao.point - (Vector2)posLuz; //alterar fim do laser
        }
        else //se nao ocorreu colisao
        {
            lineRenderer.SetPosition(0, transform.position); //linha entre origem e ponto final do laser
            lineRenderer.SetPosition(1, pontoFinalLaser);

            lightCollider.transform.position = pontoFinalLaser; //alterar fim do laser
        }

    }
}
