using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerEnemyMonkey : MonoBehaviour //generalizado pra mirar/atirar no target do user
{
    //public
    public GameObject bulletPrefab; //bala (para teste)

    public float fireRate; //frequencia per sec
    public float bulletSpeed; //velocidade da bala
    public float bulletLifeTime; //tempo de vida maximo da bala    

    public int bulletDamage; //dano da bala

    public float maxRange; //maxima range do tiro (para ver o player)

    //public float rotationSpeed; //velocidade de rotacao da arma

    public AudioClip shootSound; //som do tiro

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private GameObject target; //target da bala (em quem atirar)
    private GameObject user; //usuario da arma

    private float tempo;
    private Vector2 userPosition; //posicao do usuario da arma

    private bool canLookAtTarget; //verifica se da pra olhar pro player (sem obstaculo no meio)

    private int collisionMask; //colisoes possiveis do raycast

    private AudioSource audioSource; //componente de audio

    private EnemyControllerMonkey enemyControllerMonkey;
    private bool changeFireRateRunning; //para nao mudar de novo enquanto rodando

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        //GameObject bulletSpawnn = GameObject.Find("BulletSpawn"); 
        bulletSpawn = gameObject.transform.GetChild(0); //pegar bulletSpawn

        BoxCollider2D gunBoxCollider2d = gameObject.GetComponent<BoxCollider2D>();//posicionar bulletspawn no fim da arma
        float initialPosX = transform.localPosition.x + gunBoxCollider2d.size.x;
        Vector3 bulletSpawnPos = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);// + transform.right;

        bulletSpawn.transform.localPosition = bulletSpawnPos;
        //bulletSpawn.SetPositionAndRotation(bulletSpawnPos, bulletSpawn.rotation);

        user = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)
        //target = user.GetComponent<EnemyControllerMonkey>().target; // jeito 1 de pegar o target
        IGetTarget myRef = user.GetComponent(typeof(IGetTarget)) as IGetTarget; //jeito 2
        target = myRef.GetTarget();
        //Debug.Log("Target Name: " + target.name);

        enemyControllerMonkey = user.GetComponent<EnemyControllerMonkey>();

        tempo = 0f;
        canLookAtTarget = false;

        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy");//| 1 << LayerMask.NameToLayer("Character") ; //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)
    }

    // Update is called once per frame
    void Update()
    {
        if (user != null) //se existir o atirador
        {
            if (enemyControllerMonkey != null && enemyControllerMonkey.speed == 0) //se parar de se mexer
            {
                //Debug.Log("Changing Fire Rate Monkey");
                ChangeFireRateTemporarily(); //diminuir fireRate por tempo x enviar dano/"freeze", nao requisitar erro caso nao encontre
            }

            tempo += Time.deltaTime;

            if (canLookAtTarget && tempo > (1 / fireRate)) //atirar //tiros por seg permitidos se der pra olhar pro target
            {
                tempo = 0f;

                Fire();
            }
        }
    }

    private void FixedUpdate()
    {
        if (target != null) //se existir o alvo
        {
            userPosition.Set(user.transform.position.x, user.transform.position.y); //pegar posicao corrente da camera (Camera.main.transform.position), pegar pos do player

            canLookAtTarget = TryLookAtTarget(); //verificar se da pra olhar pro target e mirar nele

            if (canLookAtTarget && !changeFireRateRunning)
                Gun_Turning();
        }
        else canLookAtTarget = false; // se nao, nao atirar
    }

    public bool TryLookAtTarget() //verifica se eh possivel atingir o alvo ao mirar nele
    {
        return DoLaser(); //faz um raycast na direcao da arma pra ver se acerta algo (/o player), se nao acertar, nao atira
        //return true;
    }

    void Fire() //atirar
    {
        //if (bulletX) //mudar tipo de bala: raio//bala (para teste)

        audioSource.PlayOneShot(shootSound); //tocar som de atirar

        //criar bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        bullet.layer = LayerMask.NameToLayer("EnemyBullet"); //informar layer da bala

        //bullet.GetComponent<BulletHealth>().source = "Enemy"; //informar fonte da bala
        //bullet.GetComponent<BulletHealth>().bulletDamage = bulletDamage; //informar dano da bala
        BulletHealth bulletH = bullet.GetComponent<BulletHealth>(); //pegar script de vida da bala
        if (bulletH != null) //se conseguiu
        {
            bulletH.source = "Enemy"; //informar fonte da bala
            bulletH.bulletDamage = bulletDamage; //informar dano da bala
        }


        //velocidade da bala (enviar funcao)
        bullet.gameObject.SendMessageUpwards("SetVelocity", bulletSpeed, SendMessageOptions.DontRequireReceiver); //enviar velocidade, nao requisitar erro caso nao encontre

        //destruir bullet apos x segundos
        Destroy(bullet, bulletLifeTime); //tempo de vida da bullet
    }

    //Girar a arma
    void Gun_Turning()
    {
        float angleDegrees = Gun_EnemyToPlayer() - 90; //pegar posicao do inimigo - posicao do player (up->0) (inicial->right->90)
        //Debug.Log(angleDegrees);
        //gunPivot.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees));

        //Quaternion newRotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees)); //nova rotacao da arma
        //Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed); //rotacionar arma (antiga, nova, velocidade)

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees)); //rotacionar arma
    }

    //Pega o angulo em graus entre a posicao do enemy e do player (começa em cima==> 0, sentido horario direita==> 90)
    float Gun_EnemyToPlayer() //"girar"
    {
        Vector2 playerPosition = target.transform.position; //posicao do player

        //pegar angulo
        Vector2 newVector = playerPosition - userPosition; //pegar vetor do ponto ao mouse

        float angleRadians = Mathf.Atan2(newVector.x, newVector.y); //pegar angulo em radianos

        float angleDegrees = angleRadians * Mathf.Rad2Deg; //converter para graus (-180, 180)
        if (angleDegrees < 0) angleDegrees += 360.0f; //normalizar (0, 360) "359.9_" (0 up, 90 right, 180 down, 270 left)

        //Debug.Log(angleDegrees);
        //Animating(angleDegrees);
        return angleDegrees;
    }

    public bool DoLaser() //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        Transform bulletSpawnTransform = bulletSpawn.transform; //pegar transform
        //Vector3 pontoFinalLaser = bulletSpawnTransform.position + bulletSpawnTransform.right * maxRange; //pra frente vermelho ate distancia maxima

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Vector3 direction = (target.transform.position - bulletSpawnTransform.position).normalized; //direcao para verificar
        Ray2D ray = new Ray2D(bulletSpawnTransform.position, direction/*bulletSpawnTransform.right*/); //criar raio da posicao na direcao da seta vermelha (do mundo) 

        pontoColisao = Physics2D.Raycast(ray.origin, direction/*bulletSpawnTransform.right*/, maxRange, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //lightCollider.transform.position = pontoColisao.point;// - (Vector2)posLuz; //alterar fim do laser

            if (pontoColisao.transform.gameObject == target) return true;

            //Debug.Log(pontoColisao.transform.gameObject.name);
            //Debug.Log(startPos + "  " + scale.magnitude);
        }
        //else //se nao ocorreu colisao
        //{
        //    //lightCollider.transform.position = pontoFinalLaser; //alterar fim do laser
        //}

        return false; //nao acertou o player
    }

    public void ChangeFireRateTemporarily() //(0 speedTime) (1 newSpeed em porcentagem do speed antigo) //muda a velocidade do objeto por tempo determinado
    {
        if (!changeFireRateRunning) StartCoroutine(FireRateTemporarily()); //verificar se nao esta rodadando antes de comecar
    }

    public IEnumerator FireRateTemporarily()
    {
        changeFireRateRunning = true; //para garantir que nao vai parar 2 vezes

        float previousFireRate = fireRate; //armazenar e zerar a velocidade
        fireRate = 0 * fireRate; //0f para parar, etc

        yield return new WaitWhile(() => enemyControllerMonkey.speed == 0);//ForSeconds(speedTime); //esperar enquanto speed for zero
        //(a, b) => a + b This is roughly equivalent to:
        //delegate int(int a, int b) { return a + b; }

        fireRate = previousFireRate; //retornar a velocidade ao normal

        changeFireRateRunning = false;
    }
}
