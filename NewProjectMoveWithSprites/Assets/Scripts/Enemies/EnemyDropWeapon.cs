using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropWeapon : MonoBehaviour //por enqto com todas as armas da 5 "armas" + 5 "cores"
{
    //public
    //public Dictionary<GameObject, int> weaponsProbability; //facilita mais usar duas lists
    public GameObject player; //referencia para quem pega/usa as armas
    public GameObject weaponBasePrefab; //prefab para a base de arma/drop

    public List<GameObject> weapons; //armas que pode dropar (prefab)
    public int[] weaponsProbability; //probabilidade das armas que pode dropar (int) somando 100 //chance de nao dropar nada eh o que sobrar
    //ex: 5->armas prob->35%, 25%, 15%, 10%, 5% = 90% sobra 10% de chance de nao dropar nada

    public List<string> weaponsColors; //cores de armas que pode dropar (nomes)
    public int[] weaponsColorsProbability; //probabilidade das cores de armas que pode dropar (int) somando 100

    //private
    private bool isQuitting; //para caso esteja saindo da aplicacao nao instanciar novos objetos no onDestroy

    // Use this for initialization
    void Start()
    {
        //Probability prob = new Probability(4, new int[] { 10, 50, 20, 5 }); //teste das probabilidades
        //prob.ProbabilityVector_Imprimir();
        //Debug.Log("P1: " + prob.ProbabilityVector_ChooseOne());

        VerificarLists(); //verifica listas

        isQuitting = false;
    }

    public void VerificarLists() //verifica se as lists estao "corretas" tanto em igualdade de tamanhos quanto nao probabilidade nao ultrapassar 100
    {
        if (weapons.Count != weaponsProbability.Length || weaponsColors.Count != weaponsColorsProbability.Length)
        { //caso o tamanho do vetor de armas/cores seja diferente do de probabilidades
            Debug.Log("Something with the drop probability size is going wrong! --> " + gameObject.name);
        }

        int count = 0;
        foreach (int i in weaponsProbability)
        { //verificar a porcentagem das armas
            count += i;
        }
        if (count > 100)
        {
            Debug.Log("More than 100 percent weaponsProbability! --> " + gameObject.name);
        }

        count = 0;
        foreach (int i in weaponsColorsProbability)
        {
            count += i;
        }
        if (count > 100)
        {
            Debug.Log("More than 100 percent weaponsColorsProbability! --> " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //quando estiver saindo da aplicacao
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!isQuitting) //se nao estiver saindo
        {
            TryDropWeapon(); //"dropar" arma
        }
    }

    public void TryDropWeapon() //"dropar" arma aleatoriamente, inimigo
    {
        GameObject newWeaponDrop = PickRandomWeapon(); //pega arma que vai dropar / prefab

        if (newWeaponDrop == null)
        {
            Debug.Log(gameObject.name + " --> Nao dropou nada / problema no drop.");
            return;
        }

        string newWeaponDropColor = PickRandomColor(); //pega cor da arma que vai dropar

        GameObject newDroppedWeapon = Instantiate(weaponBasePrefab, gameObject.transform.position, gameObject.transform.rotation); //"dropar" arma que estava segurando 
        newDroppedWeapon.name = weaponBasePrefab.name; //refatorar nome

        GameObject newDroppedWeaponChild = newDroppedWeapon.transform.GetChild(0).gameObject; //pegar "arma" dropada
        WeaponControllerPickUp pickUpScript = newDroppedWeaponChild.GetComponent<WeaponControllerPickUp>(); //pegar script para setar valores
        pickUpScript.player = player;

        pickUpScript.weaponHere = newWeaponDrop; //criar arma no chao
        pickUpScript.weaponHereColor = newWeaponDropColor; //pegar / setar cor (para ativar) 

        Debug.Log(gameObject.name + ": Drop --> " + newWeaponDrop.name + "_" + newWeaponDropColor); //fala qual arma dropou / deveria dropar
    }

    public GameObject PickRandomWeapon() //pega uma das armas de acordo com a probabilidade aleatoriamente da list de armas
    {
        Probability prob = new Probability(weapons.Count, weaponsProbability); //teste das probabilidades das armas
        //prob.ProbabilityVector_Imprimir();
        //Debug.Log("P1: " + prob.ProbabilityVector_ChooseOne());

        int res = prob.ProbabilityVector_ChooseOne(); //pegar uma
        if (res != -1)
        {
            return weapons[res]; //retornar arma pega
        }

        return null; //se nao pegar nada / se "pegar" "vazio"  //sem arma
    }

    public string PickRandomColor() //pega uma das armas de acordo com a probabilidade aleatoriamente da list de armas
    {
        Probability prob = new Probability(weaponsColors.Count, weaponsColorsProbability); //teste das probabilidades das cores das armas

        int res = prob.ProbabilityVector_ChooseOne(); //pegar uma
        if (res != -1)
        {
            return weaponsColors[res]; //retornar arma pega
        }

        return "NoColor"; //se nao pegar nada / se "pegar" "vazio"  //arma sem cor
    }

}
