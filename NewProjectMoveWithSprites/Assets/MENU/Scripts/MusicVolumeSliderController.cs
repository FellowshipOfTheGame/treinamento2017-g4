using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSliderController : MonoBehaviour {

    //Slider game objects
    public Slider musicVolumeMenu;

    private float currentVolume;

    //Check change 
    public void StoreCurrentVolume()
    {
        currentVolume = musicVolumeMenu.value;
        PlayerPrefs.SetFloat("MusicVolumeMenu", currentVolume);
    }

}
