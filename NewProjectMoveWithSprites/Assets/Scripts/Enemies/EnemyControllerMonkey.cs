using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerMonkey : MonoBehaviour, IGetTarget
{
    //public
    public GameObject target;

    public float speed; //velocidade
    public float range; //range maxima

    public List<Vector3> points; //proximos pontos (para reusar dar clear antes)
    public bool canMove; //caso possa se mexer ou nao (inimigo parado)

    //private
    private Rigidbody2D rb2d;

    private bool moveToNext;

    private Vector3 nextPoint;
    private int indexAux;

    private Animator anim;

    private bool changeSpeedRunning; //para nao mudar de novo enquanto rodando

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        Vector3 initialPoint = gameObject.transform.position; //default
        //Vector3 finalPoint = gameObject.transform.position + new Vector3(10f, 0f, 0f);

        points.Add(initialPoint);
        //points.Add(finalPoint);
        //points.Add(finalPoint + new Vector3(-10f, 0f, 0f));

        moveToNext = true;
        indexAux = 0;
    }

    // Update is called once per frame
    void Update()
    {

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
            anim.SetBool("Left", false); //virar pra esquerda
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("RightEnemyMonkey");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("LeftEnemyMonkey");
        }

        bool walking = movement.x != 0 || movement.y != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        //Debug.Log("EnemyMonkey" + walking);
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

    //pegar target do inimigo
    public GameObject GetTarget()
    {
        return target;
    }
}
