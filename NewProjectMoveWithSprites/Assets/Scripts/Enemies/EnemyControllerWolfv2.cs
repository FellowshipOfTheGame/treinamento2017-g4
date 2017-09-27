using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerWolfv2 : MonoBehaviour //mover e animar
{
    //public
    public GameObject target;

    public float speed; //velocidade
    public float range; //range maxima

    public List<Vector3> points; //proximos pontos (para reusar dar clear antes)

    public float maxPathRange; //range maxima do caminho que quer encontrar

    //private
    private Rigidbody2D rb2d;

    private bool moveToNext;

    private Vector3 nextPoint;
    private int indexAux;

    private Animator anim;

    private bool changeSpeedRunning; //para nao mudar de novo enquanto rodando

    private float pathRangeFix; //arrumar range maxima do caminho
    private int collisionMask; //colisoes possiveis do raycast

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("TestLayerMap") | 1 << LayerMask.NameToLayer("CameraWal") | 1 << LayerMask.NameToLayer("CameraCol"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)

        Bounds b = gameObject.GetComponent<SpriteRenderer>().sprite.bounds; //isso deu trabalho pra encontrar '-'-' .-.-.
        float size = b.size.x > b.size.y ? b.size.x : b.size.y; //pegar maior entre eles
        size = (size == b.size.x) ? (gameObject.transform.localScale.x * size) : (gameObject.transform.localScale.y * size);
        pathRangeFix = size; //range de ajuste 
        //Debug.Log("pathWolfFix: " + pathRangeFix);

        Vector3 initialPoint = gameObject.transform.position; //default
        //Vector3 finalPoint = gameObject.transform.position + new Vector3(0f, 10f, 0f);
        Vector3 finalPoint = FindFinalPoint();

        points.Add(initialPoint);
        points.Add(finalPoint);
        //points.Add(finalPoint + new Vector3(-10f, 0f, 0f));

        moveToNext = true;
        indexAux = 0;
        changeSpeedRunning = false;
    }

    public Vector3 FindFinalPoint()
    {
        Vector3 finalPoint = gameObject.transform.position;

        int startAngle = 0; //angulo inicial
        int finishAngle = 360; //angulo final
        int segments = 360; //quantidade de segmentos para dividir
        int increment = (int)(finishAngle / segments);

        for (int i = startAngle; i < finishAngle; i += increment)
        { //verificar todos os angulos possiveis
            Vector2 rayDirection2D = (Quaternion.Euler(0, 0, i) * transform.up).normalized; //direcao pra lancar raycast
            Vector3 newPoint = DoLaser(rayDirection2D); //fazer raycast e pegar ponto de colisao

            //Debug.DrawLine(gameObject.transform.position, newPoint, Color.red, 100f); //para teste
            if (Vector3.Distance(newPoint, gameObject.transform.position) > Vector3.Distance(finalPoint, gameObject.transform.position))
            { //caso a nova distancia seja maior
                finalPoint = newPoint; //substituir
            }
        }

        return finalPoint;
    }

    public Vector3 DoLaser(Vector3 direction) //faz um raycast na direcao da arma pra ver se acerta algo (encontrar caminhos possiveis)
    {
        Vector3 endPoint = Vector3.zero;
        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        //Vector3 direction = (target.transform.position - bulletSpawnTransform.position).normalized; //direcao para verificar
        Ray2D ray = new Ray2D(gameObject.transform.position, direction); //criar raio da posicao na direcao da seta vermelha (do mundo) 

        pontoColisao = Physics2D.Raycast(ray.origin, direction, maxPathRange, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character
        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //endPoint = pontoColisao.point;
            endPoint = ray.origin + (ray.direction * (Vector3.Distance(pontoColisao.point, gameObject.transform.position) - pathRangeFix)); //pegar ponto final e arrumar distancia maxima
        }
        else //se nao ocorreu colisao
        {
            endPoint = ray.origin + (ray.direction * (maxPathRange - pathRangeFix)); //arrumar distancia maxima
        }

        return endPoint; //retornar ponto de colisao/final
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //if (target != null) //se existir target
        //{
        Move();
        //}
    }

    public void Move()
    {
        float range;

        if (target != null) range = Vector2.Distance(transform.position, target.transform.position); //caso exista target
        else range = int.MaxValue; //caso nao continuar no caminho normal

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
            anim.SetBool("Left", false); //virar pra esquerda
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("RightEnemyWolf");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("LeftEnemyWolf");
        }

        bool walking = movement.x != 0 || movement.y != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        //Debug.Log("EnemyWolf" + walking);
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
}
