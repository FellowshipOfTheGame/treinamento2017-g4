using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    //public
    public float freezeTime; //tempo que vai ficar congelado (default)
    public float freezeDelay; //tempo para poder ficar congelado novamente

    public float burnTime; //tempo que vai ficar queimando (default)
    public float burnDelay; //tempo para poder queimar novamente
    public int burnDamage; //dano do queimando
    public float burnPerTime; //de quanto em quanto tempo queima / da dano

    public Color freezeColor; //cor quando congelado
    public Color burnColor; //cor quando queimando

    public bool canBeFrozen; //pode ser congelado? 
    public bool canBeBurned; //pode ser queimado?

    public float freezeChangeColorOnPercentOf; //mudar de cor em  X% do tempo total (congelado)
    public float burnChangeColorOnPercentOf; //mudar de cor em  X% do tempo total (queimando) 

    //private
    private SpriteRenderer spriteR; //sprite do objeto
    //private EnemyHealth enemyH; //Script enemyHealth do inimigo

    private float tempoFreeze; //tempo parametro do congelado
    private float tempoBurn; //tempo parametro do queimando

    private bool canFreeze; //para verificar se pode congelar
    private bool canBurn; //para verificar se pode queimar

    private bool changeColorRunning; //para verificar seja nao esta rodando

    // Use this for initialization
    void Start()
    {
        spriteR = gameObject.GetComponent<SpriteRenderer>(); //pegar sprite renderer component
        //enemyH = gameObject.GetComponent<EnemyHealth>(); //pegar enemy health script component

        canFreeze = true; //inicializar algumas varizaveis
        canBurn = true;
        tempoFreeze = 0f;
        tempoBurn = 0f;
        changeColorRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canFreeze && canBeFrozen) //verificar se pode congelar novamente
        {
            tempoFreeze += Time.deltaTime;
            if (tempoFreeze > (freezeDelay)) //verificar delay
            {
                tempoFreeze = 0f;
                canFreeze = true;
            }
        }

        if (!canBurn && canBeBurned)//verificar se pode queimar novamente
        {
            tempoBurn += Time.deltaTime;
            if (tempoBurn > (burnDelay)) //verificar delay
            {
                tempoBurn = 0f;
                canBurn = true;
            }
        }
    }

    public void Freeze(float freezeTime) //congelar inimigo por tempo determinado
    {
        if (canFreeze) //se puder congelar novamente
        {
            canFreeze = false;

            freezeTime = freezeTime < 0f ? this.freezeTime : freezeTime; //se o freeze time for menor que 0 --> freeze default, senao colocar novo freeze time

            if (!changeColorRunning) StartCoroutine(ChangeColorTemporarily((freezeTime * freezeChangeColorOnPercentOf) / 100, freezeColor)); //mudar cor do inimigo por tempo determinado

            gameObject.SendMessageUpwards("ChangeSpeedTemporarily", new float[] { freezeTime, 0f }, SendMessageOptions.DontRequireReceiver); //enviar dano/"freeze", nao requisitar erro caso nao encontre
        }
    }

    public void Burn(float burnTime) //queimar inimigo por tempo determinado
    {
        if (canBurn) //se puder queimar novamente
        {
            canBurn = false;

            burnTime = burnTime < 0f ? this.burnTime : burnTime; //se o burn time for menor que 0 --> burnTime default, senao colocar novo burnTime time

            if (!changeColorRunning) StartCoroutine(ChangeColorTemporarily((burnTime * burnChangeColorOnPercentOf) / 100, burnColor)); //mudar cor do inimigo por tempo determinado

            StartCoroutine(BurnDamage(burnTime)); //queimar inimigo por tempo X
        }
    }

    public IEnumerator BurnDamage(float burnTime) //queimar inimigo por tempo X
    {
        InvokeRepeating("BurnDoDamage", burnPerTime, burnPerTime); //dar dano no inimigo por tempo X, comecar apos mesmo tempo X

        yield return new WaitForSeconds(burnTime);

        CancelInvoke("BurnDoDamage"); //para de dar dano
    }

    public void BurnDoDamage() //dar dano no inimigo por tempo X
    {
        gameObject.SendMessageUpwards("ReceiveDamage", burnDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre

        //enemyH.health -= burnDamage;
    }

    public IEnumerator ChangeColorTemporarily(float time, Color cor) //mudar cor do inimigo por tempo determinado
    {
        changeColorRunning = true;

        //Debug.Log("StartingColorChange");
        Color objectColor = spriteR.color; //armazenar e alterar cor do inimigo
        spriteR.color = cor;

        yield return new WaitForSeconds(time);

        spriteR.color = objectColor; //retornar ao normal apos x segundos

        changeColorRunning = false;
    }
}
