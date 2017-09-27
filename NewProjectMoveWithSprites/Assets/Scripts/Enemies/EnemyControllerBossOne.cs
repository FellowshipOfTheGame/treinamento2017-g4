using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyControllerBossOne : MonoBehaviour
{
    //public
    public GameObject target;

    public float speed; //velocidade
    public float range; //range maxima

    public List<Vector3> points; //proximos pontos (para reusar dar clear antes)
    public bool canMove; //caso possa se mexer ou nao (inimigo parado)

    public GameObject weaponPrefab; //prefab da arma que ele vai usar
    public int quantityOfWeapons; //quantas vai usar

    public bool isGunsEqualDistribution; //distribuicao das armas em torno dele (se eh uniforme ou n)
    //[HideInInspector] //esconder para customizar no editor da unity
    public float distanceBetweenGuns; //se n distribuicao das armas, distancia entre elas
                                      // [HideInInspector]
    public float initialAngle; //distancia inicial para comecar a colocar as armas

    public bool fireAllTogether; //atirar todas juntas?

    public bool canBeImmune; //pode ficar imune a dano?
                             //  [HideInInspector]
    public float timeReceiveDamage; //tempo pra receber dano
                                    // [HideInInspector]
    public float timeImmune; //tempo pra nao receber dano
                             //  [HideInInspector]
    public Color colorReceiveDamage; //cor pra receber dano
                                     //  [HideInInspector]
    public Color colorImmune; //cor pra receber dano

    public float moveRightDistance; //distancia para se mexer para a direita

    public AudioClip vunerableSound; //som do boss ficando vulneravel

    private Text youWinText;
    private Text youWinTextReturn;

    //private
    private Rigidbody2D rb2d;
    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;

    private bool moveToNext;

    private Vector3 nextPoint;
    private int indexAux;

    private Animator anim;

    private bool changeSpeedRunning; //para nao mudar de novo enquanto rodando

    private List<GameObject> weapons; //lista para quando criar armas
    private float[] tempo; //vetor de tempos pra cada arma atirar
    private bool[] isPrepFire; //verificar se ja esta tentando atirar

    //private float damageTime; //tempo de receber/n receber dano

    private AudioSource audioSource; //componente de audio

    private bool isQuitting; //para caso esteja saindo da aplicacao nao instanciar novos objetos no onDestroy

    // Use this for initialization
    void Start()
    {
        isQuitting = false;
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody
        enemyHealth = gameObject.GetComponent<EnemyHealth>(); //pegar rigdbody
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); //pegar rigdbody
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        colorImmune = spriteRenderer.color; //guardar cor atual (qualquer coisa mudar)

        Vector3 initialPoint = gameObject.transform.position; //default
        //Vector3 finalPoint = gameObject.transform.position + new Vector3(10f, 0f, 0f);

        points.Add(initialPoint);
        //points.Add(finalPoint);
        //points.Add(finalPoint + new Vector3(-10f, 0f, 0f));
        /*Vector3 finalPoint = */
        FindFinalPoint();

        moveToNext = true;
        indexAux = 0;

        CreateWeapons(); //criar armas
        if (canBeImmune) StartCoroutine(StartDamage()); //receber ou nao dano //se puder ficar imune a dano
    }

    public void FindFinalPoint()
    {
        switch (gameObject.name)
        {
            case "Enemy_BossOne":
                //do nothing
                break;
            case "Enemy_BossTwo":
                Vector3 finalPoint = gameObject.transform.position;

                Vector2 rayDirection2D = (transform.right).normalized; //direcao pra lancar raycast
                finalPoint = (Vector2)gameObject.transform.position + (rayDirection2D * moveRightDistance); //pegar ponto final e arrumar distancia maxima

                points.Add(finalPoint);
                break;
            default:
                Debug.Log("Boss not found!");
                break;
        }
    }

    public IEnumerator StartDamage() //verifica se oo boss pode ou nao tomar dano por tempos
    {
        spriteRenderer.color = colorImmune;
        enemyHealth.cantTriggerDamage = true; //nao pode receber dano
        yield return new WaitForSeconds(timeImmune);

        audioSource.PlayOneShot(vunerableSound); //tocar som de ficar vulneravel
        spriteRenderer.color = colorReceiveDamage;
        enemyHealth.cantTriggerDamage = false; //pode receber dano
        yield return new WaitForSeconds(timeReceiveDamage > vunerableSound.length ? timeReceiveDamage : vunerableSound.length);

        if (gameObject != null) StartCoroutine(StartDamage()); //se o objeto ainda existir, fazer procedimento de novo
    }

    public void CreateWeapons()
    { //cria as armas do boss
        weapons = new List<GameObject>(); //criar lista
        tempo = new float[quantityOfWeapons];
        isPrepFire = new bool[quantityOfWeapons];

        float angleDifference = isGunsEqualDistribution ? (360 / (float)quantityOfWeapons) : distanceBetweenGuns; //escolher metodo de districuicao das armas 

        for (int i = 0; i < quantityOfWeapons; i++)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, gameObject.transform);
            newWeapon.name = weaponPrefab.name + "_" + i; //arrumar nome

            FixNewWeaponRotation(newWeapon, (i * angleDifference) + initialAngle); //arrumar rotacao de cada arma
            weapons.Add(newWeapon); //adicionar nova arma ao vetor de armas
        }
    }

    public void FixNewWeaponRotation(GameObject newWeapon, float newAngleDegrees)
    { //arruma a rotacao de cada arma para o desejado
        switch (weaponPrefab.name)
        {
            case "BossArm":
                GunControllerEnemyBossArm script = newWeapon.transform.GetChild(1).GetComponent<GunControllerEnemyBossArm>();
                if (script != null)
                {
                    script.Gun_Turning(newAngleDegrees); //manda rodar a partir de cima no sentido horario x graus
                }
                else
                {
                    Debug.Log("Script do BossArm nao encontrado!");
                }
                break;
            case "BossGunYellow":
                GunControllerEnemyBossGunYellow script2 = newWeapon.transform.GetChild(0).GetComponent<GunControllerEnemyBossGunYellow>();
                if (script2 != null)
                {
                    script2.Gun_Turning(newAngleDegrees); //manda rodar a partir de cima no sentido horario x graus
                }
                else
                {
                    Debug.Log("Script do BossGunYellow nao encontrado!");
                }
                break;
            default:
                Debug.Log("Boss Weapon not found!");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < quantityOfWeapons; i++)
        {//pra cada arma tentar atirar //mexer no tempo
            tempo[i] += Time.deltaTime;
            if (!fireAllTogether) FindWeaponAndTryFire(weapons[i], i); //atirar arma por arma
        }

        if (fireAllTogether) FindWeaponAndTryFireAllTogether(); //atirar todas juntas (braco, etc)
    }

    public void FindWeaponAndTryFire(GameObject newWeapon, int i)
    { //encontra arma e tenta atirar cada uma sozinha

        switch (weaponPrefab.name)
        {
            case "BossArm":
                GunControllerEnemyBossArm script = newWeapon.transform.GetChild(1).GetComponent<GunControllerEnemyBossArm>();
                if (script != null)
                {
                    //TryFire(script.fireRate); //tentar atirar //tentar atirar de acordo com o fireRate
                    if (tempo[i] > (1 / script.fireRate)) //tentar atirar //tiros por seg permitidos
                    {
                        tempo[i] = 0f;

                        if (!script.GetIsFiring() && !(isPrepFire[i]))
                        {
                            StartCoroutine(PrepareToFire(newWeapon, i));
                        }
                        //script.Fire();
                    }
                }
                else
                {
                    Debug.Log("Script do BossArm nao encontrado! (Fire)");
                }
                break;
            case "BossGunYellow":
                break;
            default:
                Debug.Log("Boss Weapon not found!");
                break;
        }
    }

    public IEnumerator PrepareToFire(GameObject newWeapon, int i) //preparar pra atirar e atirar
    {
        isPrepFire[i] = true;

        switch (weaponPrefab.name)
        {
            case "BossArm":
                GunControllerEnemyBossArm script = newWeapon.transform.GetChild(1).GetComponent<GunControllerEnemyBossArm>();
                if (script != null)
                {
                    //Debug.Log(newWeapon.name + ": " + newWeapon.transform.GetChild(1).transform.eulerAngles);
                    //script.Gun_Turning(((-newWeapon.transform.GetChild(1).transform.eulerAngles.z) + 90) + 5f);
                    script.Gun_TurningLerp(45f);
                    yield return new WaitForSeconds(1f);

                    while (script.GetIsRotating())
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(script.fireRatePlus); //esperar tempo a mais

                    script.Fire();

                    //while (!(!script.GetIsThrowingAndPulling() && !script.GetIsThrowing() && !script.GetIsPulling()))
                    //{
                    //    yield return null;
                    //}
                }
                else
                {
                    Debug.Log("Script do BossArm nao encontrado! (Fire)");
                }
                break;
            case "BossGunYellow":
                break;
            default:
                Debug.Log("Boss Weapon not found!");
                break;
        }

        isPrepFire[i] = false;
    }

    public void FindWeaponAndTryFireAllTogether()
    { //encontra arma e tenta atirar todas juntas
        switch (weaponPrefab.name)
        {
            case "BossArm":
                bool canFireAll = true;
                GunControllerEnemyBossArm[] scripts = new GunControllerEnemyBossArm[quantityOfWeapons];
                for (int i = 0; i < quantityOfWeapons; i++)
                {//pra cada arma pegar o script
                    scripts[i] = weapons[i].transform.GetChild(1).GetComponent<GunControllerEnemyBossArm>();
                    if (scripts[i] == null) //se nao encontrar um dos scripts
                    {
                        Debug.Log("Script do BossArm nao encontrado! (Fire)");
                        canFireAll = false; //nao atirar
                    }
                    else
                    { //se encontrar
                        if (!(tempo[i] > (1 / scripts[i].fireRate))) //tentar atirar //tiros por seg permitidos
                        { //se uma das armas nao estiver no tempo de atirar
                            canFireAll = false; //nao atirar
                            //Debug.Log("Here1");
                        }
                        else
                        {
                            if (!(!scripts[i].GetIsFiring() && !(isPrepFire[i])))
                            { //se uma das armas ainda esta atirando ou coisa do genero
                                canFireAll = false; //nao atirar
                                //Debug.Log("Here2");
                            }
                        }
                    }
                }
                if (canFireAll)
                { //atirar todas as armas juntas caso possivel
                    for (int i = 0; i < quantityOfWeapons; i++)
                    {//pra cada arma atirar
                        tempo[i] = 0f;
                        StartCoroutine(PrepareToFire(weapons[i], i));
                    }
                    //Debug.Log("HERE");
                }
                break;
            case "BossGunYellow":
                break;
            default:
                Debug.Log("Boss Weapon not found!");
                break;
        }
    }

    private void FixedUpdate()
    {
        //if (target != null) //se existir target
        //{
        if (canMove)//se puder se mexer
        {
            Move();
        }
        //}
    }

    public void Move()
    {
        float range;

        //if (target != null) range = Vector2.Distance(transform.position, target.transform.position); //caso exista target
        /*else */
        range = int.MaxValue; //caso nao continuar no caminho normal (voltar sempre ao mesmo ponto/soh ficar no caminho)

        Vector3 movement = Vector3.zero;

        if (range <= this.range) //verificar se ta dentro da range maxima
        {
            movement = new Vector3(((transform.position.x) - target.transform.position.x), //seguir player
                                      ((transform.position.y) - target.transform.position.y), 0.0f);
        }
        else //move back para o caminho de antes (wolf)
        {
            if (moveToNext)
            {
                if (indexAux >= points.Count) indexAux = 0; //zerar pra n sair da range
                nextPoint = points[indexAux++]; //pegar proximo ponto           

                moveToNext = false;
            }
            else
            {
                range = Vector2.Distance(transform.position, nextPoint);
                if (range >= 0.5f) //andar ate range do ponto
                {
                    movement = new Vector3(((transform.position.x) - nextPoint.x),
                                             ((transform.position.y) - nextPoint.y), 0.0f);
                }
                else moveToNext = true;
            }
        }
        //else //caso nao esteja na range
        //{
        //    rb2d.velocity = Vector2.zero; //zerar velocidade 
        //}

        if (movement.magnitude > 1)
        {
            movement = movement.normalized; //normalizar movimento caso "velocidade"/magnitude seja maior que a maxima (1) 
        }

        movement = movement * speed * Time.deltaTime; //pegar e normalizar velocidade
        movement = -movement;

        Move_Animating(movement); //animacao

        rb2d.MovePosition(transform.position + movement); //outra maneira de mover
                                                          //rb2d.velocity = movement; //setar velocidade

    }

    void Move_Animating(Vector3 movement) //animacoes -> andando/parado
    {
        if (movement.x >= 0) // 30
        {
            anim.SetBool("Left", false); //virar pra direita
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("RightEnemyBossOne");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("RightEnemyBossOne");
        }

        bool walking = movement.x != 0 || movement.y != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        Debug.Log("RightEnemyBossOne" + walking);
    }

    public void ChangeSpeedTemporarily(float[] vector) //(0 speedTime) (1 newSpeed em porcentagem do speed antigo) //muda a velocidade do objeto por tempo determinado
    {
        if (!changeSpeedRunning) StartCoroutine(SpeedTemporarily(vector[0], vector[1])); //verificar se nao esta rodadando antes de comecar
    }

    public IEnumerator SpeedTemporarily(float speedTime, float newSpeed)
    {
        changeSpeedRunning = true; //para garantir que nao vai parar 2 vezes

        float previousSpeed = speed; //armazenar e zerar a velocidade
        speed = newSpeed * speed; //0f para parar, etc

        yield return new WaitForSeconds(speedTime);

        speed = previousSpeed; //retornar a velocidade ao normal

        changeSpeedRunning = false;
    }

    private void OnDestroy()
    {
        if (!isQuitting) //se nao estiver saindo
        {
            if (gameObject.name == "Enemy_BossTwo")
            {
                //SceneManager.LoadScene(0);
                youWinText = GameObject.Find("Player").gameObject.GetComponent<PlayerHealth>().youWinText;
                youWinTextReturn = GameObject.Find("Player").gameObject.GetComponent<PlayerHealth>().youWinTextReturn;
                youWinText.gameObject.SetActive(true);
                youWinTextReturn.gameObject.SetActive(true);
            }
        }
    }

    //quando estiver saindo da aplicacao
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

}

////criar um editor customizado para mostrar variavel quando selecionado
//[CustomEditor(typeof(EnemyControllerBossOne))]
//[CanEditMultipleObjects]
//public class EnemyControllerBossOne_Editor : Editor
//{
//    public override void OnInspectorGUI()
//    {

//        DrawDefaultInspector(); // for other non-HideInInspector fields

//        EnemyControllerBossOne script = (EnemyControllerBossOne)target;

//        // draw checkbox for the bool
//        script.isGunsEqualDistribution = EditorGUILayout.Toggle("Is Guns Equal Distribution", script.isGunsEqualDistribution);
//        if (script.isGunsEqualDistribution) // if bool is true, show other fields
//        {
//            script.distanceBetweenGuns = EditorGUILayout.FloatField("Distance Between Guns", script.distanceBetweenGuns);
//        }
//    }
//}