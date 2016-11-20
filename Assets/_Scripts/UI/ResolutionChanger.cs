﻿using UnityEngine;
using UnityEngine.UI;

public class ResolutionChanger : MonoBehaviour
{

    public Slider slider;
    public Text resolutionText;
    public Toggle fullScreenToggle;

    private SettingsManager settingsManager;

    void OnEnable()
    {
        if (settingsManager.ResolutionWidth == 800 && settingsManager.ResolutionHeight == 600)
            slider.value = 1;
        else if (settingsManager.ResolutionWidth == 1024 && settingsManager.ResolutionHeight == 768)
            slider.value = 2;
        else if (settingsManager.ResolutionWidth == 1280 && settingsManager.ResolutionHeight == 768)
            slider.value = 3;
        else if (settingsManager.ResolutionWidth == 1366 && settingsManager.ResolutionHeight == 768)
            slider.value = 4;
        fullScreenToggle.isOn = settingsManager.IsWindowed;

    }

    void Awake()
    {
        settingsManager = FindObjectOfType<SettingsManager>();
        slider.maxValue = Screen.resolutions.Length;
    }

    public void UpdateResolutionText()
    {
        resolutionText.text = Screen.resolutions[(int)(slider.value - 1)].ToString();
    }

    public void ChangeResolution()
    {
        settingsManager.IsWindowed = fullScreenToggle.isOn;

        switch ((int)slider.value)
        {
            
            case 1:
                Screen.SetResolution(800, 600, !settingsManager.IsWindowed);
                settingsManager.ResolutionWidth = 800;
                settingsManager.ResolutionHeight = 600;
                break;
            case 2:
                Screen.SetResolution(1024, 768, !settingsManager.IsWindowed);
                settingsManager.ResolutionWidth = 1024;
                settingsManager.ResolutionHeight = 768;
                break;
            case 3:
                Screen.SetResolution(1280, 768, !settingsManager.IsWindowed);
                settingsManager.ResolutionWidth = 1280;
                settingsManager.ResolutionHeight = 768;
                break;
            case 4:
                Screen.SetResolution(1366, 768, !settingsManager.IsWindowed);
                settingsManager.ResolutionWidth = 1366;
                settingsManager.ResolutionHeight = 768;
                break;
        }
        

        //  Save the new resolution in settings
        settingsManager.SaveSettings();
    }
}