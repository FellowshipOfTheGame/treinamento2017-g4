using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerGrenadeLauncher : MonoBehaviour
{
    //public
    //public Transform gunPivot; // pivot da arma para rotacao
    public GameObject grenadePrefab; //bala (para teste)
    //public GameObject bulletPrefab; //bala (para teste)
    //public GameObject laserPrefab; //laser (para teste)

    public float fireRate; //frequencia per sec
    public float bulletSpeed;
    public float grenadeLifeTime; //tempo antes de explodir

    public int bulletDamage; //dano da bala

    public AudioClip shootSoundBurn; //som do tiro disparando

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject player; //usuario da arma

    private float tempo;
    private Vector2 playerPosition;
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
            tempo += Time.deltaTime;
            if (Input.GetButton("Fire1") && tempo > (1 / fireRate)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
            {
                tempo = 0f;

                Fire();
            }

            //if (Input.GetKeyDown(KeyCode.Space)) //mudar pra laser        {
            //{
            //    if (bulletX) bulletX = false;
            //    else bulletX = true;
            //}
        }
    }

    private void FixedUpdate()
    {
        playerPosition.Set(player.transform.position.x, player.transform.position.y); //pegar posicao corrente da camera (Camera.main.transform.position), pegar pos do player

        Gun_Turning();
    }

    void Fire() //atirar
    {
        audioSource.PlayOneShot(shootSoundBurn); //tocar som de acertar

        //criar bullet
        GameObject bullet = Instantiate(grenadePrefab, bulletSpawn.position, bulletSpawn.rotation);

        bullet.layer = LayerMask.NameToLayer("PlayerBullet"); //informar layer da bala

        GrenadeHealth grenadeH = bullet.GetComponent<GrenadeHealth>(); //pegar script de vida da bala
        if (grenadeH != null) //se conseguiu
        {
            grenadeH.source = "Character"; //informar fonte da bala
            grenadeH.explosionDamage = bulletDamage; //informar dano da bala
        }

        //velocidade da bala (enviar funcao)
        bullet.gameObject.SendMessageUpwards("SetVelocity", bulletSpeed, SendMessageOptions.DontRequireReceiver); //enviar velocidade, nao requisitar erro caso nao encontre

        //destruir bullet apos x segundos
        //Destroy(bullet, grenadeLifeTime); //tempo de vida da bullet
        bullet.gameObject.SendMessageUpwards("DestroyByTime", grenadeLifeTime, SendMessageOptions.DontRequireReceiver); //enviar tempo para destruir, nao requisitar erro caso nao encontre

    }

    //Girar a arma
    void Gun_Turning()
    {
        float angleDegrees = Gun_MouseToScreenCenter() - 90; //pegar rotacao do mouse - a orientacao inicial em relacao a base (up->0) (inicial->right->90)
        //Debug.Log(angleDegrees);
        //gunPivot.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees));
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees));
    }

    //Pega o angulo em graus entre a posicao do mouse e o centro da tela (começa em cima==> 0, sentido horario direita==> 90)
    float Gun_MouseToScreenCenter() //"girar"
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //posicao do mouse

        //pegar angulo
        Vector2 newVector = worldPoint - playerPosition; //pegar vetor do ponto ao mouse

        float angleRadians = Mathf.Atan2(newVector.x, newVector.y); //pegar angulo em radianos

        float angleDegrees = angleRadians * Mathf.Rad2Deg; //converter para graus (-180, 180)
        if (angleDegrees < 0) angleDegrees += 360.0f; //normalizar (0, 360) "359.9_" (0 up, 90 right, 180 down, 270 left)

        //Debug.Log(angleDegrees);
        //Animating(angleDegrees);
        return angleDegrees;
    }
}
