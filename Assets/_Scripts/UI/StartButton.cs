using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
  
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }   
}