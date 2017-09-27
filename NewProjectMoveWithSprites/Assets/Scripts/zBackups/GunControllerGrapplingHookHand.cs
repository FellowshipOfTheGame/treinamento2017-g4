using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerGrapplingHookHand : MonoBehaviour
{
    //public
    public int bulletDamage; //dano da bala
    public string source; //fonte da bala --> se enemy dar dano em player vice-versa
    //public float destroyDelay; //delay pra destruicao da bala depois de colidir

    public bool isFreezingBullet; //eh uma bala que congela? 
    public bool isBurningBullet; //eh uma bala que queima?

    public float knockBackForce; //forca de knockback da bala (so funciona com o player/etc) //1000f parece funcionar bem

    //private
    private GunControllerGrapplingHookArm grapplingHookArmScript; //referencia ao script do braco
    //private int bulletDamage; //dano da bala

    private string target; //quem vai levar dano dessa bala?
    // private string layer; //layer do source
    private int collisionMask; //mascara para saber com quem a bala colide

    private bool canDoDamage; //verificar se pode dar dano naquele alvo
    private bool restartDamage; //verificar se pode dar dano naquela jogada de novo

    private GameObject player; //usuario da arma
    private DistanceJoint2D distanceJoint; //pegar distance joint do player

    private bool canAttach; //verifica se pode "grudar" no objeto para ser "grapplingHook"
    private bool isAttached; //verifica se o "grapplingHook" ja esta "grudado" em algum objeto

    private Vector3 actualPlayerPosition; //guarda posicao do player
    private Vector3 actualHandPosition; //guarda posicao da mao

    private SpriteRenderer spriteRendererHand;
    private SpriteRenderer spriteRendererArm;
    //private float step; //mudanca de tamanho do grappling hook para puxar

    // Use this for initialization
    void Start()
    {
        GameObject androidArm = gameObject.transform.parent.transform.GetChild(1).gameObject; //pegar braco e seu script
        grapplingHookArmScript = androidArm.GetComponent<GunControllerGrapplingHookArm>();
        spriteRendererHand = gameObject.GetComponent<SpriteRenderer>();
        spriteRendererArm = androidArm.GetComponent<SpriteRenderer>();
        //bulletDamage = androidArmScript.bulletDamage; //setar bullet damage

        SetTargetAndCollisionMask(); //setar o source e a collisionMask

        canDoDamage = true; //verifica se pode dar dano
        restartDamage = true;

        player = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)
        distanceJoint = player.GetComponent<DistanceJoint2D>(); //pegar distance joint do player
        distanceJoint.enabled = false;

        spriteRendererArm.enabled = false;
        spriteRendererHand.enabled = false;

        canAttach = false; //verifica se pode "grudar" no objeto para ser "grapplingHook"
    }

    public void SetTargetAndCollisionMask()//setar o source e a collisionMask
    {
        if (source.Equals("Enemy"))
        {
            target = "Character"; //caso inimigo --> target player
            //collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy"); //pegar layer corrente (bit shift + bit or)
        }
        else if (source.Equals("Character"))
        {
            target = "Enemy"; //caso player --> target inimigo
            //layer = "Character";
            //collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character"); //pegar layer corrente (bit shift + bit or)
        }

        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)    
    }

    // Update is called once per frame
    void Update()
    {
        if (!grapplingHookArmScript.GetIsThrowing() && !grapplingHookArmScript.GetIsPulling() && !grapplingHookArmScript.GetIsThrowingAndPulling())
        {
            canDoDamage = false;
            restartDamage = true;
            //canAttach = true;
        }
        else
        {
            if (grapplingHookArmScript.GetIsThrowing())
            { //soh pode dar dano enquanto esta jogando
                if (restartDamage)
                {
                    canDoDamage = true;
                }
            }
            else if (grapplingHookArmScript.GetIsPulling()) //nao pode dar dano enquanto esta puxando
            {
                canDoDamage = false;
                //canAttach = false;
            }
        }

        if (/*Input.GetButtonDown("Fire1")*/Input.GetKeyDown(KeyCode.Q)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
        {
            canAttach = true;

            spriteRendererArm.enabled = true;
            spriteRendererHand.enabled = true;
        }
        if (/*Input.GetButtonUp("Fire1")*/Input.GetKeyUp(KeyCode.Q))
        {
            canAttach = false;
            isAttached = false;
            distanceJoint.enabled = false; //desativa distance joint

            spriteRendererArm.enabled = false;
            spriteRendererHand.enabled = false;
        }

        if (isAttached)
        {
            Vector3 newActualHandPosition = gameObject.transform.position; //nova posicao da mao
            float handDistance = Vector3.Distance(actualHandPosition, actualPlayerPosition); //distancia da posicao onde fixou
            float newHandDistance = Vector3.Distance(newActualHandPosition, actualPlayerPosition); //distancia da nova posicao da mao

            if (handDistance > newHandDistance)
            { //caso a posicao antiga seja maior que a atual
                distanceJoint.distance = newHandDistance; //atualizar distancia do grappling hook (distance joint)
                //actualHandPosition = newActualHandPosition; //atualizar nova posicao da mao/player
                actualPlayerPosition = player.gameObject.transform.position; //atualizar posicao do player 
            }
        }

    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    StopThrowingAndTryDoDamage(other);
    //}

    private void OnTriggerEnter2D(Collider2D other)
    { //ao entrar tentar dar dano/parar
        StopThrowingAndTryDoDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    { //ao ficar dentro tentar dar dano/parar
        StopThrowingAndTryDoDamage(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    { //ao sair resetar a possibilidade de dar dano
        //canDoDamage = true;
    }

    public void StopThrowingAndTryDoDamage(Collider2D other) //verificar se esta vendo a possibilidade de dar dano
    {
        //Debug.Log(other.gameObject.name);
        if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        {
            //Debug.Log(other.gameObject.name);
            //StartCoroutine(BulletDestroy()); //destroi bala
            if (grapplingHookArmScript.GetIsThrowing())
            {
                grapplingHookArmScript.stopThrowing = true;
            }

            if (canAttach && !isAttached)
            {
                TryAttachGrapplingHook(); //tenta fixar o grappling hook para puxar
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer(target) && canDoDamage)
        {
            //DoDamage(other); //dar dano
            canDoDamage = false;
            restartDamage = false;
            //DoKnockBack(other.transform.gameObject, knockBackForce); //dar knockback

            canAttach = false;
            isAttached = false;
            distanceJoint.enabled = false; //desativa distance joint

            spriteRendererArm.enabled = false;
            spriteRendererHand.enabled = false;
        }
    }

    public void TryAttachGrapplingHook() //tenta fixar o grappling hook para puxar
    {
        isAttached = true;

        //Vector3 handPosition = gameObject.transform.position; //pega posicao da colisao/da mao
        //Vector3 playerPosition = player.transform.position; //pega posicao do player
        actualHandPosition = gameObject.transform.position; //pega posicao da colisao/da mao
        actualPlayerPosition = player.transform.position; //pega posicao do player

        //distanceJoint.anchor = playerPosition;
        distanceJoint.connectedAnchor = actualHandPosition;

        distanceJoint.distance = Vector3.Distance(actualHandPosition, actualPlayerPosition);

        distanceJoint.enabled = true; //ativa distance joint
    }

    public void DoDamage(Collider2D other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessageUpwards("ReceiveDamage", bulletDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

        if (isFreezingBullet) //se for bala congelante ("DoStatus")
        {
            //congelar pelo tempo padrao (-1f)
            other.gameObject.SendMessageUpwards("Freeze", -1f, SendMessageOptions.DontRequireReceiver); //enviar freeze, nao requisitar erro caso nao encontre
        }
        if (isBurningBullet) //se for bala que queima
        {
            //queimar pelo tempo padrao (-1f)
            other.gameObject.SendMessageUpwards("Burn", -1f, SendMessageOptions.DontRequireReceiver); //enviar burn, nao requisitar erro caso nao encontre
        }
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

    public int GetCollisionMask()
    {
        return collisionMask;
    }

}
