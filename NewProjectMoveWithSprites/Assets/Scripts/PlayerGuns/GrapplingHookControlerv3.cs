using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookControlerv3 : MonoBehaviour
{
    //public
    public float stepPull; //distancia que vai mudar por update do grappling hook
    public float maxRange; //distancia maxima do grappling hook

    public float timeDelayThrow; //tempo de delay do resize pra lancar
    public float sizeDifferenceThrow; //tamanho por tempo de delay pra lancar

    public bool stopThrowing;

    public AudioClip shootSound; //som do tiro disparando

    //private
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Transform bulletSpawn; //posicao de spawn das balas / posicao do hook

    private bool isThrowing; //se esta jogando/puxando (dando resize) 

    private GameObject player; //usuario da arma
    private DistanceJoint2D distanceJoint; //pegar distance joint do player

    private string target; //quem podera ser acertado
    private int collisionMask; //mascara para saber com quem a "bala" colide / raycast

    private Vector2 pontoColisao; //pega o ponto ao qual o grappling hook vai se direcionar
    private bool needRotation; //se precisa rodar o grappling hook enquanto lancando
    private bool needResize; //se precisa resetar tamanho do grappling hook quando para de lancar

    private Vector3 originalSize; //tamanho original do grappling hook
    private float xSize; //tamanho da sprite pra resize

    //private bool startResizingSprite; //pra arrumar um bug

    private AudioSource audioSource; //componente de audio

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        bulletSpawn = gameObject.transform.GetChild(0); //pegar bulletSpawn

        SetBulletSpawnPosition();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        player = gameObject.transform.parent.gameObject.transform.parent.gameObject; //pegar usuario da arma (partindo do pressuposto que a arma esta a um GunPivot abaixo do objeto)
        distanceJoint = player.GetComponent<DistanceJoint2D>(); //pegar distance joint do player
        distanceJoint.enabled = false;

        collisionMask = 1 << LayerMask.NameToLayer("PlayerBullet") | 1 << LayerMask.NameToLayer("EnemyBullet") | 1 << LayerMask.NameToLayer("Character") | 1 << LayerMask.NameToLayer("Enemy"); //pegar layer corrente (bit shift + bit or)
        collisionMask = ~collisionMask; //todas menos as selecionadas (inverter)

        //reajustar tamanhos
        Vector2 newSize = new Vector2(spriteRenderer.size.x * gameObject.transform.localScale.x, spriteRenderer.size.y * gameObject.transform.localScale.y);
        spriteRenderer.size = newSize;
        gameObject.transform.localScale = new Vector3(1f, 1f, transform.localScale.z);

        originalSize = /*gameObject.transform.localScale*/spriteRenderer.size; // guarda tamanho original do grappling hook

        Bounds b = spriteRenderer.sprite.bounds; //isso deu trabalho pra encontrar '-'-' .-.-.
        //Debug.Log("Tamanhos gph: " + player.transform.localScale.x + " : : " + spriteRenderer.size.x);// + " : : " + b.size.x);
        xSize = /*b.size.x * */player.transform.localScale.x;// * player.transform.localScale.x;// * spriteRenderer.size.x);//player.GetComponent<SpriteRenderer>().sprite.bounds.size.x; //como ja tem o tamanho da sprite original, soh precisa do tamanho do parent/player
        //isso tbm deu um trabalho pra achar --> resize no filho (independente, tipo como se fosse separado) = 1/size do pai
        //o xSize ja ta na divisao

        isThrowing = false;
        stopThrowing = true;
        needRotation = false;
        needResize = false;

        //startResizingSprite = false;
    }

    public void SetBulletSpawnPosition() //posiciona o bulletSpawn no fim do grappHook
    {
        BoxCollider2D gunBoxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        float initialPosX = transform.localPosition.x + gunBoxCollider2d.size.x;
        Vector3 bulletSpawnPos = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);// + transform.right;

        bulletSpawn.transform.localPosition = bulletSpawnPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (distanceJoint.enabled)
        {
            float aux = Vector3.Distance(player.transform.position, distanceJoint.connectedAnchor);
            spriteRenderer.size = new Vector3((aux != 0 ? aux : 1) / xSize, /*transform.localScale.y*/spriteRenderer.size.y);//, transform.localScale.z);
        }

        if (distanceJoint.distance > 0.5f)
        { //"puxar"
          //float scaleX = Mathf.Abs(pontoColisao.x - player.transform.position.x);
          //float scaleY = Mathf.Abs(pontoColisao.y - player.transform.position.y);
          //Vector2 scale = new Vector2(scaleX, scaleY);

            /*gameObject.transform.localScale*/
            //spriteRenderer.size = new Vector3((distanceJoint.distance) / xSize, /*transform.localScale.y*/spriteRenderer.size.y);//, transform.localScale.z);
            //float aux = Vector3.Distance(player.transform.position, distanceJoint.connectedAnchor);
            //spriteRenderer.size = new Vector3((aux != 0 ? aux : 1) / xSize, /*transform.localScale.y*/spriteRenderer.size.y);//, transform.localScale.z);

            distanceJoint.distance -= stepPull;
        }
        else
        {
            //distanceJoint.enabled = false;
        }

        if (needRotation) //se precisar arruamr rotacao
        {
            Gun_Turning(pontoColisao);
        }

        if (Input.GetKeyDown(KeyCode.Space)) //atirar //tiros por seg permitidos se a tecla estiver pressionada
        {
            audioSource.PlayOneShot(shootSound); //tocar som de acertar

            /*gameObject.transform.localScale*/
            spriteRenderer.size = originalSize; //resetar tamanho
            needResize = false;
            stopThrowing = false;

            //TryLaunchGrapplingHook();
            if (!isThrowing) StartCoroutine(ScaleOverTimeThrow());
            spriteRenderer.enabled = true;
            boxCollider.enabled = true;

            //isAttached = true;
            Vector3 actualPlayerPosition = gameObject.transform.position; //pega posicao do player
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pegar "target" transform

            pontoColisao = DoLaser(targetPosition); //tentar pegar no objeto proximo
            Gun_Turning(pontoColisao);

            needRotation = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            needResize = true;
            //isAttached = false;
            stopThrowing = true;

            distanceJoint.enabled = false; //desativa distance joint
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;

            pontoColisao = Vector3.zero;
            needRotation = false;

            /*gameObject.transform.localScale*/
            spriteRenderer.size = originalSize; //resetar tamanho
        }
    }

    //quando entrar no trigger
    private void OnTriggerStay2D(Collider2D other)
    { //ao entrar tentar dar dano/parar
        if (!stopThrowing) TryStopThrowing(other); //se estiver arremessando o hook
    }

    public void TryStopThrowing(Collider2D other) //verificar se esta vendo a possibilidade de dar dano
    {
        //Debug.Log(other.gameObject.name);
        if ((collisionMask & (1 << other.gameObject.layer)) != 0) //se estiver colidindo com uma das layers selecionadas
        { //primeiro parar, depois realizar outros processos
            stopThrowing = true;

            distanceJoint.enabled = true; //ativa distance joint

            distanceJoint.connectedAnchor = pontoColisao;
            distanceJoint.distance = Vector3.Distance(pontoColisao, player.transform.position);
        }
    }

    public IEnumerator ScaleOverTimeThrow() //novo jeito de jogar o laser (que nem o braco)
    {
        isThrowing = true;

        Vector3 originalScale = spriteRenderer.size;//gameObject.transform.localScale; //para tiled
        Vector3 destinationScale = new Vector3(originalScale.x + sizeDifferenceThrow, originalScale.y, originalScale.z);

        float currentTime = 0.0f;
        do
        {
            /*gameObject.transform.localScale*/
            spriteRenderer.size = Vector3.Lerp(originalScale, destinationScale, currentTime / timeDelayThrow);
            currentTime += Time.deltaTime;
            yield return null;
            if (/*gameObject.transform.localScale.x*/spriteRenderer.size.x >= maxRange) isThrowing = false; //se alcançar a range maxima parar de jogar
        } while (currentTime <= timeDelayThrow && isThrowing && !stopThrowing); //enquanto estiver jogando/nao acabar o tempo/maxScale

        if (/*gameObject.transform.localScale.x*/spriteRenderer.size.x < maxRange && !stopThrowing)
        {
            StartCoroutine(ScaleOverTimeThrow()); //se nao estiver jogando nem puxando
        }
        else
        {
            //isPulling = true;
            isThrowing = false; //resetar variaveis e comecar a puxar
            stopThrowing = false;

            if (needResize)
            {
                /*gameObject.transform.localScale*/
                spriteRenderer.size = originalSize; //resetar tamanho
            }
        }
    }

    public Vector3 DoLaser(Vector3 targetPosition) //faz um raycast na direcao da arma pra ver se acerta algo (/o player) [caso acerte o target, retorna true, caos nao, false]
    {
        //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pegar "target" transform

        RaycastHit2D pontoColisao; //ponto de colisao do laser antes da distancia maxima

        Vector3 direction = (targetPosition - gameObject.transform.position).normalized; //direcao para verificar
        Ray2D ray = new Ray2D(gameObject.transform.position, direction); //criar raio da posicao na direcao do mouse 

        pontoColisao = Physics2D.Raycast(ray.origin, direction, maxRange, collisionMask);//, LayerMask.NameToLayer("PlayerBullet")); //com ponto de colisao e distancia e ignorar layers de inimgos, balas e do character
        Debug.DrawLine(ray.origin, pontoColisao.point, new Color(10f, 10f, 10f), 100f);

        //Debug.Log(pontoColisao.collider + " bla " + LayerMask.NameToLayer("PlayerBullet"));
        if (pontoColisao.collider) //verificar hit com algo
        {
            //if (pontoColisao.transform.gameObject == target) return true;
            return pontoColisao.point;
        }

        return Vector3.zero; //nao acertou nada (nao deve ocorrer)
    }

    //Girar a arma na direcao de um ponto
    void Gun_Turning(Vector2 point)
    {
        float angleDegrees = Gun_PlayerToPoint(point) - 90; //pegar rotacao do mouse - a orientacao inicial em relacao a base (up->0) (inicial->right->90)
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angleDegrees));
    }

    //Pega o angulo em graus entre a posicao do mouse e o centro da tela (começa em cima==> 0, sentido horario direita==> 90)
    float Gun_PlayerToPoint(Vector2 point) //"girar"
    {
        //Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //posicao do mouse

        //pegar angulo
        Vector2 newVector = point - (Vector2)player.transform.position; //pegar vetor do ponto ao mouse

        float angleRadians = Mathf.Atan2(newVector.x, newVector.y); //pegar angulo em radianos

        float angleDegrees = angleRadians * Mathf.Rad2Deg; //converter para graus (-180, 180)
        if (angleDegrees < 0) angleDegrees += 360.0f; //normalizar (0, 360) "359.9_" (0 up, 90 right, 180 down, 270 left)

        return angleDegrees;
    }
}

/** descricao rapida:
 *      sprite invisivel ate comecar a lancar (como o grapplingHook v1 braco)
 *      so pode lancar de novo quando retornar/parar de lancar (como o braco)
 *      lancamento na direcao do mouse (como o grapplinghookv2 raycast)
 *      lancamento em formato de laser (como o laser) + bulletSpawn na ponta final (ja filho)
 *      rotacao fixa na direcao do lancamento
 *      ao atingir algum objeto (ignorar inimigos/resto) fixar ponto e tamanho da sprite do "laser"
 *      utilizar reducao por step (como grapplingHookv2) + dar resize na sprite para caber (como o laserv2 doLaser)
 *      sempre ajustar rotacao da sprite (no update na direcao do lancamento) para caso o player se mova
 *      fazer raycast antes do lancamento (como grapplingHookv2) com maxRange alto para saber 
 *        ponto final para consertar rotacao (posPlayer -> pontoFinal) 
 *         [sempre existira um ponto final (deve, para evitar erros)]
**/
