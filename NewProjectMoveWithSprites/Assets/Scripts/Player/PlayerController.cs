using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour //mover e animar //atirar
{
    //publico
    public float speed; //velocidade player
    //public Camera cam;

    //public float knockbackForce; //forca com a qual o player leva knockback de inimigos

    //privado
    private Rigidbody2D rb2d;
    private Animator anim;

    //private int floorMask;
    //private float camRayLenght = 100f;
    private Vector3 movement;
    private Vector2 playerPosition;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody e animator
        anim = gameObject.GetComponent<Animator>(); //descomentar os anims quando juntar com animacoes/animators

        //floorMask = LayerMask.GetMask("Background"); //criar uma mascara para o chao/bg do cenario
        //cameraPosition.Set(Camera.main.transform.position.x, Camera.main.transform.position.y); //pegar posicao da camera
    }

    // Use this for initialization
    void Start()
    {
        //Probability prob = new Probability(4, new int[] { 10, 50, 20, 5 }); //teste das probabilidades
        //prob.ProbabilityVector_Imprimir();
        //Debug.Log("P1: " + prob.ProbabilityVector_ChooseOne());
        //Debug.Log("P2: " + prob.ProbabilityVector_ChooseOne());
        //Debug.Log("P3: " + prob.ProbabilityVector_ChooseOne());
        //Debug.Log("P4: " + prob.ProbabilityVector_ChooseOne());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //fisica
    private void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical"); //pegar inputs da movimentacao
        float moveHorizontal = Input.GetAxis("Horizontal");
        //playerPosition.Set(Camera.main.transform.position.x, Camera.main.transform.position.y); //pegar posicao corrente da camera
        playerPosition.Set(transform.position.x, transform.position.y); //pegar posicao corrente do player

        //move the character
        Move(moveHorizontal, moveVertical);

        //turn the character //rotate
        float angleDegrees = Move_MouseToScreenCenter();

        //Animating the character
        Move_Animating(angleDegrees, moveHorizontal, moveVertical);
    }

    void Move(float moveHorizontal, float moveVertical) //movimentar
    {
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f); //transladar no cenario

        //Vector2 test = movement;
        //Debug.Log("Mag1: " + test.magnitude);

        if (movement.magnitude > 1)
        {
            movement = movement.normalized; //normalizar movimento caso "velocidade"/magnitude seja maior que a maxima (1) 
        }

        //movement = movement.normalized * speed * Time.deltaTime; //normaliza para nao se mexer mais rapido na diagonal, velocidade, e sincornizar por tempo/seg
        movement = movement * speed * Time.deltaTime;

        //test = movement;
        //Debug.Log("Mag2: " + test.magnitude);
        //movement = movement * speed * Time.deltaTime;

        //gameObject.transform.Translate(movement);
        rb2d.MovePosition(transform.position + movement); //ajuda a evitar erros de colisao
        //gameObject.transform.Translate(movement);
        //rb2d.velocity = movement;
    }

    //Pega o angulo em graus entre a posicao do mouse e o centro da tela (começa em cima==> 0, sentido horario direita==> 90)
    float Move_MouseToScreenCenter() //"girar"
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pegar pos do mouse

        //pegar angulo
        Vector2 newVector = worldPoint - playerPosition; //pegar vetor do ponto ao mouse

        float angleRadians = Mathf.Atan2(newVector.x, newVector.y); //pegar angulo em radianos

        float angleDegrees = angleRadians * Mathf.Rad2Deg; //converter para graus (-180, 180)
        if (angleDegrees < 0) angleDegrees += 360.0f; //normalizar (0, 360) "359.9_" (0 up, 90 right, 180 down, 270 left)

        //Debug.Log(angleDegrees);
        //Animating(angleDegrees);
        return angleDegrees;
    }

    void Move_Animating(float angle, float moveHorizontal, float moveVertical) //animacoes -> andando/parado
    {
        if (angle <= 180) // 30
        {
            anim.SetBool("Left", false); //virar pra esquerda
            anim.SetBool("Right", true); //virar pra direita
            //Debug.Log("Right");
        } //180 --> 360
        else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
        {
            anim.SetBool("Right", false); //virar pra esquerda
            anim.SetBool("Left", true); //virar pra esquerda
            //Debug.Log("Left");
        }

        bool walking = moveHorizontal != 0 || moveVertical != 0; //se estiver se movendo em alguma direcao = true, senao = false

        anim.SetBool("IsWalking", walking);
        //Debug.Log(walking);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if (other.gameObject.CompareTag("Enemy")) //se for um inimigo //movel
        //{
        //    Vector3 direct = other.contacts[0].point - (Vector2) transform.position; //calcular angulo entre o ponto de colisao e o player

        //    direct = -direct.normalized; //normalizar e inverter ponto de colisao
        //    //Debug.Log("saida");
        //    rb2d.AddForce(direct * knockbackForce); //adicionar forca de knockback e direcao para empurrar o player
        //}
    }

}

