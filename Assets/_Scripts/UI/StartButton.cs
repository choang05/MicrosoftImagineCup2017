using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
  
    public void LoadSceneByIndex(int index)
    {
        GameManager.manager.CurrentScene++;
        SceneManager.LoadScene(index);
    }   
}