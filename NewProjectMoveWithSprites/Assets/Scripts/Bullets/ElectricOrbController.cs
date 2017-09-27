using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricOrbController : MonoBehaviour
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

    //setar velocidade da bala
    public void SetVelocityAndDirection(Vector3 direction, float bulletSpeedPercent)
    {
        rb2d.velocity = direction * (bulletSpeed * bulletSpeedPercent);// * Time.deltaTime; //velocidade da bala por segundo //nova velocidade com porcentagem da anterior

        //Vector3 newRotation = Vector3.RotateTowards(gameObject.transform.right, direction, 2 * Mathf.PI, 0.0f); //arrumar rotacao de acordo com o vetor, maximo por vez 2pi, profundidade 0
        //Quaternion.LookRotation(newRotation);
        gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction); //arrumar rotacao da bala
    }
}
