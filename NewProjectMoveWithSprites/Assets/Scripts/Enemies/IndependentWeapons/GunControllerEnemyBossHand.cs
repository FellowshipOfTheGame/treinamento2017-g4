using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerEnemyBossHand : MonoBehaviour
{
    //public
    public int bulletDamage; //dano da bala
    public string source; //fonte da bala --> se enemy dar dano em player vice-versa
    //public float destroyDelay; //delay pra destruicao da bala depois de colidir

    //private
    private GunControllerEnemyBossArm androidArmScript; //referencia ao script do braco
    //private int bulletDamage; //dano da bala

    private string target; //quem vai levar dano dessa bala?
    private int collisionMask; //mascara para saber com quem a bala colide

    private bool canDoDamage; //verificar se pode dar dano naquele alvo

    // Use this for initialization
    void Start()
    {
        GameObject androidArm = gameObject.transform.parent.transform.GetChild(1).gameObject; //pegar braco e seu script
        androidArmScript = androidArm.GetComponent<GunControllerEnemyBossArm>();
        //bulletDamage = androidArmScript.bulletDamage; //setar bullet damage

        SetTargetAndCollisionMask(); //setar o source e a collisionMask

        canDoDamage = true;
    }

    public void SetTargetAndCollisionMask()//setar o source e a collisionMask
    {
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

        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("Enemy"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)    
    }

    // Update is called once per frame
    void Update()
    {
        if (!androidArmScript.GetIsThrowing() && !androidArmScript.GetIsPulling() && !androidArmScript.GetIsThrowingAndPulling())
        {
            canDoDamage = false;
        }
        else
        {
            if (androidArmScript.GetIsThrowing())
            { //soh pode dar dano enquanto esta jogando
                canDoDamage = true;
            }
            else if (androidArmScript.GetIsPulling()) //nao pode dar dano enquanto esta puxando
            {
                canDoDamage = false;
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
            if (androidArmScript.GetIsThrowing())
            {
                androidArmScript.stopThrowing = true;
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer(target) && canDoDamage)
        {
            DoDamage(other); //dar dano
            canDoDamage = false;
        }
    }

    public void DoDamage(Collider2D other) //dar dano no target (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", bulletDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre
    }

    public int GetCollisionMask()
    {
        return collisionMask;
    }
}
