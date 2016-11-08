using UnityEngine;

public class SliderController : MonoBehaviour {

    private UnityEngine.UI.Slider slider;
    public enum SoundGroup { Master, SFX, Music }
    public SoundGroup group;
    void Awake()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
        settings = FindObjectOfType<SettingsManager>();
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
    }
}
