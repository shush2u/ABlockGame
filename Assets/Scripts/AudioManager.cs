using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource settleSound;

    public AudioMixer audioMixer;

    public void setSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void setMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    private bool focused = true;
    public void toggleAudioFocus()
    {
        if (focused)
        {
            audioMixer.SetFloat("MasterVolume", outOfFocusVolume);
            focused = false;
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", gameplayVolume);
            focused = true;
        }
        
    }

    public GameObject music;

    public void StopMusic()
    {
        music.SetActive(false);
    }
    public void StartMusic()
    {
        music.SetActive(true);
    }

    private void Start()
    {
        UpdateSliderVisuals();
    }

    [Header("Master Audio")]
    public float gameplayVolume;
    public float outOfFocusVolume;

    [Header("Slider Visuals")]
    public Slider SFXSlider;
    private float SFXValue;
    public Slider MusicSlider;
    private float MusicValue;

    private void UpdateSliderVisuals()
    {
        SFXValue = GetChannelLevel("SFXVolume");
        MusicValue = GetChannelLevel("MusicVolume");
        SFXSlider.value = SFXValue;
        MusicSlider.value = MusicValue;
    }

    private float GetChannelLevel(string channel) // thanks jeromeWork
    {
        float value;
        bool result = audioMixer.GetFloat(channel, out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }
}
