using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour //contem o dano e a vida do player
{
    //public
    public int bodyDamage; //dano do jogador (casos especificos --> descomentar)
    public int health; //vida do jogador

    public float damageDelay; //delay para poder levar dano de novo

    public AudioClip damageSound; //som do player levando dano

    public Text youWinText;
    public Text youWinTextReturn;

    //private
    private bool canReceiveDamage;

    private AudioSource audioSource; //componente de audio

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source

        canReceiveDamage = true;

        //youWinText.gameObject.SetActive(true);
        //youWinTextReturn.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //enquanto estiver colidindo
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            DoDamage(other);
            //Debug.Log(health);
        }
    }

    void DoDamage(Collision2D other) //dar dano no player (caso possivel)
    {
        other.gameObject.SendMessage/*Upwards*/("ReceiveDamage", bodyDamage, SendMessageOptions.DontRequireReceiver); //enviar dano, nao requisitar erro caso nao encontre
    }

    public void ReceiveDamage(int damage)
    {
        if (canReceiveDamage && damage > 0) //caso possa receber dano e o dano seja maior que zero
        { //caso possa levar dano
            audioSource.PlayOneShot(damageSound); //tocar som de levar dano

            StartCoroutine(DecreaseHealth(damage));
        }
    }

    public IEnumerator DecreaseHealth(int damage) //levar dano e esperar x segundos ate poder levar novamente
    {
        canReceiveDamage = false; //nao pode levar dano por determinado tempo

        if (health - damage <= 0) //diminuir vida
        {
            health = 0;
            bool infinityLife = true;

            //infinityLife
            if (PlayerPrefs.HasKey("infinityLife")) //pegar dos prefs iniciais
            {
                infinityLife = PlayerPrefs.GetInt("infinityLife") == 1 ? true : false; //Se 1 true se zero false
            }
            else
            {
                Debug.Log("Non Existing Key: infinityLife");
                PlayerPrefs.SetInt("inifinityLife", 1);
            }

            if (infinityLife)
            {
                health = 100; //re setar vida em 100 quando acabar (mudar dps)
            }
            else
            { //voltar para o menu
                SceneManager.LoadScene(0);
                //youWinText.enabled = true;
                //youWinTextReturn.enabled = true;
            }
        }
        else
        {
            health = health - damage;
        }

        //Debug.Log("Health: " + health);
        Debug.Log("Health " + gameObject.name + ": " + health);

        yield return new WaitForSeconds(damageDelay); //esperar x segundos antes de poder levar dano de novo

        canReceiveDamage = true;//pode levar dano de novo
    }

}
