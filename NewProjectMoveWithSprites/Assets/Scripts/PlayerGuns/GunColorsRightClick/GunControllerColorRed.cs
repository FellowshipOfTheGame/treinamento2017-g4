using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerColorRed : MonoBehaviour
{
    //public
    //public Transform gunPivot; // pivot da arma para rotacao
    //public GameObject bulletPrefab; //bala (para teste)
    //public GameObject laserPrefab; //laser (para teste)

    public float fireRate; //frequencia per sec
    //public float bulletSpeed;
    //public float bulletLifeTime;
    public int areaDamage; //dano da bala/"explosao" em torno do player
    public float areaRadius; //raio da bala/"explosao" em torno do player

    public string source; //fonte da bala --> se enemy dar dano em player vice-versa

    public int typeOfKnockBack; //tipo 0 --> add force //tipo 1 --> changeVelocity (tipo um fear, talvez melhor)
    //Tipo1
    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem
    //Tipo2
    public float knockBackTime; //tempo de duracao do "knockBack"/fear (0.2f)
    public float knockBackSpeedChange; // Velocidade em porcentagem da atual/direcao (-0.3f)

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public AudioClip shootSound; //som do tiro disparando

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject player; //usuario da arma
    private string target; //quem vai levar dano dessa bala?

    private float tempo;
    private int collisionMask; //mascara para saber com quem a bala colide

    private HashSet<GameObject> listOfDamages; //lista de objetos que receberao dano na explosao

    private AudioSource audioSource; //componente de audio

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        //GameObject bulletSpawnn = GameObject.Find("BulletSpawn"); //posicionar bulletspawn no fim da arma
        bulletSpawn = gameObject.transform.GetChild(0); //pegar bulletSpawn

        BoxCollider2D gunBoxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        float initialPosX = transform.localPosition.x + gunBoxCollider2d.size.x;
        Vector3 bulletSpawnPos = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);// + transform.right;

        bulletSpawn.transform.localPosition = bulletSpawnPos;
        //bulletSpawn.SetPositionAndRotation(bulletSpawnPos, bulletSpawn.rotation);

        player = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)

        if (source.Equals("Enemy"))
        {
            target = "Character"; //caso inimigo --> target player
            collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        }
        else if (source.Equals("Character"))
        {
            target = "Enemy"; //caso player --> target inimigo
            //layer = "Character";
            collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        }

        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)

        //bulletX = false;
        tempo = 0f;
        listOfDamages = new HashSet<GameObject>(); //inicializar lista de objetos que receberao dano/knockback na explosao

        /*<----------->*/
        Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x + areaRadius, gameObject.transform.position.y, 0f), new Color(10f, 10f, 10f), 100f);
        /*<----------->*/
        Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + areaRadius, 0f), new Color(10f, 10f, 10f), 100f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            tempo += Time.deltaTime; //Fire1 --> LeftClick //Fire2 --> RightClick //Fire3 --> MiddleClick
            if (Input.GetButton("Fire2") && tempo > (1 / fireRate)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
            {
                tempo = 0f;

                Fire();
            }
        }
    }

    void Fire() //atirar
    {
        Explode(); //criar "explosao" em volta do player

        audioSource.PlayOneShot(shootSound); //tocar som de acertar

        foreach (GameObject gameO in listOfDamages) ////pra cada objeto verificar se eh um target e dar dano
        {
            if (gameO != null && gameO.gameObject.layer == LayerMask.NameToLayer(target))
            {
                GameObject gmO = DoLaser((gameO.transform.position - gameObject.transform.position), areaRadius); //tentar acertar o alvo pela range
                if (gmO != null && gmO.layer == gameO.layer) //verifica se layers batem (enemy cm enemy) [qualquer coisa tirar]
                { //se foi possivel alcancar o alvo (sem barreiras)
                    if (gmO.gameObject.layer == LayerMask.NameToLayer(target)) //dar dano e knock back se for um target (qualquer coisa alterar pro player tbm levar dano)
                    {
                        DoDamage(gmO); //dar dano
                        DoKnockBack(gmO.transform.gameObject); //dar knockback ao acertar
                    }
                    //DoDamage(gameO); //dar dano
                }
            }
        }
    }

    public void Explode() //explodir/dar dano/knockback em area
    {
        listOfDamages.Clear(); //limpar antes de usar
        Vector3 centralPoint = player.transform.position; //pegar ponto central da explosao

        Collider2D[] colliders; //colliders dos objetos que receberao dano
        colliders = Physics2D.OverlapCircleAll(centralPoint, areaRadius, collisionMask); //pegar todos os objetos com collider num raio x em uma mascara de colisao especifica

        foreach (Collider2D coll in colliders) //pra cada collider adicionar o gameobject na lista
        {
            if (coll.gameObject != gameObject)
            {
                //Debug.Log("GMObject1: " + coll.gameObject);
                //Debug.Log("GMObject2: " + coll.transform.gameObject);
                listOfDamages.Add(coll.gameObject);
            }
        }
    }

    public GameObject DoLaser(Vector3 direction, float maxRange) //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Ray2D ray = new Ray2D(gameObject.transform.position, direction); //criar raio da posicao na direcao da seta vermelha (do mundo) 

        //nova collision mask que atravessa inimigos/player/balas pra dar dano nas explosoes (qualquer coisa, para permitir "body block" remover)
        //int collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)

        pontoColisao = Physics2D.Raycast(ray.origin, direction, maxRange, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character

        /*<----------->*/
        Debug.DrawLine(ray.origin, pontoColisao.point, new Color(10, 10, 10), 100f);

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            return pontoColisao.transform.gameObject;
            //if (pontoColisao.transform.gameObject == target) return true;
        }

        return null; //nao acertou o player
    }

    public void DoDamage(GameObject other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", areaDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

        if (isFreezingBullet) //se for bala congelante
        {
            //enemyS.Freeze(-1f); //congelar pelo tempo padrao
            other.gameObject.SendMessage/*Upwards*/("Freeze", -1f, SendMessageOptions.DontRequireReceiver); //enviar freeze, nao requisitar erro caso nao encontre
        }
        if (isBurningBullet) //se for bala que queima
        {
            //enemyS.Burn(-1f); //queimar pelo tempo padrao
            other.gameObject.SendMessage/*Upwards*/("Burn", -1f, SendMessageOptions.DontRequireReceiver); //enviar burn, nao requisitar erro caso nao encontre
        }
    }

    public void DoKnockBack(GameObject other) //adicionar knock back [sempre pra longe de quem atirou] (soh esta funcionando com o player/etc)
    {
        if (knockBackForce == 0) return; //se for 0, n fazer nada

        if (typeOfKnockBack == 0)
        {
            Rigidbody2D rb2d = other.transform.gameObject.GetComponent<Rigidbody2D>();

            if (rb2d == null)
            {
                Debug.Log("KnockBack Seems to Be Impossible!");
                return;
            }

            Vector3 direct = player.transform.position - other.transform.position;

            direct = -direct.normalized; //normalizar e inverter ponto de colisao

            rb2d.AddForce(direct * knockBackForce); //adicionar forca de knockback e direcao para empurrar o player
        }
        else if (typeOfKnockBack == 1)
        {
            //--teste
            other.transform.gameObject.SendMessage/*Upwards*/("ChangeSpeedTemporarily", new float[] { knockBackTime, knockBackSpeedChange }, SendMessageOptions.DontRequireReceiver); //enviar dano/"freeze", nao requisitar erro caso nao encontre
            //--
        }
    }
}
