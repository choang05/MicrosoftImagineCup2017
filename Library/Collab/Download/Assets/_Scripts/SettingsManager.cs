using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class SettingsData
{
    public int resolutionHeight;
    public int resolutionWidth;
    public bool isWindowed;
    public float masterVol;
    public float sfxVol;
    public float musicVol;
    public float ambianceVol;
    public float uiVol;
}

public class SettingsManager : MonoBehaviour
{
    private int resolutionHeight;
    private int resolutionWidth;
    private bool isWindowed;
    private float masterVol;
    private float sfxVol;
    private float musicVol;
    private float ambianceVol;
    private float uiVol;

    //  References
    private static SettingsManager control;

    public enum VolType { Master, SFX, Music, Ambiance, UI };
    public delegate void VolChangeHandler(VolType v, float f);
    public static event VolChangeHandler OnVolumeChanged;

    public delegate void SettingsLoadHandler();
    public static event SettingsLoadHandler OnSettingsLoaded;

    void Awake()
    {
        #region Dont Destroy On Load
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
            Destroy(gameObject);
        #endregion

        LoadSettings();
        SaveSettings();
    }

    #region Properties for fields
    public int ResolutionHeight
    {
        get
        {
            return resolutionHeight;
        }

        set
        {
            resolutionHeight = value;
        }
    }

    public int ResolutionWidth
    {
        get
        {
            return resolutionWidth;
        }

        set
        {
            resolutionWidth = value;
        }
    }

    public bool IsWindowed
    {
        get
        {
            return isWindowed;
        }

        set
        {
            isWindowed = value;
        }
    }

    public float MasterVol
    {
        get
        {
            return masterVol;
        }

        set
        {
            masterVol = value;
            if (OnVolumeChanged != null)
                OnVolumeChanged(VolType.Master, value);
        }
    }

    public float SfxVol
    {
        get
        {
            return sfxVol;
        }

        set
        {
            sfxVol = value;
            if (OnVolumeChanged != null)
                OnVolumeChanged(VolType.SFX, value);

        }
    }

    public float MusicVol
    {
        get
        {
            return musicVol;
        }

        set
        {
            musicVol = value;
            if (OnVolumeChanged != null)
                OnVolumeChanged(VolType.Music, value);

        }
    }

    public float AmbianceVol
    {
        get
        {
            return ambianceVol;
        }

        set
        {
            ambianceVol = value;
            if (OnVolumeChanged != null)
                OnVolumeChanged(VolType.Ambiance, value);

        }
    }

    public float UiVol
    {
        get
        {
            return uiVol;
        }

        set
        {
            uiVol = value;
            if (OnVolumeChanged != null)
                OnVolumeChanged(VolType.UI, value);

        }
    }
    #endregion

    public void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/settings.dat");

        SettingsData data = new SettingsData();
        data.isWindowed = IsWindowed;
        data.resolutionHeight = ResolutionHeight;
        data.resolutionWidth = ResolutionWidth;
        data.masterVol = masterVol;
        data.sfxVol = sfxVol;
        data.musicVol = musicVol;
        data.ambianceVol = ambianceVol;
        data.uiVol = uiVol;
        bf.Serialize(file, data);
        file.Close();

    }
    public void LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/settings.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/settings.dat", FileMode.Open);
            SettingsData data = (SettingsData)bf.Deserialize(file);
            ResolutionWidth = data.resolutionWidth;
            ResolutionHeight = data.resolutionHeight;
            IsWindowed = data.isWindowed;
            masterVol = data.masterVol;
            sfxVol = data.sfxVol;
            musicVol = data.musicVol;
            ambianceVol = data.ambianceVol;
            uiVol = data.uiVol;
            file.Close();
        }
        else // assign defaults
        {
            ResolutionWidth = Screen.resolutions[0].width;
            ResolutionHeight = Screen.resolutions[0].height;
            IsWindowed = true;
            masterVol = 1;
            sfxVol = 1;
            musicVol = 1;
            ambianceVol = 1;
            uiVol = 1;
        }

        Screen.SetResolution(ResolutionWidth, ResolutionHeight, IsWindowed);

        if (OnSettingsLoaded != null)
            OnSettingsLoaded();
    }
}
