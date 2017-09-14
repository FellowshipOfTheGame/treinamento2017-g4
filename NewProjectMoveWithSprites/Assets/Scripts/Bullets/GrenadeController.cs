using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    //public
    public float bulletSpeed; //velocidade da bala (default)

    public float explosionRadius; //raio da explosao
    public float linearDrag; //forca contraria ao movimento da bala (simular "parada" com o tempo)

    //private
    private Rigidbody2D rb2d; //rigdbody2d da bala
    private ParticleSystem ps; //particleSystem da granada ("explosao")

    private HashSet<GameObject> listOfDamages; //lista de objetos que receberao dano na explosao

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //pegar Rigidbody2D da bala

        GameObject child = gameObject.transform.GetChild(0).gameObject; //pegar filho
        ps = child.GetComponent<ParticleSystem>(); //pegar o particle System (estando no filho)

        //alterando linear drag da bala
        gameObject.GetComponent<Rigidbody2D>().drag = linearDrag; //alterar forca contraria ao movimento da bala (drag)

        //adicionando velocidade na bala
        rb2d.velocity = gameObject.transform.right * bulletSpeed;// * Time.deltaTime; //velocidade da bala por segundo
        //rb2d.AddForce(gameObject.transform.right * bulletSpeed);

        listOfDamages = new HashSet<GameObject>(); //inicializar lista de objetos que receberao dano na explosao

/*<----------->*/         Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x + explosionRadius, gameObject.transform.position.y, 0f), new Color(10f, 10f, 10f), 100f);
/*<----------->*/        Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + explosionRadius, 0f), new Color(10f, 10f, 10f), 100f);
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

    public float Explode(int collisionMask)
    {
        rb2d.velocity = Vector3.zero; // zerar velocidade quando for destruir
        Vector3 centralPoint = gameObject.transform.position; //pegar ponto central da explosao

        Collider2D[] colliders; //colliders dos objetos que receberao dano
        colliders = Physics2D.OverlapCircleAll(centralPoint, explosionRadius, collisionMask); //pegar todos os objetos com collider num raio x em uma mascara de colisao especifica

        foreach (Collider2D coll in colliders) //pra cada collider adicionar o gameobject na lista
        {
            if (coll.gameObject != gameObject)
            {
                //Debug.Log("GMObject1: " + coll.gameObject);
                //Debug.Log("GMObject2: " + coll.transform.gameObject);
                listOfDamages.Add(coll.gameObject);
            }
        }

        gameObject.GetComponent<SpriteRenderer>().enabled = false; //desabilitar sprite

        ParticleSystem.ShapeModule s = ps.shape; //teste de alterar o shape total
        s.radius = explosionRadius / 2; //reajustar raio da explosao
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
        ps.emission.GetBursts(bursts);
        bursts[0].minCount = (short) (explosionRadius <= 1f ? explosionRadius * 53f : explosionRadius <= 5f ? 53f + (explosionRadius * 3f) : 68f + (explosionRadius * 3f)); //reajsutar min qtd de particulas (if ternario (x <= 1 < y <= 5 < z))
        bursts[0].maxCount = (short) (explosionRadius <= 1f ? explosionRadius * 67f : explosionRadius <= 5f ? 67f + (explosionRadius * 3f) : 82f + (explosionRadius * 3f)); //reajsutar max qtd de particulas

        ps.Play(); //comecar efeito de particulas
        return ps.main.duration; //informar tempo que vai durar
    }

    public HashSet<GameObject> GetListOfDamages() //pegar lista de danos (objetos pra enviar dano/tentar)
    {
        return listOfDamages;
    }

    //quando for destruir
    private void OnDestroy()
    {

    }
}
