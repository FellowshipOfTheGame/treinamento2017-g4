using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMuscisScriptTrial : MonoBehaviour
{
    //public
    public AudioClip medievalSound; //som do boss ficando vulneravel
    public AudioClip bossSound; //som do boss ficando vulneravel
    public AudioClip futuristSound; //som do boss ficando vulneravel

    //private
    private AudioSource audioSource; //componente de audio

    private GameObject bossFind;

    private bool playFuturist;

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source
        bossFind = null;

        audioSource.clip = medievalSound;
        audioSource.Play();

        playFuturist = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Teleporter") != null)
        {
            playFuturist = true;
        }

        if (playFuturist && GameObject.Find("Teleporter") == null)
        { //se mudar de tela trocar a musica tema
            medievalSound = futuristSound;
            audioSource.clip = medievalSound;
            audioSource.Play();
            playFuturist = false;
        }

        bossFind = GameObject.Find("Enemy_BossOne");

        if (bossFind == null)
        {
            bossFind = GameObject.Find("Enemy_BossTwo");
            if (bossFind == null)
            {
                if (audioSource.isPlaying)
                {
                    if (audioSource.clip == bossSound)
                    {
                        audioSource.clip = medievalSound;
                        audioSource.Play();
                    }
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    if (audioSource.clip == medievalSound)
                    {
                        audioSource.clip = bossSound;
                        audioSource.Play();
                    }
                }
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                if (audioSource.clip == medievalSound)
                {
                    audioSource.clip = bossSound;
                    audioSource.Play();
                }
            }
        }

    }
}
