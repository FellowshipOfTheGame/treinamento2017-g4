using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerEnemyBossGunYellow : MonoBehaviour
{
    //public
    public GameObject electricOrbPrefab; //bala (para teste)

    public float fireRate; //frequencia per sec
    public float bulletSpeed; //velocidade
    public float bulletLifeTime; //tempo de vida da orb

    public int bulletDamage; //dano da bala

    public AudioClip shootSound; //som do tiro
    public AudioClip newHitSound; //som do tiro disparando

    public float timeDelayRot; //tempo de delay para girar

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject user; //usuario da arma

    private float tempo;
    //private Vector2 playerPosition;

    private AudioSource audioSource; //componente de audio

    private bool isRotating; //verificar se ja esta rodando

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

        user = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)

        //bulletX = false;
        tempo = 0f;
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (user != null)
        {
            tempo += Time.deltaTime; //Fire1 --> LeftClick //Fire2 --> RightClick //Fire3 --> MiddleClick
            if (/*Input.GetButton("Fire2") && */tempo > (1 / fireRate)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
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
        GameObject bullet = Instantiate(electricOrbPrefab, bulletSpawn.position, bulletSpawn.rotation);

        bullet.layer = LayerMask.NameToLayer("EnemyBullet"); //informar layer da bala

        ElectricOrbHealth bulletH = bullet.GetComponent<ElectricOrbHealth>(); //pegar script de vida da bala
        if (bulletH != null) //se conseguiu
        {
            bulletH.source = "Enemy"; //informar fonte da bala
            bulletH.bulletDamage = bulletDamage; //informar dano da bala

            bulletH.destroyDelay = bulletLifeTime;
            //bulletH.isFreezingBullet = true; //informar que congela/ou queima (isFreeezingBullet/isBurningBullet)

            bulletH.vanishSound = null; //remover som da bala qdo some
            //bulletH.vanishSound = newHitSound;
        }

        //velocidade da bala (enviar funcao)
        bullet.gameObject.SendMessageUpwards("SetVelocity", bulletSpeed, SendMessageOptions.DontRequireReceiver); //enviar velocidade, nao requisitar erro caso nao encontre

        //destruir bullet apos x segundos
        Destroy(bullet, bulletLifeTime); //tempo de vida da bullet

    }

    //Girar a arma devagar
    public void Gun_TurningLerp(float angleDegrees)
    {
        StartCoroutine(RotateOverTime(angleDegrees));
    }

    public IEnumerator RotateOverTime(float plusRotation)
    { //rodar devagar
        isRotating = true;

        Quaternion originalRot = transform.rotation; // Quaternion.Euler(new Vector3(0f, 0f, gameObject.transform.eulerAngles.z/* + 270) % 360*/));
        Quaternion destinationRot = Quaternion.Euler(new Vector3(0f, 0f, gameObject.transform.eulerAngles.z /*+ 270) % 360*/ + plusRotation));
        //Debug.Log("rotacao Orig: " + originalRot.eulerAngles);
        //Debug.Log("rotacao Dest: " + destinationRot.eulerAngles);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.rotation = Quaternion.Lerp(originalRot, destinationRot, currentTime / timeDelayRot);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= timeDelayRot);
        gameObject.transform.rotation = destinationRot; //fix final rotation

        isRotating = false;
    }

    //Girar a arma
    public void Gun_Turning(float angleDegrees)
    {
        //float angleDegrees = Gun_UserToAngle() - 90; //pegar rotacao do mouse - a orientacao inicial em relacao a base (up->0) (inicial->right->90)
        //Debug.Log(angleDegrees);
        //gunPivot.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees));
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -(angleDegrees - 90)));
    }

    public bool GetIsRotating() // pegar is Rotating
    {
        return isRotating;
    }
}
