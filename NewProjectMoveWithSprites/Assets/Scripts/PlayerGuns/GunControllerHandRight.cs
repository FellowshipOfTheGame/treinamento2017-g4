﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerHandRight : MonoBehaviour //todos os leftClicks tem o controle da arma (armas left, cores right)
{
    //public
    //public Transform gunPivot; // pivot da arma para rotacao
    public GameObject bulletPrefab; //bala (para teste)
    //public GameObject laserPrefab; //laser (para teste)

    public float fireRate; //frequencia per sec
    public float bulletSpeed;
    public float bulletLifeTime;

    public int bulletDamage; //dano da bala

    public AudioClip shootSound; //som do tiro

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject player; //usuario da arma

    private float tempo;
    //private Vector2 playerPosition;
    //private bool bulletX; //tipo da bala que vai atirar (tiro/laser, para teste)

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

        //bulletX = false;
        tempo = 0f;
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
        audioSource.PlayOneShot(shootSound); //tocar som de atirar

        //criar bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        bullet.layer = LayerMask.NameToLayer("PlayerBullet"); //informar layer da bala

        BulletHealth bulletH = bullet.GetComponent<BulletHealth>(); //pegar script de vida da bala
        if (bulletH != null) //se conseguiu
        {
            bulletH.source = "Character"; //informar fonte da bala
            bulletH.bulletDamage = bulletDamage; //informar dano da bala

            bulletH.isFreezingBullet = true; //informar que congela/ou queima (isFreeezingBullet/isBurningBullet)
        }

        //velocidade da bala (enviar funcao)
        bullet.gameObject.SendMessageUpwards("SetVelocity", bulletSpeed, SendMessageOptions.DontRequireReceiver); //enviar velocidade, nao requisitar erro caso nao encontre

        //destruir bullet apos x segundos
        Destroy(bullet, bulletLifeTime); //tempo de vida da bullet

    }

    private void FixedUpdate()
    {

    }

    //Girar a arma
    //void Gun_Turning()

    //Pega o angulo em graus entre a posicao do mouse e o centro da tela (começa em cima==> 0, sentido horario direita==> 90)
    //float Gun_MouseToScreenCenter() //"girar"

}
