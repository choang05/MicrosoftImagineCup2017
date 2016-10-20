using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class volumeChanger : MonoBehaviour {

    public AudioMixer AudioManager;

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
}
