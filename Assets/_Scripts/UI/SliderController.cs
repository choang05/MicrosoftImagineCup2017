using UnityEngine;
using System.Collections;

public class SliderController : MonoBehaviour {

    public volumeChanger changer;
    public enum SoundGroup { Master, SFX, Music };
    public SoundGroup group;
    public UnityEngine.UI.Slider slider;
    public AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        
    }
    public void UpdateValue()
    {
        int value = (int)slider.value;

        if(group == SoundGroup.Master)
        {
            changer.SetMasterLvl(value);
        }
        else if(group == SoundGroup.SFX)
        {
            changer.SetSfxLvl(value);
        }
        else if(group == SoundGroup.Music)
        {
            changer.SetMusicLvl(value);
        }

    }
    
    public void PlaySound()
    {
        source.PlayOneShot(source.clip);
        
    }
}
