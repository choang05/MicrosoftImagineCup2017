using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueGame : MonoBehaviour
{
    GameManager gameManager;

    void Awake()
    {
        //  Chad - if its a new game, set the AreaID to 0 in the GameManager script

        gameManager = FindObjectOfType<GameManager>();

        /*if (GameManager.manager.IsUserNew)
            gameObject.SetActive(false);
        else gameObject.SetActive(true);
        */
    }

    public void LoadLastScene()
    {
        //  Chad - always load the master scene which is index 0
        //  Chad - Save the last AreaID the player was in. Assign that value in the GameManager and that will be determine the continue checkpoint

        //  Load in the save file
        gameManager.LoadData();

        //  Start the Master Scene
        SceneManager.LoadScene(1);

        //SceneManager.LoadScene(GameManager.manager.LastPuzzle);
    }
}
