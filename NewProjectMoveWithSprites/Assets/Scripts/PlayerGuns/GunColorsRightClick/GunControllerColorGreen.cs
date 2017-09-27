using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerColorGreen : MonoBehaviour
{
    //public
    //public Transform gunPivot; // pivot da arma para rotacao
    public GameObject grenadePrefab; //bala (para teste)
    //public GameObject laserPrefab; //laser (para teste)

    public float fireRate; //frequencia per sec
    public float bulletSpeed; //da pra setar speed, mas para mina o speed deve ser zero
    public float grenadeLifeTime; //tempo de vida da mina

    public int bulletDamage; //dano da bala

    public int grenadesMaxQuantity; //quantidade maxima de minas da tela

    public AudioClip shootSound; //som do tiro disparando

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject player; //usuario da arma

    private float tempo;
    //private Vector2 playerPosition;
    //private bool bulletX; //tipo da bala que vai atirar (tiro/laser, para teste)

    private List<GameObject> grenadesOnScreen; //granadas na tela

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
        grenadesOnScreen = new List<GameObject>(); //inicializar
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
        audioSource.PlayOneShot(shootSound); //tocar som de acertar

        //criar bullet
        GameObject bullet = Instantiate(grenadePrefab, bulletSpawn.position, bulletSpawn.rotation);

        VerifyGrenadesOnScreen(bullet); //verifica e disponibiliza espaço para outra granada/mina caso necessario e adiciona bala

        bullet.layer = LayerMask.NameToLayer("PlayerBullet"); //informar layer da bala

        GrenadeHealth grenadeH = bullet.GetComponent<GrenadeHealth>(); //pegar script de vida da bala
        if (grenadeH != null) //se conseguiu
        {
            grenadeH.source = "Character"; //informar fonte da bala
            grenadeH.explosionDamage = bulletDamage; //informar dano da bala

            grenadeH.isFreezingBullet = true; //informar que congela/ou queima (isFreeezingBullet/isBurningBullet)
        }

        //velocidade da bala (enviar funcao)
        bullet.gameObject.SendMessageUpwards("SetVelocity", bulletSpeed, SendMessageOptions.DontRequireReceiver); //enviar velocidade, nao requisitar erro caso nao encontre

        //destruir bullet apos x segundos
        //Destroy(bullet, grenadeLifeTime); //tempo de vida da bullet
        bullet.gameObject.SendMessageUpwards("DestroyByTime", grenadeLifeTime, SendMessageOptions.DontRequireReceiver); //enviar tempo para destruir, nao requisitar erro caso nao encontre

    }

    public void VerifyGrenadesOnScreen(GameObject bullet) //limpa e verifica quantos objetos existem na lista (e remove o primeiro colocado caso necessario para colocar outro) e coloca o outro
    {
        if (bullet == null) return;

        grenadesOnScreen.RemoveAll(item => item == null); //remover todos os nulos
        //foreach (GameObject gameO in grenadesOnScreen)
        //{ //limpar
        //    if (gameO == null)
        //    {
        //        grenadesOnScreen.Remove(gameO);
        //    }
        //}

        if (grenadesOnScreen.Count == grenadesMaxQuantity)
        {
            Destroy(grenadesOnScreen[0]); //remover/destruir primeiro cara na lista caso cheia
            //grenadesOnScreen.RemoveAt(0);
        }

        grenadesOnScreen.Add(bullet); //balas na tela

    }
}
