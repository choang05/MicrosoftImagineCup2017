using UnityEngine;

public class SliderController : MonoBehaviour
{
    private UnityEngine.UI.Slider slider;
    private SettingsManager settings;
    public enum SoundGroup { Master, SFX, Music, Ambiance, UI }
    public SoundGroup group;

    void Awake()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
        settings = FindObjectOfType<SettingsManager>();
        
        switch (group)
        {
            case SoundGroup.Master:
                slider.onValueChanged.AddListener((float f) => { settings.MasterVol = f; });
                break;
            case SoundGroup.SFX:
                slider.onValueChanged.AddListener((float f) => { settings.SfxVol = f; });
                break;
            case SoundGroup.Music:
                slider.onValueChanged.AddListener((float f) => { settings.MusicVol = f; });
                break;
            case SoundGroup.Ambiance:
                slider.onValueChanged.AddListener((float f) => { settings.AmbianceVol = f; });
                break;
            case SoundGroup.UI:
                slider.onValueChanged.AddListener((float f) => { settings.UiVol = f; });
                break;
        }
    }

    private void Start()
    {
        ChangeSliderValue();
    }

    void OnEnable()
    {
        SettingsManager.OnSettingsLoaded += ChangeSliderValue;
    }

    void OnDisable()
    {
        SettingsManager.OnSettingsLoaded -= ChangeSliderValue;
    }

    void ChangeSliderValue()
    {
        switch (group)
        {
            case SoundGroup.Master:
                slider.value = settings.MasterVol;
                break;
            case SoundGroup.SFX:
                slider.value = settings.SfxVol;
                break;
            case SoundGroup.Music:
                slider.value = settings.MusicVol;
                break;
            case SoundGroup.Ambiance:
                slider.value = settings.AmbianceVol;
                break;
            case SoundGroup.UI:
                slider.value = settings.UiVol;
                break;
        }
    }
}
