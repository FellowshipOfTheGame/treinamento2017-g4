using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerEnemyBossArm : MonoBehaviour
{
    //public
    //public Transform gunPivot; // pivot da arma para rotacao
    public float fireRate; //frequencia per sec

    public float timeDelayThrow; //tempo de delay do resize pra lancar
    public float sizeDifferenceThrow; //tamanho por tempo de delay pra lancar

    public float timeDelayPull; //tempo de delay do resize pra puxar
    //public float sizeDifferencePull; //tamanho por tempo de delay pra puxar

    public float maxRange; //range maxima do resize
    public float timeBetweenActions; //tempo entre jogar/puxar

    public bool stopThrowing;

    public bool canFire; //para verificar se pode atirar
    public float fireRatePlus; //adicao ao tempo final de shoot
    public float timeDelayRot; //tempo de delay para girar

    //private
    private Transform bulletSpawn; //posicao de spawn das balas
    private Transform androidHand; //posicao da mao do android
    private GameObject user; //usuario da arma

    private float tempo;
    private Vector2 userPosition; //posicao do usuario da arma
    //private bool bulletX; //tipo da bala que vai atirar (tiro/laser, para teste)

    private Vector3 originalRange; //range original/minima do resize
    private bool isThrowing; //se esta jogando/puxando (dando resize) 
    private bool isPulling;
    private bool isThrowingAndPulling;

    private bool isRotating; //verificar se ja esta rodando
    private bool isFiring; //verificar se esta atirando

    // Use this for initialization
    void Start()
    {
        //GameObject bulletSpawnn = GameObject.Find("BulletSpawn"); //posicionar a mao do android no fim da arma
        androidHand = gameObject.transform.parent.GetChild(0); //pegar hand
        bulletSpawn = gameObject.transform.GetChild(0); //pegar bullet spawn

        BoxCollider2D gunBoxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        float initialPosX = transform.localPosition.x + gunBoxCollider2d.size.x;
        Vector3 bulletSpawnPos = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);// + transform.right;

        bulletSpawn.transform.localPosition = bulletSpawnPos;
        //bulletSpawn.SetPositionAndRotation(bulletSpawnPos, bulletSpawn.rotation);

        SetHandPositionAndRotation();

        user = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)

        //bulletX = false;
        tempo = 0f;
        originalRange = gameObject.transform.localScale; //guardar tamanho original
        isThrowing = false;
        isPulling = false;
        stopThrowing = false;
        isThrowingAndPulling = false;

        canFire = false;
        isRotating = false;
        isFiring = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetHandPositionAndRotation(); //re setar posicao da mao
        if (user != null)
        {
            canFire = true;
            tempo += Time.deltaTime;
            if (/*Input.GetButton("Fire1") && */tempo > (1 / fireRate)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
            {
                tempo = 0f;

                //    Fire();
            }
        }
    }

    public void SetHandPositionAndRotation() //setar posicao e rotacao da mao de acordo com o bulletSpawn do braco
    {
        androidHand.transform.position = bulletSpawn.transform.position;
        androidHand.transform.rotation = bulletSpawn.transform.rotation;
        //bulletSpawn.SetPositionAndRotation(bulletSpawnPos, bulletSpawn.rotation);
    }

    private void FixedUpdate()
    {
        userPosition.Set(user.transform.position.x, user.transform.position.y); //pegar posicao corrente da camera (Camera.main.transform.position), pegar pos do player

        //if (Input.GetKeyDown(KeyCode.Z)) //para teste
        //{
        //    Gun_Turning(180);
        //}
    }

    public void Fire() //atirar
    {
        if (canFire) //verificar se pode atirar
        {
            //comecar a esticar e puxar braco
            if (!isThrowingAndPulling && !isThrowing && !isPulling)//se nao estiver no meio de alguma das acoes, realizar acoes
            {
                isFiring = true;
                StartCoroutine(ThrowAndPull());
            }
        }
    }

    //--------------------------teste----------------------------------

    public IEnumerator ThrowAndPull()
    { //controlar jogar/puxar
        isThrowingAndPulling = true;

        if (!isThrowing && !isPulling) StartCoroutine(ScaleOverTimeThrow()); //se nao estiver jogando nem puxando

        StartCoroutine(TryPull());

        isThrowingAndPulling = false;

        yield return null;
    }

    public IEnumerator TryPull()
    { //tentar puxar
        if (isPulling)
        {
            yield return new WaitForSeconds(timeBetweenActions); //esperar tempo
            StartCoroutine(ScaleOverTimePull());
        }
        else
        {
            yield return new WaitForSeconds(0.5f); //esperar tempo
            StartCoroutine(TryPull());
        }
    }

    public IEnumerator ScaleOverTimeThrow()
    { //jogar braco
        isThrowing = true;

        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(originalScale.x + sizeDifferenceThrow, originalScale.y, originalScale.z);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / timeDelayThrow);
            currentTime += Time.deltaTime;
            yield return null;
            if (gameObject.transform.localScale.x >= maxRange) isThrowing = false; //se alcançar a range maxima parar de jogar
        } while (currentTime <= timeDelayThrow && isThrowing && !stopThrowing); //enquanto estiver jogando/nao acabar o tempo/maxScale

        if (gameObject.transform.localScale.x < maxRange && !stopThrowing)
        {
            StartCoroutine(ScaleOverTimeThrow()); //se nao estiver jogando nem puxando
        }
        else
        {
            isPulling = true;
            isThrowing = false; //resetar variaveis e comecar a puxar
            stopThrowing = false;
        }
    }

    public IEnumerator ScaleOverTimePull()
    { //puxar braco
        isPulling = true;

        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = originalRange; //new Vector3(originalScale.x - sizeDifferencePull, originalScale.y, originalScale.z); //scale original

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / timeDelayPull);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= timeDelayPull);

        isPulling = false;
        isFiring = false; //termionu de atirar
    }

    public bool GetIsThrowing() // pegar is Throwing
    {
        return isThrowing;
    }

    public bool GetIsPulling() // pegar is Pulling
    {
        return isPulling;
    }

    public bool GetIsThrowingAndPulling() // pegar is ThrowingAndPulling
    {
        return isThrowingAndPulling;
    }

    public bool GetIsRotating() // pegar is Rotating
    {
        return isRotating;
    }

    public bool GetIsFiring() // pegar is Firing
    {
        return isFiring;
    }

    private void OnTriggerStay2D(Collider2D other)
    { //se o braco estiver collidindo
        //int collisionMask = androidHand.GetComponent<GunControllerAndroidHand>().GetCollisionMask();
        //if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        //{
        //    if (GetIsThrowing())
        //    {
        //        stopThrowing = true;
        //    }
        //}
        //if (GetIsThrowing()) //enquanto estiver colidindo nao deixar jogar
        //{
        //    stopThrowing = true;
        //}
    }

    //----------------------mudar de script----------------------------

    //Girar a arma devagar
    public void Gun_TurningLerp(float angleDegrees)
    {
        //Quaternion oldRot = Quaternion.Euler(new Vector3(0f, 0f, (transform.rotation.eulerAngles.z + 270) % 360));
        //Quaternion newRot = Quaternion.Euler(new Vector3(0f, 0f, -(angleDegrees - 90)));
        //Quaternion.LerpUnclamped(oldRot, newRot, Time.deltaTime * speedRot);
        StartCoroutine(RotateOverTime(angleDegrees));
    }

    public IEnumerator RotateOverTime(float plusRotation)
    { //puxar braco
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

    //Pega o angulo em graus entre a posicao do mouse e o centro da tela (começa em cima==> 0, sentido horario direita==> 90)
    //float Gun_UserToAngle() //"girar"
    //{
    //    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //posicao do mouse

    //    //pegar angulo
    //    Vector2 newVector = worldPoint - userPosition; //pegar vetor do ponto ao mouse

    //    float angleRadians = Mathf.Atan2(newVector.x, newVector.y); //pegar angulo em radianos

    //    float angleDegrees = angleRadians * Mathf.Rad2Deg; //converter para graus (-180, 180)
    //    if (angleDegrees < 0) angleDegrees += 360.0f; //normalizar (0, 360) "359.9_" (0 up, 90 right, 180 down, 270 left)

    //    //Debug.Log(angleDegrees);
    //    //Animating(angleDegrees);
    //    return angleDegrees;
    //}
}
