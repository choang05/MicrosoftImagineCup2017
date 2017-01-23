using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class ExitButton : MonoBehaviour
{

    public void SaveAndQuit()
    {

        //FindObjectOfType<SettingsManager>().SaveSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
