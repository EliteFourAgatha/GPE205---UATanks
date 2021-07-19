using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    public Slider sfxSlider;
    public Slider musicSlider;
    public AudioMixer masterAudioMixer;
    private float sfxVolume;
    private float musicVolume;
    void Start() 
    {
        //play menu music when first generated
        audioSource.clip = menuMusic;
        audioSource.Play();
    }

    //Functions supplied to slider in inspector
    // Saves current slider value to float, save to player prefs and audio mixer channel
    //  Slider values are from 0.001 to 1 to match actual audio mixer values
    //   Taking the log and multiplying by 20 gives attenuation level (logarithmic)
    //   instead of the slider level (linear). From unity forums answer:
    //   https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
    public void SaveSFXVolume()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", Mathf.Log(sfxVolume) * 20);
        masterAudioMixer.SetFloat("sfxVolume", Mathf.Log(sfxVolume) * 20);
        PlayerPrefs.Save();
    }
    public void SaveMusicVolume()
    {
        musicVolume = musicSlider.value;
        PlayerPrefs.SetFloat("musicVolume", Mathf.Log(musicVolume) * 20);
        masterAudioMixer.SetFloat("musicVolume", Mathf.Log(musicVolume) * 20);
        PlayerPrefs.Save();
    }
    public void EnableGameMusic()
    {
        audioSource.Stop();
        audioSource.clip = gameMusic;
        audioSource.Play();
    }
    public void EnableMenuMusic()
    {
        audioSource.Stop();
        audioSource.clip = menuMusic;
        audioSource.Play();
    }
}
