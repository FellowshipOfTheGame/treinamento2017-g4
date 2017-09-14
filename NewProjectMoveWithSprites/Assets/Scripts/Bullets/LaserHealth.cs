using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHealth : MonoBehaviour
{
    //public
    public int bulletDamage; //dano da bala
    public string source; //fonte da bala --> se enemy dar dano em player vice-versa

    public float destroyDelay; //delay pra destruicao da bala depois de colidir

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem
    //public GameObject bulletSource; //source da bala //quem atirou (para o knock back // preenchido por script)

    //private
    private LaserController3 laserControllerScript; //referencia ao script do braco
    private string target; //quem vai levar dano dessa bala?
    // private string layer; //layer do source
    private int collisionMask; //mascara para saber com quem a bala colide

    private bool canDoDamage; //verificar se pode dar dano naquele alvo

    // Use this for initialization
    void Start()
    {
        laserControllerScript = gameObject.GetComponent<LaserController3>();

        if (source.Equals("Enemy"))
        {
            target = "Character"; //caso inimigo --> target player
            collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy"); //pegar layer corrente (bit shift + bit or)
        }
        else if (source.Equals("Character"))
        {
            target = "Enemy"; //caso player --> target inimigo
            //layer = "Character";
            collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)
        }

        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)

        canDoDamage = true; //pode dar dano (dar em apenas 1)
    }

    // Update is called once per frame
    void Update()
    {

    }

    //quando entrar no trigger
    private void OnTriggerEnter2D(Collider2D other)
    { //ao entrar tentar dar dano/parar
        StopThrowingAndTryDoDamage(other);
    }

    public void StopThrowingAndTryDoDamage(Collider2D other) //verificar se esta vendo a possibilidade de dar dano
    {
        //Debug.Log(other.gameObject.name);
        if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        { //primeiro parar, depois realizar outros processos
            laserControllerScript.stopThrowing = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer(target) && canDoDamage)
        {
            DoDamage(other); //dar dano
            canDoDamage = false;
            DoKnockBack(other); //dar knockback
        }

        if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        {
            StartCoroutine(BulletDestroy()); //destroi bala
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