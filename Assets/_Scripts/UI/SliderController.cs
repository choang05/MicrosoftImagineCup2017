using UnityEngine;

public class SliderController : MonoBehaviour {

    private UnityEngine.UI.Slider slider;
    public enum SoundGroup {Master, SFX, Music };
    public SoundGroup Group;
    void OnEnable()
    {
        SettingsManager.OnSettingsLoaded += UpdateSlider;
    }
    void OnDisable()
    {
        SettingsManager.OnSettingsLoaded -= UpdateSlider;
    }
    void Awake()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
    }
    public void UpdateSlider(SettingsData data)
    {
        
        switch(Group)
        {
            case SoundGroup.Master:
                slider.value = data.masterVol;
                break;
            case SoundGroup.SFX:
                slider.value = data.sfxVol;
                break;
            case SoundGroup.Music:
                slider.value = data.musicVol;
                break;
        }
    }
}
