using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class volumeChanger : MonoBehaviour {

    public AudioMixer AudioManager;

    private void OnEnable()
    {
        SettingsManager.OnVolumeChanged += ChangeVolume;
    }

    private void OnDisable()
    {
        SettingsManager.OnVolumeChanged -= ChangeVolume;
    }


    public void ChangeVolume(SettingsManager.VolType v, float f)
    {
            switch (v)
            {
                case SettingsManager.VolType.Master:
                    SetMasterLvl(f);
                    break;
                case SettingsManager.VolType.SFX:
                    SetSfxLvl(f);
                    break;
                case SettingsManager.VolType.Music:
                    SetMusicLvl(f);
                    break;
                case SettingsManager.VolType.Ambiance:
                    SetAmbianceLvl(f);
                    break;
                case SettingsManager.VolType.UI:
                    SetUilvl(f);
                    break;
            }
    }

    public void SetMasterLvl(float masterLvl)
    {
        AudioManager.SetFloat("masterVol", masterLvl);
    }

    public void SetSfxLvl(float sfxLvl)
    {
        AudioManager.SetFloat("sfxVol", sfxLvl);
    }

    public void SetMusicLvl(float musicLvl)
    {
        AudioManager.SetFloat("musicVol", musicLvl);
    }

    public void SetAmbianceLvl(float ambLvl)
    {
        AudioManager.SetFloat("ambianceVol", ambLvl);
    }

    public void SetUilvl(float uiLvl)
    {
        AudioManager.SetFloat("uiVol", uiLvl);
    }
}
