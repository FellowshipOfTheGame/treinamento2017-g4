using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerSpider : MonoBehaviour //mover e animar
{
    //public
    //public GameObject target;

    public float speed; //velocidade
    public float maxRange; //range maxima

    //private
    private Rigidbody2D rb2d;

    private Vector3 randomPoint; //era gameObject, vector3 eh melhor

    private bool moveToNext;
    private float time;

    private Animator anim; //animacao

    private bool changeSpeedRunning; //para nao mudar de novo enquanto rodando

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        //randomPoint = new GameObject("randomPoint"); //criar randPoint object
        //finalPoint.transform.SetParent(gameObject.transform);
        NextRandomPoint(); //setar proximo ponto
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
        //StartCoroutine(Move());
    }

    public void NextRandomPoint()
    {
        float randomX = Random.Range(1f, maxRange); //gerar range em o minimo e o maximo (0.5f)
        float randomY = Random.Range(1f, maxRange); //e negativo ou positivo para a posicao do novo ponto
        float negativeX = Random.Range(-1, 1);
        float negativeY = Random.Range(-1, 1);
        negativeX = negativeX < 0 ? -1 : 1; //se for menor que zero -1 se n +1 
        negativeY = negativeY < 0 ? -1 : 1; //se for menor que zero -1 se n +1 

        randomPoint = gameObject.transform.position + new Vector3(randomX * negativeX, randomY * negativeY, 0f);
        //moveToNext = true;
    }

    public void Move()
    {
        float range = Vector2.Distance(transform.position, randomPoint); //randomPoint.transform.position

        Vector3 movement = Vector3.zero;

        if (moveToNext) //move para proximo ponto random (spider)
        {
            range = Vector2.Distance(transform.position, randomPoint);

            if (range >= 0.5f)
            {
                movement = new Vector3(((transform.position.x) - randomPoint.x),
                                         ((transform.position.y) - randomPoint.y), 0.0f);
            }
            else
            {
                //yield return new WaitForSeconds(1f); //esperar 1 sec pra se mexer de novo (return IEnumerator)
                moveToNext = false;
            }
        }
        else
        {
            //new WaitForSeconds(1f); //esperar 1 sec pra se mexer de novo
            NextRandomPoint(); //pegar proximo ponto
            moveToNext = true;
        }

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

    private void OnCollisionEnter2D(Collision2D other)
    {

    }

    //enquanto estiver colidindo
    private void OnCollisionStay2D(Collision2D other)
    {
        time += Time.deltaTime; //criar um tempo enquanto colide para limitar rand

        if (time >= 0.8f)
        {
            //se encostar em algo (criar limitacoes pra chegar no novo ponto)
            NextRandomPoint(); //pegar novo ponto
            moveToNext = false;
            time = 0f;
        }
    }

    void Move_Animating(Vector3 movement) //animacoes -> andando/parado
    {
        if (movement.x >= 0) // 30
        {
            anim.SetBool("Left", false); //virar pra esquerda
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("RightEnemySpider");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("LeftEnemySpider");
        }

        bool walking = movement.x != 0 || movement.y != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        //Debug.Log("EnemySpider" + walking);
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
