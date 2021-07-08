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
    public void SaveSFXVolume()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        masterAudioMixer.SetFloat("sfxVolume", sfxVolume);
    }
    public void SaveMusicVolume()
    {
        musicVolume = musicSlider.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        masterAudioMixer.SetFloat("musicVolume", musicVolume);
    }
    public void EnableGameMusic()
    {
        audioSource.Stop();
        audioSource.clip = gameMusic;
        audioSource.Play();
    }
    public void DisableGameMusic()
    {
        audioSource.Stop();
    }
}
