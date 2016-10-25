using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueGame : MonoBehaviour {
    public void LoadLastScene()
    {
        SceneManager.LoadScene(GameManager.manager.LastPuzzle);
    }

    void Awake()
    {
        if (GameManager.manager.IsUserNew)
            gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

	
}
