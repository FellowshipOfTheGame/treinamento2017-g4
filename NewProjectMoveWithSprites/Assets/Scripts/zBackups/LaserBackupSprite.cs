using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class /*LaserController2*/LaserBackupSprite : MonoBehaviour
{
    //public (iniciais p/ lembrar)
    public Color corLaser; //cor do laser   (Color.green)
    public int maxDistLaser; //distancia maxima do laser (50)
    public float larguraInicial; // (0.02f)
    public float larguraFinal; // (0.1f)

    //public GameObject player;
    //public GameObject gun;

    public Transform bulletSpawnTransform;

    //private
    private GameObject lightCollider; //objeto na posicao onde a luz ira colidir
    private Vector3 posLuz; //direcao pra onde o objeto da posicao final ira
    //private LineRenderer lineRenderer;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        //bulletSpawn = GameObject.Find("BulletSpawn").transform;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        // spriteRenderer.color = corLaser;

        lightCollider = new GameObject("lightCollider"); //criar objeto vazio com o nome "lightCollider"
        Light light = lightCollider.AddComponent<Light>(); //adicionar componente luz ao objeto

        light.intensity = 8; //alterar valores do componenente
        light.bounceIntensity = 8; //8 = valor maximo
        light.range = larguraFinal * 2;
        light.color = corLaser;

        posLuz = new Vector3(larguraFinal, 0, 0);

        //lineRenderer = gameObject.AddComponent<LineRenderer>(); //criar uma linha no objeto do script (cria linha entre duas posicoes)
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive")); //adicionar um material a linha parecido com um "laser"
        //lineRenderer.startColor = corLaser; // cor inicial e cor final //setColors(inicial, final) obsoleto
        //lineRenderer.endColor = corLaser;
        //lineRenderer.startWidth = larguraInicial; // largura inicial e largura final //setWidth(inicial, final) obsoleto
        //lineRenderer.endWidth = larguraFinal;
        //lineRenderer.positionCount = 2; //quantia de pontos da linha //SetVertexCount(2) e numPositions obsoleto

        //lineRenderer.useWorldSpace = true;
        //lineRenderer.enabled = true;

        ////SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //Debug.Log(SortingLayer.NameToID("Player"));
        //lineRenderer.sortingLayerID = SortingLayer.NameToID("Player"); // spriteRenderer.sortingLayerID;
        //lineRenderer.sortingOrder = 0; // spriteRenderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletSpawnTransform != null)
        {
            DoLaser();
        }
    }

    public void DoLaser()
    {
        Vector3 pontoFinalLaser = bulletSpawnTransform.position + bulletSpawnTransform.right * maxDistLaser; //pra frente vermelho ate distancia maxima

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Ray2D ray = new Ray2D(bulletSpawnTransform.position, bulletSpawnTransform.right); //criar raio da posicao na direcao da seta vermelha (do mundo) 
        pontoColisao = Physics2D.Raycast(ray.origin, bulletSpawnTransform.right, maxDistLaser, 9); //com ponto de colisao e distancia e ignorar layer 9 do character
        //new ContactFilter2D();

        Vector3 startPos = bulletSpawnTransform.position + bulletSpawnTransform.right;
        Vector3 centerPos = new Vector3(startPos.x + pontoFinalLaser.x, startPos.y + pontoFinalLaser.y) / 2;
        float scaleX = Mathf.Abs(startPos.x - pontoFinalLaser.x);
        float scaleY = Mathf.Abs(startPos.y - pontoFinalLaser.y);

        //Debug.Log(pontoColisao.collider);
        if (pontoColisao.collider) //verificar hit com algo
        {
            //lineRenderer.SetPosition(0, transform.position); //linha entre origem e request da colisao/ponto onde aconteceu a colisao
            //lineRenderer.SetPosition(1, pontoColisao.point); //ponto onde o raio colidiu

            lightCollider.transform.position = pontoColisao.point - (Vector2)posLuz; //alterar fim do laser

            centerPos = new Vector3(startPos.x + pontoColisao.point.x, startPos.y + pontoColisao.point.y) / 2;
            scaleX = Mathf.Abs(startPos.x - pontoColisao.point.x);
            scaleY = Mathf.Abs(startPos.y - pontoColisao.point.y);
        }
        else //se nao ocorreu colisao
        {
            //lineRenderer.SetPosition(0, transform.position); //linha entre origem e ponto final do laser
            //lineRenderer.SetPosition(1, pontoFinalLaser);

            lightCollider.transform.position = pontoFinalLaser; //alterar fim do laser
        }

        //centerPos.x -= 0.5f;
        //centerPos.y += 0.5f;

        //lightCollider.transform.position = pontoFinalLaser;


        //Strech(gameObject, transform.position, pontoFinalLaser, false);

        gameObject.transform.rotation = bulletSpawnTransform.rotation;
        gameObject.transform.position = centerPos;
        gameObject.transform.localScale = new Vector3(scaleY, scaleX, 1) * 2;
    }

    //teste pronto
    public void Strech(GameObject _sprite, Vector3 _initialPosition, Vector3 _finalPosition, bool _mirrorZ)
    {
        Vector3 centerPos = (_initialPosition + _finalPosition) / 2f;
        _sprite.transform.position = centerPos;

        Vector3 direction = _finalPosition - _initialPosition;
        direction = Vector3.Normalize(direction);
        _sprite.transform.right = direction;

        if (_mirrorZ) _sprite.transform.right *= -1f;

        Vector3 scale = new Vector3(1, 1, 1);
        scale.x = Vector3.Distance(_initialPosition, _finalPosition);
        _sprite.transform.localScale = scale;
    }

    private void OnDestroy()
    {
        Destroy(lightCollider);
    }
}
