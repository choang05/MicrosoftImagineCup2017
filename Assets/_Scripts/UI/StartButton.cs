using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
  
    public void LoadSceneByIndex(int index)
    {
        //  Chad - no reference error. Wrote alternative below
        //GameManager.manager.CurrentScene++;
        FindObjectOfType<GameManager>().CurrentScene++;

        SceneManager.LoadScene(index);
    }   
}