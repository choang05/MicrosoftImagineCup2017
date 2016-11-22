using UnityEngine;
using UnityEngine.UI;

public class ResolutionChanger : MonoBehaviour
{

    public Slider slider;
    public Text resolutionText;
    public Toggle fullScreenToggle;

    private SettingsManager settingsManager;

    void OnEnable()
    {
        bool isScreenSame = false;
        for(int i = 0; i<Screen.resolutions.Length - 1; i++)
        {
            if (Screen.resolutions[i].width == settingsManager.ResolutionWidth && Screen.resolutions[i].height == settingsManager.ResolutionHeight)
            {
                slider.value = i;
                isScreenSame = true;
            }
        }
        if (!isScreenSame)
            slider.value = 1;
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
        settingsManager.ResolutionWidth = Screen.resolutions[(int)(slider.value - 1)].width;
        settingsManager.ResolutionHeight = Screen.resolutions[(int)(slider.value - 1)].height;

        Screen.SetResolution(settingsManager.ResolutionWidth, settingsManager.ResolutionHeight, !settingsManager.IsWindowed);

        //  Save the new resolution in settings manager
        settingsManager.SaveSettings(); 
    }
}