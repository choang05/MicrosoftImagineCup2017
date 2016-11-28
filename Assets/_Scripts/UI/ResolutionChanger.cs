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
        for(int i = 0; i<Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == settingsManager.ResolutionWidth && Screen.resolutions[i].height == settingsManager.ResolutionHeight)
            {
                slider.value = i + 1;
                isScreenSame = true;
                break;
            }
        }
        if (!isScreenSame)
            slider.value = 1;
        fullScreenToggle.isOn = settingsManager.IsWindowed;
    }

    void Awake()
    {
        settingsManager = FindObjectOfType<SettingsManager>();
        slider.onValueChanged.AddListener((float f) => { settingsManager.ResolutionWidth = Screen.resolutions[(int)(slider.value) - 1].width; settingsManager.ResolutionHeight = Screen.resolutions[(int)(slider.value) - 1].height; });
        slider.maxValue = Screen.resolutions.Length;

    }

    public void UpdateResolutionText()
    {
        resolutionText.text = Screen.resolutions[(int)slider.value - 1].ToString();
    }

    public void ChangeResolution()
    {
        Screen.SetResolution(settingsManager.ResolutionWidth, settingsManager.ResolutionHeight, !settingsManager.IsWindowed);
    }
}