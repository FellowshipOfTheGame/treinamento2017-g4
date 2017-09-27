using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerWasp : MonoBehaviour
{
    //public
    public GameObject target;

    public float speed; //velocidade
    public float range; //range maxima
    //public List<Vector3> points; //proximos pontos (para reusar dar clear antes)

    //private
    private Rigidbody2D rb2d;

    //private float range; //"range maxima"
    private bool moveToNext;
    private Vector3 nextPoint; //proximo ponto
    private Vector3 currentPoint; //ponto corrente (pra fazer a vespa "tremer" no lugar)
    //private int indexAux;

    private Animator anim;

    private bool changeSpeedRunning; //para nao mudar de novo enquanto rodando

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        //range = int.MaxValue - 1; //"definir range maxima" (quando entrar na range seguir)
        currentPoint = gameObject.transform.position;
        moveToNext = true;
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

            currentPoint = gameObject.transform.position; //guardar posicao corrente do inimigo
            nextPoint = currentPoint; //ajustar proxima posicao
            //Debug.Log("Current: " + currentPoint);
        }
        else //wader around his point/do nothing (wasp)
        {
            if (moveToNext)
            {
                nextPoint = ChooseNewPoint(); //pegar proximo ponto   
                //Debug.Log("Next: " + nextPoint);

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

    //pegar novo ponto (to wander around his point)
    public Vector3 ChooseNewPoint()
    {
        //Debug.Log("Position: " + gameObject.transform.position);
        //Debug.Log("Current: " + currentPoint);

        Vector3[] nxtPoints = new Vector3[5]; //vetor de proximos pontos possiveis

        nxtPoints[0] = new Vector3(currentPoint.x, currentPoint.y, currentPoint.z); ; //nao fazer nada, ir pra cima, ir pra baixo, direita, esquerda
        nxtPoints[1] = new Vector3(currentPoint.x + 1f, currentPoint.y, currentPoint.z);
        nxtPoints[2] = new Vector3(currentPoint.x - 1f, currentPoint.y, currentPoint.z);
        nxtPoints[3] = new Vector3(currentPoint.x, currentPoint.y + 1f, currentPoint.z);
        nxtPoints[4] = new Vector3(currentPoint.x, currentPoint.y - 1f, currentPoint.z);

        int nextPointsAux = Random.Range(0, 4); //random entre 0 e 4 inteiro

        Vector3 nxtPoint = nxtPoints[nextPointsAux]; //escolher proxima posicao
        return nxtPoint; //retornar proxima posicao
    }

    void Move_Animating(Vector3 movement) //animacoes -> andando/parado
    {
        if (movement.x >= 0) // 30
        {
            anim.SetBool("Left", false); //virar pra esquerda
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("RightEnemyWasp");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("LeftEnemyWasp");
        }

        bool walking = movement.x != 0 || movement.y != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        //Debug.Log("EnemyWasp" + walking);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        { //tornar wasp trigger
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        { //tornar wasp trigger
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false; //sair do trigger
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false; //sair do trigger
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
