using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public Slider musicslider, sfxslider;

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicslider.value);
    }

    public void sfxVolume()
    {
        AudioManager.Instance.SFXVolume(sfxslider.value);
    }
}
