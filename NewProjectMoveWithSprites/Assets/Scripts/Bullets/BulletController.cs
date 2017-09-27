using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //public
    public float bulletSpeed; //velocidade da bala (default)

    //private
    private Rigidbody2D rb2d; //rigdbody2d da bala

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar Rigidbody2D da bala

        //adicionando velocidade na bala
        rb2d.velocity = gameObject.transform.right * bulletSpeed;// * Time.deltaTime; //velocidade da bala por segundo
    }

    // Update is called once per frame
    void Update()
    {

    }

    //setar velocidade da bala
    public void SetVelocity(float bulletSpeed)
    {
        this.bulletSpeed = bulletSpeed;
    }
}
