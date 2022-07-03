using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void setSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void setMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetFullscren(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    Resolution[] resolutions;

    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        UpdateSliderVisuals();

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

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
