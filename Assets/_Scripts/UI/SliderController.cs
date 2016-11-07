using UnityEngine;
using System.Collections;

public class SliderController : MonoBehaviour {

    public volumeChanger changer;
    public enum SoundGroup { Master, SFX, Music, Ambiance, UI };
    public SoundGroup group;
    public UnityEngine.UI.Slider slider;

    public void UpdateValue()
    {
        float vol = (float)slider.value;

        if(group == SoundGroup.Master)
        {
            changer.SetMasterLvl(vol);
        }
        else if(group == SoundGroup.SFX)
        {
            changer.SetSfxLvl(vol);
        }
        else if(group == SoundGroup.Music)
        {
            changer.SetMusicLvl(vol);
        }
        else if(group == SoundGroup.Ambiance)
        {
            changer.SetAmbianceLvl(vol);
        }
        else if(group == SoundGroup.UI)
        {
            changer.SetUilvl(vol);
        }

    }
}
