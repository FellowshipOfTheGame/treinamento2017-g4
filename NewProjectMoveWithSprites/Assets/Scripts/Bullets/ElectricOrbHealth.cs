using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricOrbHealth : MonoBehaviour
{
    //public
    public int bulletDamage; //dano da bala
    public string source; //fonte da bala --> se enemy dar dano em player vice-versa

    public float destroyDelay; //delay pra destruicao da bala depois de colidir

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem
    //public GameObject bulletSource; //source da bala //quem atirou (para o knock back // preenchido por script)

    public float bulletSpeedPercent; //percentagem de velocidade da bala mantida apos refletir

    public AudioClip vanishSound; //som do tiro desfazendo

    //private
    private string target; //quem vai levar dano dessa bala?
    // private string layer; //layer do source
    private int collisionMask; //mascara para saber com quem a bala colide
    private int wallsCollisonMask; //mascara para saber objetos onde a bala reflete

    private ElectricOrbController electricOrbC; //script ElectricOrbController

    private float tempo;
    private bool soundNotPlayed;
    private AudioSource audioSource; //componente de audio

    // Use this for initialization
    void Start()
    {
        tempo = 0;
        soundNotPlayed = true;
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        electricOrbC = gameObject.GetComponent<ElectricOrbController>(); //pegar script ElectricOrbController 

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

        wallsCollisonMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        wallsCollisonMask = ~wallsCollisonMask; //todas menos as selecionadas (inverter)
    }

    // Update is called once per frame
    void Update()
    {
        tempo += Time.deltaTime;
        if (soundNotPlayed && vanishSound != null && tempo >= (destroyDelay - vanishSound.length))
        {
            audioSource.PlayOneShot(vanishSound); //tocar som de atirar
            soundNotPlayed = false;
        }
    }

    //quando entrar no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(target))
        {
            DoDamage(other);
            DoKnockBack(other);
        }

        //Debug.Log(other.gameObject.name);
        if ((wallsCollisonMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        {
            //Debug.Log(other.gameObject.name);
            //StartCoroutine(BulletDestroy()); //destroi bala
            ReflectBullet(); //refletir bala (quicar pelo cenario)
        }
    }

    public void DoDamage(Collider2D other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", bulletDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

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

    public IEnumerator BulletDestroy() //destroi a bala
    {
        yield return new WaitForSeconds(destroyDelay); //esperar x segundos antes de destruir bala

        audioSource.PlayOneShot(vanishSound); //tocar som de atirar

        yield return new WaitForSeconds(vanishSound.length); //esperar x segundos antes de destruir bala para tocar o som

        Destroy(gameObject); //destroi bala quando colide //implementar bala perfuravel--> tirar destroy/arrumar
    }

    public int GetCollisionMask()
    {
        return collisionMask;
    }

    public void DoKnockBack(Collider2D other) //adicionar knock back [sempre pra longe de quem atirou] (soh esta funcionando com o player/etc)
    {
        //--teste
        //other.transform.gameObject.SendMessageUpwards("ChangeSpeedTemporarily", new float[] { 0.2f, -0.3f }, SendMessageOptions.DontRequireReceiver); //enviar dano/"freeze", nao requisitar erro caso nao encontre
        //--
        if (knockBackForce == 0) return; //se for 0, n fazer nada

        Rigidbody2D rb2d = other.transform.gameObject.GetComponent<Rigidbody2D>();

        if (rb2d == null)
        {
            Debug.Log("KnockBack Seems to Be Impossible!");
            return;
        }

        Vector3 direct = gameObject.transform.position - other.transform.position;

        direct = -direct.normalized; //normalizar e inverter ponto de colisao

        rb2d.AddForce(direct * knockBackForce); //adicionar forca de knockback e direcao para empurrar o player
    }

    public void ReflectBullet() //reflete a bala na superficie
    {
        RaycastHit2D pontoColisao = DoLaser(10f);

        if (pontoColisao.collider)
        {
            Vector3 incomingVector = pontoColisao.point - (Vector2)gameObject.transform.position; //pegar vetor de entrada da bala
            Vector3 reflectVector = Vector3.Reflect(incomingVector, pontoColisao.normal); //reflete um vetor de acordo com uma direcao e uma normal a superficie

            electricOrbC.SetVelocityAndDirection(reflectVector.normalized, bulletSpeedPercent);
        }
        else
        {
            Debug.Log("Problems with reflect!");
            StartCoroutine(BulletDestroy()); //destroi bala
        }
    }

    public RaycastHit2D DoLaser(/*Vector3 direction,*/ float maxRange) //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Vector3 direction = gameObject.transform.right;
        Ray2D ray = new Ray2D(gameObject.transform.position, direction); //criar raio da posicao na direcao da seta vermelha (do mundo) 

        //nova collision mask que atravessa inimigos/player/balas pra dar dano nas explosoes (qualquer coisa, para permitir "body block" remover)
        //int collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)

        pontoColisao = Physics2D.Raycast(ray.origin, direction, maxRange, wallsCollisonMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character

        //Debug.DrawLine(ray.origin, pontoColisao.point, new Color(10, 10, 10), 100f);

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        //if (pontoColisao.collider) //verificar hit com algo
        //{
        return pontoColisao;//pontoColisao.transform.gameObject;
        //if (pontoColisao.transform.gameObject == target) return true;
        //}

        //return null; //nao acertou o player
    }
}
