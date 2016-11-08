using UnityEngine;

public class SliderController : MonoBehaviour
{
    private UnityEngine.UI.Slider slider;
    public SettingsManager settings;
    public enum SoundGroup { Master, SFX, Music }
    public SoundGroup group;
    /*void Awake()
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
        }

    }
    void OnEnable()
    {
        switch(group)
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
        }
    }*/
}
