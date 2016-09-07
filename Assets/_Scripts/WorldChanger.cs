using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldChanger : MonoBehaviour
{
    public Transform player;

    [Space(10)]
    public Transform PresentMask;
    public GameObject PresentObjects;
    public Transform PastMask;
    public GameObject PastObjects;
    public Transform FutureMask;
    public GameObject FutureObjects;

    [Space(10)]
    public Shader MaskOneShader;
    public Shader ObjectOneShader;
    public Shader MaskTwoShader;
    public Shader ObjectTwoShader;

    public WorldState CurrentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool canSwitch = true;
    public float transitionSpeed = 3;
	
    void Start()
    {
        //  Initial setups
        CurrentWorldState = WorldState.Present;

        PresentMask.localScale = new Vector3((Camera.main.orthographicSize * 2 / Screen.height * Screen.width) * 3, Camera.main.orthographicSize * 5, 1);
        PastMask.localScale = new Vector3(0, 0, 1);
        PastObjects.SetActive(false);
        FutureObjects.SetActive(false);
    }

	// Update is called once per frame
	void Update ()
    {
        //  Evaluate input from player. 1-3 selects which world to transition to
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SwitchWorld(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SwitchWorld(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SwitchWorld(3);
        }
    }
    
    public void SwitchWorld(int worldID)
    {
        //  Calculate which world and mask we need to unload
        Transform UnloadingWorldMask;
        GameObject UnloadingWorld;
        if (CurrentWorldState == WorldState.Present)
        {
            UnloadingWorldMask = PresentMask;
            UnloadingWorld = PresentObjects;
        }
        else if (CurrentWorldState == WorldState.Past)
        {
            UnloadingWorldMask = PastMask;
            UnloadingWorld = PastObjects;
        }
        else 
        {
            UnloadingWorldMask = FutureMask;
            UnloadingWorld = FutureObjects;
        }

        //  If player is allowed to switch...
        if (canSwitch)
        {
            //  Determine which world ID to switch to and check if world is already active.
            if (worldID == 1 && CurrentWorldState != WorldState.Present)
            {
                CurrentWorldState = WorldState.Present;
                StartCoroutine(MaskTransition(PresentObjects, PresentMask, UnloadingWorld, UnloadingWorldMask));
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
                //Debug.Log("Present");
            }
            else if (worldID == 2 && CurrentWorldState != WorldState.Past)
            {
                StartCoroutine(MaskTransition(PastObjects, PastMask, UnloadingWorld, UnloadingWorldMask));
                CurrentWorldState = WorldState.Past;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 15);
                //Debug.Log("Past");
            }
            else if (worldID == 3 && CurrentWorldState != WorldState.Future)
            {
                StartCoroutine(MaskTransition(FutureObjects, FutureMask, UnloadingWorld, UnloadingWorldMask));
                CurrentWorldState = WorldState.Future;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 30);
                //Debug.Log("Future");
            }
        }
    }

    //  Ienumerator that creates the transition effect
    IEnumerator MaskTransition(GameObject LoadingWorld, Transform LoadingWorldMask, GameObject UnloadingWorld, Transform UnloadingWorldMask)
    {
        //  Disable ability to switch world until transition completes.
        canSwitch = false;

        //  Set active scene to load
        LoadingWorld.SetActive(true);
        
        //  Calculate size to scale the mask to entire screen
        float height = Camera.main.orthographicSize * 2.0f;
        float width = height / Screen.height * Screen.width;
        Vector3 destinationScale = new Vector3(width * 3, height * 3, 1);

        //  set mask position to translate at the player position
        LoadingWorldMask.position = new Vector3(player.position.x, player.position.y, LoadingWorldMask.position.z);

        //  scaling function
        float progress = 0;
        while (progress <= 1)
        {
            LoadingWorldMask.localScale = Vector3.Lerp(new Vector3(0, 0, 1), destinationScale, progress);
            progress += Time.deltaTime * transitionSpeed;
            yield return null;
        }        

        //  Reassign shaders
        AssignNewShaders(LoadingWorld, UnloadingWorld);

        //  set the previous world to inactive
        UnloadingWorldMask.localScale = new Vector3(0, 0, 1);
        UnloadingWorld.SetActive(false);
        
        //  Enable ability to switch worlds
        canSwitch = true;
    }

    void AssignNewShaders(GameObject LoadingWorld, GameObject UnloadingWorld)
    {
        //  Change the loaded's shaders to mask & object one
        Renderer[] loadRenders = LoadingWorld.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in loadRenders)
        {
            if (rend.CompareTag(Tags.WorldMask))
                rend.material.shader = MaskOneShader;
            else
                rend.material.shader = ObjectOneShader;
        }

        //  Change the unloaded world's shaders to mask & object two
        Renderer[] unloadRenders = UnloadingWorld.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in unloadRenders)
        {
            if (rend.CompareTag(Tags.WorldMask))
                rend.material.shader = MaskTwoShader;
            else
                rend.material.shader = ObjectTwoShader;
        }
    }
}
