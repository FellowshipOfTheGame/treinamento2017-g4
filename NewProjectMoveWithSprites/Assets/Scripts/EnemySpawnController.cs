using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour //precisa ser atualizada pra cada inimigo novo que vai utilizar o spawn por causa dos scripts (quando mudar o player dos scripts de lugar da pra tirar)
{
    //public
    public GameObject enemyPrefab1; //prefab do inimigo a ser spawnado
    public GameObject enemyPrefab2; //prefab do inimigo a ser spawnado
    public GameObject enemyPrefab3; //prefab do inimigo a ser spawnado
    public GameObject enemyPrefab4; //prefab do inimigo a ser spawnado
    public GameObject player; //referencia do player

    public int enemiesQuantity; //quantidade de inimigos a ser spawnado
    public float spawnTime; //de quanto em quanto tempo spawna um inimigo

    public bool canSpawn; //se pode spawnar

    //private
    private int enemyCounter; //contador de inimigos spawnados

    private List<GameObject> inimigos;
    private GameObject[] en;

    // Use this for initialization
    void Start()
    {
        enemyCounter = 0;

        player = GameObject.Find("Player");

        if (player != null)
        {
            TrySpawnEnemies();
        }

        inimigos = new List<GameObject>();

        en = new GameObject[4];
        en[0] = enemyPrefab1;
        en[1] = enemyPrefab2;
        en[2] = enemyPrefab3;
        en[3] = enemyPrefab4;
    }

    // Update is called once per frame
    void Update()
    {
        if ((enemiesQuantity == 0 && inimigos.Count == 0) || canSpawn == false)
        {
            gameObject.SetActive(false);
        }
        VerifyList();
    }

    public void VerifyList()
    {
        inimigos.RemoveAll(item => item == null);
        //foreach (GameObject go in inimigos)
        //{
        //    if (go == null)
        //    {
        //        inimigos.Remove(go);
        //    }
        //}
    }

    public void TrySpawnEnemies() //tentar spawnar inimigo
    {
        StartCoroutine(SpawnEnemy()); //comecar a spawnar inimigos
    }

    public IEnumerator SpawnEnemy() //spawnar inimigo
    {
        if (canSpawn && enemyCounter < enemiesQuantity) //enquanto nao tiver sido spawnada a quantidade de inimigos
        {
            yield return new WaitForSeconds(spawnTime); //esperar tempo x 
                                                        //enemyCounter++;

            GameObject g = en[Random.Range(0, 4)];

            GameObject enemy = Instantiate(g); //instanciar inimigo
            enemy.name = g.name; // ajustar nome
            enemy.transform.position = gameObject.transform.position; // ajustar posicao
            SetScriptsValues(enemy); //setar valores necessarios nos scripts

            inimigos.Add(enemy);
            enemiesQuantity--;

            StartCoroutine(SpawnEnemy()); //tentar spawnar novo inimigo
        }
    }

    public void SetScriptsValues(GameObject enemy) //setar valores necessarios nos scripts
    {
        enemy.GetComponent<EnemyDropWeapon>().player = player; //setar player no drop component (qual pode pegar o drop, etc)

        switch (enemy.name) //setar player no enemy controller component
        {
            case "Enemy_Monkey":
            case "Enemy_MonkeyRobot":
                enemy.GetComponent<EnemyControllerMonkey>().target = player;
                break;
            case "Enemy_Spider":
            case "Enemy_SpiderRobot":
                break;
            case "Enemy_Wasp":
            case "Enemy_WaspRobot":
                enemy.GetComponent<EnemyControllerWasp>().target = player;
                break;
            case "Enemy_Wolf":
            case "Enemy_WolfRobot":
                enemy.GetComponent<EnemyControllerWolfv2>().target = player;
                break;
            default:
                Debug.Log("What Enemy to spawn? (target)");
                break;
        }
    }
}
