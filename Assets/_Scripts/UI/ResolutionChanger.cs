using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResolutionChanger : MonoBehaviour
{

    public Slider slider;
    public Text resolutionText;
    public Toggle fullScreenToggle;

    private SettingsManager settingsManager;

    void Awake()
    {
        settingsManager = FindObjectOfType<SettingsManager>();
    }

    public void UpdateResolutionText()
    {
        switch((int)slider.value)
        {
            case 1:
                resolutionText.text = "800 x 600";
                break;
            case 2:
                resolutionText.text = "1024 x 768";
                break;
            case 3:
                resolutionText.text = "1280 x 768";
                break;
            case 4:
                resolutionText.text = "1366 x 768";
                break;
        }
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