//===========================================================================================================
//
//void Move_Animating(float angle, float moveHorizontal, float moveVertical) //animacoes -> andando/parado
//{
//    if (angle <= 45) // 30
//    {
//        //anim.SetBool("Top", true); //virar pra cima
//        Debug.Log("Top");
//    }
//    else if (angle <= 135) // 60
//    {
//        //anim.SetBool("Right", true); //virar pra direita
//        Debug.Log("Right");
//    }
//    else if (angle <= 225) // 120
//    {
//        //anim.SetBool("Down", true); //virar pra cima
//        Debug.Log("Down");
//    }
//    else if (angle <= 315) // 150
//    {
//        //anim.SetBool("Left", true); //virar pra esquerda
//        Debug.Log("Left");
//    }
//    else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
//    {
//        //anim.SetBool("Top", true); //virar pra cima
//        Debug.Log("Top");
//    }

//    bool walking = moveHorizontal != 0 || moveVertical != 0; //se estiver se movendo em alguma direcao = true, senao = false

//    //anim.SetBool("IsWalking", walking);
//    Debug.Log(walking);
//}

//void Move_Animating(float angle, float moveHorizontal, float moveVertical) //animacoes -> andando/parado
//{
//    if (angle <= 22.5) // 30
//    {
//        //anim.SetBool("Top", true); //virar pra cima
//        Debug.Log("Top");
//    }
//    else if (angle <= 67.5) // 60
//    {
//        //anim.SetBool("TopRight", true); //virar pra cima direita
//        Debug.Log("TopRight");
//    }
//    else if (angle <= 112.5) // 120
//    {
//        //anim.SetBool("Right", true); //virar pra direita
//        Debug.Log("Right");
//    }
//    else if (angle <= 157.5) // 150
//    {
//        //anim.SetBool("DownRight", true); //virar pra baixo direita
//        Debug.Log("DownRight");
//    }
//    else if (angle <= 202.5) // 210
//    {
//        //anim.SetBool("Down", true); //virar pra baixo
//        Debug.Log("Down");
//    }
//    else if (angle <= 247.5) // 240
//    {
//        //anim.SetBool("DownLeft", true); //virar pra baixo esquerda
//        Debug.Log("DownLeft");
//    }
//    else if (angle <= 292.5) // 300
//    {
//        //anim.SetBool("Left", true); //virar pra esquerda
//        Debug.Log("Left");
//    }
//    else if (angle <= 337.5) // 330
//    {
//        //anim.SetBool("TopLeft", true); //virar pra cima esquerda
//        Debug.Log("TopLeft");
//    }
//    else //337.5 -> 0 -> 22.5 // 330 -> 0 -> 30
//    {
//        //anim.SetBool("Top", true); //virar pra cima
//        Debug.Log("Top");
//    }

//    bool walking = moveHorizontal != 0 || moveVertical != 0; //se estiver se movendo em alguma direcao = true, senao = false

//    //anim.SetBool("IsWalking", walking);
//    Debug.Log(walking);
//}
