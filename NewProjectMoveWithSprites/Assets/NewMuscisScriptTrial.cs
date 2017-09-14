using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMuscisScriptTrial : MonoBehaviour
{
    //public
    public AudioClip medievalSound; //som do boss ficando vulneravel
    public AudioClip bossSound; //som do boss ficando vulneravel

    //private
    private AudioSource audioSource; //componente de audio

    private GameObject bossFind;

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); //pegar audio source
        bossFind = null;

        audioSource.clip = medievalSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
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
