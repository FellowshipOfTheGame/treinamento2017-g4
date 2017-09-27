using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController2 : MonoBehaviour //a curto prazo, devido a colisao, apenas o player pode utilizar esse laser (por enquanto) [da pra ajustar igual ao da bala]
{
    //public (iniciais p/ lembrar)
    public Color corLaser; //cor do laser   (Color.green)
    public int maxDistLaser; //distancia maxima do laser (50)
    public float larguraInicial; // (0.02f)
    public float larguraFinal; // (0.1f)

    //public GameObject player;
    //public GameObject gun;

    public Transform bulletSpawnTransform;
    //public GameObject bulletPrefab; //soh para teste

    public float speedResize;
    public float differenceAdjust; //preferencia: 0.32 //serve para reajustar a range final do laser

    public float stopTime; //tempo que o inimigo fica parado pra receber o laser

    //private
    private GameObject lightCollider; //objeto na posicao onde a luz ira colidir
    //private Vector3 posLuz; //direcao pra onde o objeto da posicao final ira
    //private LineRenderer lineRenderer;

    private SpriteRenderer spriteRenderer;

    private bool doLaser;
    private Vector3 finalScale;

    private int collisionMask;

    // Use this for initialization
    void Start()
    {
        doLaser = true;
        //bulletSpawn = GameObject.Find("BulletSpawn").transform;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.color = corLaser;

        lightCollider = new GameObject("lightCollider"); //criar objeto vazio com o nome "lightCollider"
        Light light = lightCollider.AddComponent<Light>(); //adicionar componente luz ao objeto

        light.intensity = 8; //alterar valores do componenente
        light.bounceIntensity = 8; //8 = valor maximo
        light.range = larguraFinal * 2;
        light.color = corLaser;

        //posLuz = new Vector3(larguraFinal, 0, 0);

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

        //default
        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletSpawnTransform != null)
        {
            if (doLaser)
            {
                finalScale = DoLaser();
                doLaser = false;
            }
            if (transform.localScale == finalScale) Destroy(gameObject); //se o laser atingir a scale final do tiro, destruir laser

            gameObject.transform.localScale = Vector3.Lerp(transform.localScale, finalScale, speedResize * Time.deltaTime);//Mathf.SmoothStep(0.0f, 1.0f, speedResize * Time.deltaTime)); //reescalonar com o tempo o laser ate um ponto finalScale 
        }
    }

    public Vector3 DoLaser()
    {
        BulletHealth bulletH = gameObject.GetComponent<BulletHealth>();
        if (bulletH != null && bulletH.GetCollisionMask() != 0) //verificar se ja existe, e se nao eh nulo
        {
            collisionMask = bulletH.GetCollisionMask(); //setar mascara de colisao (pra dependendo de quem segura a arma, etc)
        }

        Vector3 pontoFinalLaser = bulletSpawnTransform.position + bulletSpawnTransform.right * maxDistLaser; //pra frente vermelho ate distancia maxima

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Ray2D ray = new Ray2D(bulletSpawnTransform.position, bulletSpawnTransform.right); //criar raio da posicao na direcao da seta vermelha (do mundo) 
        //Physics2D.queriesStartInColliders = false;

        //int collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)
        //collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)

        pontoColisao = Physics2D.Raycast(ray.origin, bulletSpawnTransform.right, maxDistLaser, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layer 9 do character
        //new ContactFilter2D();

        Vector3 startPos = bulletSpawnTransform.position;// + bulletSpawnTransform.right;
        //Vector3 centerPos = new Vector3(startPos.x + pontoFinalLaser.x, startPos.y + pontoFinalLaser.y) / 2;
        float scaleX = Mathf.Abs(startPos.x - pontoFinalLaser.x);
        float scaleY = Mathf.Abs(startPos.y - pontoFinalLaser.y);
        Vector2 scale = new Vector2(scaleX, scaleY);

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //lineRenderer.SetPosition(0, transform.position); //linha entre origem e request da colisao/ponto onde aconteceu a colisao
            //lineRenderer.SetPosition(1, pontoColisao.point); //ponto onde o raio colidiu

            lightCollider.transform.position = pontoColisao.point;// - (Vector2)posLuz; //alterar fim do laser

            //centerPos = new Vector3(startPos.x + pontoColisao.point.x, startPos.y + pontoColisao.point.y) / 2;

            scaleX = Mathf.Abs(startPos.x - pontoColisao.point.x);
            scaleY = Mathf.Abs(startPos.y - pontoColisao.point.y);
            scale = new Vector2(scaleX, scaleY);
            //scale = pontoColisao.point;

            //Debug.Log(pontoColisao.point);
            //Debug.Log(startPos + "  " + scale.magnitude);

            //pra arrumar o "delay" de chegar o laser --> parar objeto momentaneamente
            if (pontoColisao.transform.gameObject.CompareTag("Enemy")) //torna meio impossivel um freeze laser (a nao ser que altere o stopTime)
            {
                pontoColisao.transform.gameObject.SendMessageUpwards("ChangeSpeedTemporarily", new float[] { stopTime, 0f }, SendMessageOptions.DontRequireReceiver); //enviar "freeze", nao requisitar erro caso nao encontre
            }

            //criar bala no ponto final do laser
            //GameObject bullet = Instantiate(bulletPrefab, pontoColisao.point, bulletSpawnTransform.rotation); //para teste
            //Debug.Log(bullet.transform.localPosition); //criar um "laserEnd" para arrumar a colisao final
            //Debug.Log("sizeBullet: " + bullet.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x); //para teste
            //Destroy(bullet, 4f);
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

        //gameObject.transform.position = centerPos;
        gameObject.transform.position = bulletSpawnTransform.position;
        //gameObject.transform.localScale = new Vector3(scaleX, scaleY, 1); //*2

        Bounds b = gameObject.GetComponent<SpriteRenderer>().sprite.bounds; //isso deu trabalho pra encontrar '-'-' .-.-.
        float xSize = b.size.x;
        //Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite.bounds);

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f)); //nao tao necessario agr, mas normaliza rotacao aumenta dps rotaciona
        //gameObject.transform.localScale = new Vector3(scale.magnitude / xSize, larguraFinal, 1); //*2 //encontrar proporcao certa '-'
        //gameObject.transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scale.magnitude / xSize, larguraFinal, 1), speedResize * Time.deltaTime); //*2 //encontrar proporcao certa '-'
        gameObject.transform.rotation = bulletSpawnTransform.rotation;

        return new Vector3((scale.magnitude + differenceAdjust) / xSize, larguraFinal, 1); //scale.magnitude = tamanho do desenho do traço do laser
    }

    //teste pronto (nao utilizado)
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

    //ao destruir o objeto
    private void OnDestroy()
    {
        Destroy(lightCollider);
    }

    public void SetBulletSpawnTransform(Transform bulletSpawnTransform)
    {
        this.bulletSpawnTransform = bulletSpawnTransform;
    }
}
