using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class ExitButton : MonoBehaviour
{

    public void SaveAndQuit()
    {

        //  Chad - this is a error. No reference. Wrote my alternative below
        //GameManager.manager.SaveData();
        FindObjectOfType<GameManager>().SavePlayerData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
