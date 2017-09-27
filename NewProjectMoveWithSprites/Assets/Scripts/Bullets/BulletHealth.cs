using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealth : MonoBehaviour //contem o dano e a vida (+destroy etc) da bala (para perfuraveis --> ainda nao feito)
{
    //public
    public int bulletDamage; //dano da bala
    public string source; //fonte da bala --> se enemy dar dano em player vice-versa

    public float destroyDelay; //delay pra destruicao da bala depois de colidir

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem
    //public GameObject bulletSource; //source da bala //quem atirou (para o knock back // preenchido por script)

    public AudioClip hitFireSound; //som do tiro acertando de fogo
    public AudioClip hitFreezeSound; //som do tiro acertando de gelo

    //private
    private string target; //quem vai levar dano dessa bala?
    // private string layer; //layer do source
    private int collisionMask; //mascara para saber com quem a bala colide

    private AudioSource audioSource; //componente de audio
    private bool canDoDamage; //para auxiliar o audio

    // Use this for initialization
    void Start()
    {
        canDoDamage = true;
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    //quando entrar no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDoDamage && other.gameObject.layer == LayerMask.NameToLayer(target))
        {
            DoDamage(other);
            DoKnockBack(other);
        }

        //Debug.Log(other.gameObject.name);
        if (canDoDamage && (collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        {
            //Debug.Log(other.gameObject.name);
            StartCoroutine(BulletDestroy()); //destroi bala
        }
    }

    public void DoDamage(Collider2D other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", bulletDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

        //EnemyStatus enemyS = other.gameObject.GetComponent<EnemyStatus>(); //tentar pegar o script de status do alvo
        //if (enemyS != null) //se conseguir
        //{
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
        //}
    }

    public IEnumerator BulletDestroy() //destroi a bala
    {
        canDoDamage = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        if (isFreezingBullet) //se for bala congelante
        {
            destroyDelay = destroyDelay >= hitFreezeSound.length ? destroyDelay : hitFreezeSound.length;
            audioSource.PlayOneShot(hitFreezeSound); //tocar som de acertar
        }
        else //if (isBurningBullet) //se for bala que queima
        {
            if (source.Equals("Character"))
            {
                destroyDelay = destroyDelay >= hitFireSound.length ? destroyDelay : hitFireSound.length;
                audioSource.PlayOneShot(hitFireSound); //tocar som de acertar
            }
        }
        //gameObject.SetActive(false);

        yield return new WaitForSeconds(destroyDelay); //esperar x segundos antes de destruir bala

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
        //GameObject player = GameObject.Find("Player");

        //if (bulletSource == null)
        //{
        //    Debug.Log("KnockBack Impossible");
        //    return;
        //}
        if (rb2d == null)
        {
            Debug.Log("KnockBack Seems to Be Impossible!");
            return;
        }

        Vector3 direct = gameObject.transform.position - other.transform.position;

        direct = -direct.normalized; //normalizar e inverter ponto de colisao

        rb2d.AddForce(direct * knockBackForce); //adicionar forca de knockback e direcao para empurrar o player
    }
}


//public void DoKnockBack(Collider2D other)
//{
//    float knockbackForce = 1000f;
//    Rigidbody2D rb2d = other.transform.gameObject.GetComponent<Rigidbody2D>();
//    //if (other.gameObject.CompareTag("Enemy")) //se for um inimigo //movel
//    //{
//    //ContactPoint2D[] contactPoints = new ContactPoint2D[10];
//    //other.GetContacts(contactPoints);
//    //Vector3 direct = contactPoints[0].point - (Vector2)transform.position; //calcular angulo entre o ponto de colisao e o player
//    Vector3 direct = GameObject.Find("Player").transform.position - transform.position;

//    direct = -direct.normalized; //normalizar e inverter ponto de colisao
//                                 //Debug.Log("saida");
//    rb2d.AddForce(direct * knockbackForce); //adicionar forca de knockback e direcao para empurrar o player
//    //}

//    //rb2d.AddForce(ForceMode2D.Impulse);

//    //ForceMode2D.Impulse();
//    //other.GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
//}
