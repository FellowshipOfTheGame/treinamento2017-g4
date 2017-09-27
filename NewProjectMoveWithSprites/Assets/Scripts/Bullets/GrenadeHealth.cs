using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeHealth : MonoBehaviour
{
    //public
    public int bulletDamage; //dano da bala
    public int explosionDamage; //dano da explosao

    public string source; //fonte da bala --> se enemy dar dano em player vice-versa

    public float destroyDelay; //delay pra destruicao da bala depois de colidir

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem
    public float knockBackForceExplosion; //forca de knockback da explosao (so funciona com o player/etc) //1000f parece funcionar bem

    public AudioClip explosionSoundFreeze; //som de explodir pra congelante
    public AudioClip explosionSoundBurn; //som de explodir generico

    //private
    private string target; //quem vai levar dano dessa bala?
    // private string layer; //layer do source
    private int collisionMask; //mascara para saber com quem a bala colide

    private bool bulletDestroying; //verifica se ja nao esta destruindo a bala/granada

    private AudioSource audioSource; //componente de audio

    // Use this for initialization
    void Start()
    {
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

        //bulletDestroying = true;
        bulletDestroying = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //quando entrar no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(target))
        {
            DoDamage(other); //dar dano
            DoKnockBack(other.transform.gameObject, knockBackForce); //dar knockback ao acertar
        }

        //Debug.Log(other.gameObject.name);
        if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
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

    public void DoDamage(GameObject other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", explosionDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

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

    public void DestroyByTime(float lifeTime)
    {
        StartCoroutine(WaitDestruction(lifeTime));
    }

    public IEnumerator WaitDestruction(float lifeTime)
    {
        //Debug.Log("Waiting for time");
        yield return new WaitForSeconds(lifeTime);
        //Debug.Log("Starting destruction");
        //BulletDestroy();
        StartCoroutine(BulletDestroy()); //destroi bala
    }

    public IEnumerator BulletDestroy() //destroi a bala
    {
        if (!bulletDestroying)
        {
            bulletDestroying = true;

            GrenadeController grenadeC = gameObject.GetComponent<GrenadeController>();
            float limitTime = grenadeC.Explode(collisionMask); // tempo para explodir

            yield return new WaitForSeconds(destroyDelay); //esperar x segundos antes de destruir bala

            HashSet<GameObject> listOfDamages = grenadeC.GetListOfDamages();
            foreach (GameObject gameO in listOfDamages) ////pra cada objeto verificar se eh um target e dar dano
            {
                if (gameO != null && gameO.gameObject.layer == LayerMask.NameToLayer(target))
                {
                    GameObject gmO = DoLaser((gameO.transform.position - gameObject.transform.position), grenadeC.explosionRadius); //tentar acertar o alvo pela range
                    if (gmO != null && gmO.layer == gameO.layer) //verifica se layers batem (enemy cm enemy) [qualquer coisa tirar]
                    { //se foi possivel alcancar o alvo (sem barreiras)
                        if (gmO.gameObject.layer == LayerMask.NameToLayer(target)) //dar dano e knock back se for um target (qualquer coisa alterar pro player tbm levar dano)
                        {
                            //Debug.Log("Alo: " + gameO);
                            DoDamage(gmO); //dar dano
                            DoKnockBack(gmO.transform.gameObject, knockBackForceExplosion); //dar knockback ao acertar
                        }
                        //DoDamage(gameO); //dar dano
                    }
                }
            }

            if (isFreezingBullet) //se for congelante
            {
                limitTime = limitTime >= explosionSoundFreeze.length ? limitTime : explosionSoundFreeze.length; //se o maior tempo for o da animacao usar ele se n usar o do som de explosao
                audioSource.PlayOneShot(explosionSoundFreeze); //tocar som de explodir
            }
            else //se n
            {
                limitTime = limitTime >= explosionSoundBurn.length ? limitTime : explosionSoundBurn.length; //se o maior tempo for o da animacao usar ele se n usar o do som de explosao
                audioSource.PlayOneShot(explosionSoundBurn); //tocar som de explodir
            }

            Destroy(gameObject, limitTime); //destroi bala quando colide //implementar bala perfuravel--> tirar destroy/arrumar //tempo pra destruir objeto
        }
    }

    public GameObject DoLaser(Vector3 direction, float maxRange) //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        //Transform bulletSpawnTransform = bulletSpawn.transform; //pegar transform
        //Vector3 pontoFinalLaser = bulletSpawnTransform.position + bulletSpawnTransform.right * maxRange; //pra frente vermelho ate distancia maxima

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
            //lightCollider.transform.position = pontoColisao.point;// - (Vector2)posLuz; //alterar fim do laser

            return pontoColisao.transform.gameObject;
            //if (pontoColisao.transform.gameObject == target) return true;

            //Debug.Log(pontoColisao.transform.gameObject.name);
            //Debug.Log(startPos + "  " + scale.magnitude);
        }

        return null; //nao acertou o player
    }

    public void DoKnockBack(GameObject other, float knockbackForce) //adicionar knock back [sempre pra longe de quem atirou] (soh esta funcionando com o player/etc)
    {
        if (knockBackForce == 0) return; //se for 0, n fazer nada

        Rigidbody2D rb2d = other.gameObject.GetComponent<Rigidbody2D>();
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
