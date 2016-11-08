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
}

public class SettingsManager : MonoBehaviour
{
    private int resolutionHeight;
    private int resolutionWidth;
    private bool isWindowed;
    private float masterVol;
    private float sfxVol;
    private float musicVol;
    //  References
    private static SettingsManager control;

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
        data.masterVol = MasterVol;
        data.sfxVol = SfxVol;
        data.musicVol = MusicVol;
        bf.Serialize(file, data);
        file.Close();

    }
    public void LoadSettings()
    {
        volumeChanger changer = FindObjectOfType<volumeChanger>();
        if (File.Exists(Application.persistentDataPath + "/settings.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/settings.dat", FileMode.Open);
            SettingsData data = (SettingsData)bf.Deserialize(file);
            ResolutionWidth = data.resolutionWidth;
            ResolutionHeight = data.resolutionHeight;
            IsWindowed = data.isWindowed;
            MasterVol = data.masterVol;
            SfxVol = data.sfxVol;
            MusicVol = data.musicVol;
        }
        else // assign defaults
        {
            ResolutionWidth = 800;
            ResolutionHeight = 600;
            IsWindowed = false;
            MasterVol = 0.5F;
            SfxVol = 0.5F;
            MusicVol = 0.5F;
        }
        changer.SetMasterLvl(MasterVol);
        changer.SetSfxLvl(SfxVol);
        changer.SetMusicLvl(MusicVol);
    }
}
