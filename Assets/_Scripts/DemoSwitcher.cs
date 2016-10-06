using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DemoSwitcher : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
