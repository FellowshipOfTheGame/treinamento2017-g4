using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnControllerBoss : MonoBehaviour
{
    //public
    public GameObject enemyPrefab; //prefab do inimigo a ser spawnado
    public GameObject player; //referencia do player

    public int enemiesQuantity; //quantidade de inimigos a ser spawnado
    public float spawnTime; //de quanto em quanto tempo spawna um inimigo

    public bool canSpawn; //se pode spawnar

    //private
    private int enemyCounter; //contador de inimigos spawnados

    private List<GameObject> inimigos;

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

            GameObject enemy = Instantiate(enemyPrefab); //instanciar inimigo
            enemy.name = enemyPrefab.name; // ajustar nome
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
            case "Enemy_BossOne":
                enemy.GetComponent<EnemyControllerBossOne>().target = player;
                break;
            case "Enemy_BossTwo":
                enemy.GetComponent<EnemyControllerBossOne>().target = player;
                break;
            default:
                Debug.Log("What Enemy Boss to spawn? (target)");
                break;
        }
    }
}
