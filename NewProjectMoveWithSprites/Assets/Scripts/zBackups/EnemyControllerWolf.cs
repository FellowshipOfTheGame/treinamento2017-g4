using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerWolf : MonoBehaviour
{
    //public
    public GameObject target;

    public float speed; //velocidade
    public float range; //range maxima

    //private
    private Rigidbody2D rb2d;

    private GameObject finalPoint;
    private GameObject initialPoint;

    private bool moveToFinal;

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar rigdbody

        finalPoint = new GameObject("finalPoint");
        //finalPoint.transform.SetParent(gameObject.transform);
        finalPoint.transform.position = gameObject.transform.position + new Vector3(0f, 10f, 0f);

        initialPoint = new GameObject("initialPoint");
        //initialPoint.transform.SetParent(gameObject.transform);
        initialPoint.transform.position = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (target != null) //se existir target
        {
            Move();
        }
    }

    public void Move()
    {
        float range = Vector2.Distance(transform.position, target.transform.position);

        Vector3 movement = Vector3.zero;

        if (range <= this.range) //verificar se ta dentro da range maxima
        {
            movement = new Vector3(((transform.position.x) - target.transform.position.x),
                                      ((transform.position.y) - target.transform.position.y), 0.0f);
        }
        else //move back para a linha de antes (wolf)
        {
            if (moveToFinal)
            {
                range = Vector2.Distance(transform.position, finalPoint.transform.position);

                if (range >= 0.5f)
                {
                    movement = new Vector3(((transform.position.x) - finalPoint.transform.position.x),
                                             ((transform.position.y) - finalPoint.transform.position.y), 0.0f);
                }
                else moveToFinal = false;
            }
            else
            {
                range = Vector2.Distance(transform.position, initialPoint.transform.position);
                if (range >= 0.5f)
                {
                    movement = new Vector3(((transform.position.x) - initialPoint.transform.position.x),
                                             ((transform.position.y) - initialPoint.transform.position.y), 0.0f);
                }
                else moveToFinal = true;
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

        rb2d.MovePosition(transform.position + movement); //outra maneira de mover
        //rb2d.velocity = movement; //setar velocidade

    }
}
