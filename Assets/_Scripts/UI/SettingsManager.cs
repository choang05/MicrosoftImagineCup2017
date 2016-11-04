using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
class SettingsData
{
    public int resolutionHeight;
    public int resolutionWidth;
    public bool isWindowed;
}

public class SettingsManager : MonoBehaviour
{
    private int resolutionHeight;
    private int resolutionWidth;
    private bool isWindowed;

    private int currentScene;

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
    #endregion

    public void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/settings.dat");

        SettingsData data = new SettingsData();
        data.isWindowed = IsWindowed;
        data.resolutionHeight = ResolutionHeight;
        data.resolutionWidth = ResolutionWidth;

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
        }
        else
        {
            ResolutionWidth = 800;
            ResolutionHeight = 600;
            IsWindowed = false;
        }

    }
}
