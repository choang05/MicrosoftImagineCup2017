using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

[Serializable]
class GameData
{
    public int resolutionHeight;
    public int resolutionWidth;
    public int lastPuzzle;
    public bool isWindowed;
    public bool isUserNew;
}

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public Transform RespawnNode;

    private int resolutionHeight;
    private int lastPuzzle;
    private bool isWindowed;
    private int resolutionWidth;
    private bool isUserNew;
    private bool isPaused;
    private int currentScene;

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
    public int LastPuzzle
    {
        get
        {
            return lastPuzzle;
        }

        set
        {
            lastPuzzle = value;
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

    public bool IsUserNew
    {
        get
        {
            return isUserNew;
        }

        set
        {
            isUserNew = value;
        }
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }

        set
        {
            isPaused = value;
        }
    }

    public int CurrentScene
    {
        get
        {
            return currentScene;
        }

        set
        {
            currentScene = value;
        }
    }

    void Update()
    {
        // if Esc is pressed, enable panel and pause game
        if (Input.GetKeyUp(KeyCode.Escape) && CurrentScene != 0 && !isPaused)
        {
            GameObject resumePanel = GameObject.Find("MainMenuPanel");
            resumePanel.SetActive(true);
            IsPaused = true;
        }
        else if(Input.GetKeyUp(KeyCode.Escape) && CurrentScene != 0 && isPaused)
        {
            GameObject resumePanel = GameObject.Find("MainMenuPanel");
            resumePanel.SetActive(false);
            IsPaused = false;
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }
    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
            IsPaused = false;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
        LoadData();
        CurrentScene = 0;
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/settings.dat");

        GameData data = new GameData();
        data.isWindowed = IsWindowed;
        data.resolutionHeight = ResolutionHeight;
        data.resolutionWidth = ResolutionWidth;
        data.lastPuzzle = LastPuzzle;
        data.isUserNew = IsUserNew;

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "settings.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "settings.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);

            ResolutionHeight = data.resolutionHeight;
            ResolutionWidth = data.resolutionWidth;
            IsWindowed = data.isWindowed;
            LastPuzzle = data.lastPuzzle;
            IsUserNew = data.isUserNew;
        }
        else
        {
            ResolutionHeight = 600;
            ResolutionWidth = 800;
            IsWindowed = false;
            LastPuzzle = 1;
            IsUserNew = true;
        }
    }

}
