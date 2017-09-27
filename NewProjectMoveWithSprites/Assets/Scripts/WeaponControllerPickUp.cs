using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllerPickUp : MonoBehaviour
{
    //public
    public GameObject player; //quem pode pegar a arma
    public GameObject weaponHere; //"arma" que esta aqui

    public string weaponHereColor; //cor da "arma" que esta aqui

    public float edgeRadius; //raio alem do colider da borda //0.2
    public float changeTime; //tempo para poder trocar a arma //1

    public GameObject weaponBasePrefab; //prefab para a base de arma/drop
    public List<GameObject> weapons; //armas disponiveis para re comparar //size 5

    public Sprite grapplingHookSprite; //nova sprite do grappling hook no drop

    //private
    //private GameObject weapons; //armas (indicado pelo weaponDrop)
    private SpriteRenderer spriteRenderer; //sprite renderer do pickUp
    private BoxCollider2D boxCollider; //box collider 2D do pickUp

    private GameObject playerWeapon; //arma de que esta trocando

    private GameObject droppedWeaponParent; //o pai da "arma" (WeaponBase) eh uma representacao do tamanho do player, para nao desconfigurar a arma
    private bool canChange; //verifica se pode trocar arma 
    private bool onTrigger; //verifica se esta no trigger 

    // Use this for initialization
    void Start()
    {
        if (weaponHere == null || player == null) //caso seja nulo destruir objeto
        {
            Debug.Log("Its somehow impossible to pick up this");
            Destroy(gameObject);
            return;
        }

        //boxCollider = gameObject.GetComponent<BoxCollider2D>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        droppedWeaponParent = gameObject.transform.parent.gameObject; //o pai da "arma" (WeaponBase) eh uma representacao do tamanho do player, para nao desconfigurar a arma
        droppedWeaponParent.transform.localScale = player.transform.localScale; //arrumar scale base

        GameObject weaponHereChild = weaponHere.gameObject.transform.GetChild(0).gameObject; //actually the gun itself
        spriteRenderer.sprite = weaponHereChild.GetComponent<SpriteRenderer>().sprite;
        gameObject.transform.localScale = weaponHereChild.transform.localScale;

        if (weaponHere.name == "GrapplingHookv3")
        { //ajustar sprite/tamanho caso seja grappling hook
            spriteRenderer.sprite = grapplingHookSprite;
            gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
        }

        if (player.gameObject.transform.childCount > 0)
        {
            playerWeapon = player.gameObject.transform.GetChild(0).gameObject; //arma do player
            //Debug.Log(playerWeapon);
        }

        onTrigger = false;
        canChange = false;

        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        Destroy(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        boxCollider.isTrigger = true;

        //boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.edgeRadius = edgeRadius; //raio alem do colider da borda

        StartCoroutine(WaitUntilCanChange());
    }

    public IEnumerator WaitUntilCanChange() //esperar tempo ate poder pegar arma apos aparecer
    {
        yield return new WaitForSeconds(changeTime);
        canChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && onTrigger) // Se estiver apertando o E trocar a arma
        {
            TryChangeWeapon();
        }
    }

    public void TryChangeWeapon()
    {
        if (player.gameObject.transform.childCount > 0)
        {
            playerWeapon = player.gameObject.transform.GetChild(0).gameObject; //arma do player
        }

        if (playerWeapon == null)
        { //se nao tiver nada
          //GameObject newWeapon = Instantiate(weaponHere, player.transform); //instanciar como child
          //newWeapon.transform.parent = player.gameObject.transform; //instanciar como child
        }
        else if (playerWeapon.name == "Hand")
        { //se for a "mao"
            Destroy(playerWeapon); //remover
        }
        else if (playerWeapon.name == "GrapplingHookv3")
        { //se for o grappling hook
            TryChangeWeaponAgain(); //re faz o if para a segunda arma (pq o grappling hook esta na primeira)
        }
        else
        { //se tiver alguma outra arma --> dropar e trocar
            DropCurrentWeapon(); //dropar
            Destroy(playerWeapon); //remover
        }
        GameObject newWeapon = Instantiate(weaponHere, player.transform); //instanciar como child
        newWeapon.name = weaponHere.name; //refatorar nome
        ChooseColor(newWeapon); //pegar cor da arma 

        Destroy(droppedWeaponParent); //destruir drop
    }

    public void ChooseColor(GameObject newWeapon) //ativa cor da arma nova
    {
        if (newWeapon.name != "Pistol" && newWeapon.name != "MachineGun" && newWeapon.name != "LaserCannon" && newWeapon.name != "GrenadeLauncher")
        //if (playerWeapon == null || playerWeapon.name == "GrapplingHookv3" || playerWeapon.name == "Hand")
        { //nao colocar cor caso nao seja uma dessas
            Debug.Log("This weapon has no color available!");
            return;
        }

        GameObject newWeaponChild = newWeapon.gameObject.transform.GetChild(0).gameObject; //pegar a arma

        switch (weaponHereColor)
        { //escolher cor e ativar script
            case "Gray":
                GunControllerColorGray gunColorGrayScript = newWeaponChild.GetComponent<GunControllerColorGray>();
                gunColorGrayScript.enabled = true;
                break;
            case "Blue":
                GunControllerColorBlue gunColorBlueScript = newWeaponChild.GetComponent<GunControllerColorBlue>();
                gunColorBlueScript.enabled = true;
                break;
            case "Green":
                GunControllerColorGreen gunColorGreenScript = newWeaponChild.GetComponent<GunControllerColorGreen>();
                gunColorGreenScript.enabled = true;
                break;
            case "Red":
                GunControllerColorRed gunColorRedScript = newWeaponChild.GetComponent<GunControllerColorRed>();
                gunColorRedScript.enabled = true;
                break;
            case "Yellow":
                GunControllerColorYellow gunColorYellowScript = newWeaponChild.GetComponent<GunControllerColorYellow>();
                gunColorYellowScript.enabled = true;
                break;
            default:
                Debug.Log("What color? '-', probably NoColor -> " + weaponHereColor);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == player && canChange)
        { //se for o player (ativar)
            onTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player && canChange)
        { //se for o player (destivar)
            onTrigger = false;
        }
    }

    public void DropCurrentWeapon() //dropar arma corrente do player
    {
        GameObject newDroppedWeapon = Instantiate(weaponBasePrefab, gameObject.transform.position, gameObject.transform.rotation); //"dropar" arma que estava segurando 
        newDroppedWeapon.name = "Map:-> " + weaponBasePrefab.name; //refatorar nome

        //GameObject newPrefabFound = (GameObject)Resources.Load("../Prefabs/PlayerGuns/" + playerWeapon.name, typeof(GameObject)); // encontrar prefab da arma //user pasta resources
        GameObject newPrefabFound = FindPrefab(playerWeapon.name); //encontra prefab da arma no vetor de disponiveis

        GameObject newDroppedWeaponChild = newDroppedWeapon.transform.GetChild(0).gameObject; //pegar "arma" dropada
        WeaponControllerPickUp pickUpScript = newDroppedWeaponChild.GetComponent<WeaponControllerPickUp>(); //pegar script para setar valores
        pickUpScript.player = player;

        if (newPrefabFound == null) //se nao encontrar
        {
            Debug.Log("A problem occurred while trying to find the dropped prefab: " + playerWeapon.name);
            return;
        }
        pickUpScript.weaponHere = newPrefabFound; //trocar arma
        pickUpScript.weaponHereColor = ChooseColorDrop(); //trocar cor
    }

    public GameObject FindPrefab(string nome)
    { //procura um prefab em uma lista pelo nome
        GameObject prefabFound = null; //inicializar

        foreach (GameObject gameO in weapons) //for each arma nas possiveis
        {
            if (gameO.name == nome)
            {
                prefabFound = gameO;
                break;
            }
        }

        return prefabFound;
    }

    public string ChooseColorDrop() //ativa cor da arma nova de acordo com a do player
    {
        if (playerWeapon.name != "Pistol" && playerWeapon.name != "MachineGun" && playerWeapon.name != "LaserCannon" && playerWeapon.name != "GrenadeLauncher")
        //if (playerWeapon == null || playerWeapon.name == "GrapplingHookv3" || playerWeapon.name == "Hand")
        { //nao colocar cor caso nao seja uma dessas
            Debug.Log("This weapon has no color available! Drop");
            return "NoColor"; //default
        }

        GameObject playerWeaponChild = playerWeapon.gameObject.transform.GetChild(0).gameObject; //pegar a arma

        GunControllerColorGray gunColorGrayScript = playerWeaponChild.GetComponent<GunControllerColorGray>();
        GunControllerColorBlue gunColorBlueScript = playerWeaponChild.GetComponent<GunControllerColorBlue>();
        GunControllerColorGreen gunColorGreenScript = playerWeaponChild.GetComponent<GunControllerColorGreen>();
        GunControllerColorRed gunColorRedScript = playerWeaponChild.GetComponent<GunControllerColorRed>();
        GunControllerColorYellow gunColorYellowScript = playerWeaponChild.GetComponent<GunControllerColorYellow>();

        if (gunColorGrayScript.enabled) //escolher cor e retornar script
        {
            return "Gray";
        }
        else if (gunColorBlueScript.enabled)
        {
            return "Blue";
        }
        else if (gunColorGreenScript.enabled)
        {
            return "Green";
        }
        else if (gunColorRedScript.enabled)
        {
            return "Red";
        }
        else if (gunColorYellowScript.enabled)
        {
            return "Yellow";
        }
        else
        {
            Debug.Log("Player weapon color? '-'");
            return "NoColor"; //default
        }
    }

    public void TryChangeWeaponAgain()
    {
        if (player.gameObject.transform.childCount > 1)
        {
            playerWeapon = player.gameObject.transform.GetChild(1).gameObject; //arma do player
        }
        else
        {
            playerWeapon = null;
        }

        if (playerWeapon == null)
        { //se nao tiver nada
          //GameObject newWeapon = Instantiate(weaponHere, player.transform); //instanciar como child
          //newWeapon.transform.parent = player.gameObject.transform; //instanciar como child
        }
        else if (playerWeapon.name == "Hand")
        { //se for a "mao"
            Destroy(playerWeapon); //remover
        }
        else
        { //se tiver alguma outra arma --> dropar e trocar
            DropCurrentWeapon(); //dropar
            Destroy(playerWeapon); //remover
        }
    }
}
