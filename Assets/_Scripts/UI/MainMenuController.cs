using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using CameraTransitions;

public class MainMenuController : MonoBehaviour
{
    [Space(10)]
    public GameObject[] PanelObjects;
    public Camera[] Cameras;

    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;

    private int previousCameraIndex;
    private int currentCameraIndex;
    private CameraTransition cameraTransition;
    private bool isCurrentlyTransitioning = false;

    [Space(10)]
    [Header("Visual Settings")]
    [Range(0.0f, 1.0f)]
    public float PlayerMoveSpeed;
    public Camera parallaxCamera;

    private FreeParallax[] parallaxes;
    private CharacterController2D charController;

    public AudioClip[] timeWarps;
    private AudioSource menuSound;

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
        charController = FindObjectOfType<CharacterController2D>();
        parallaxes = FindObjectsOfType<FreeParallax>();
        menuSound = GetComponent<AudioSource>();
    }

    void Start()
    {
        //  Initial setups
        currentCameraIndex = 0;

        //  Disable all menu objects at start
        for (int i = 1; i < PanelObjects.Length; i++)
            PanelObjects[i].SetActive(false);

        //  Set up visual stuff

        //  Set up the cape helper
        CapePhysicsHelper capeHelper = FindObjectOfType<CapePhysicsHelper>();
        capeHelper.transform.position = charController.transform.position;
        capeHelper.capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        capeHelper.GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        //  Set the charController move speed to simulate walking
        charController.velocity = new Vector3(PlayerMoveSpeed, charController.velocity.y, charController.velocity.z);
        charController.animator.SetFloat("xVelocity", PlayerMoveSpeed);

        //  Update parallaxes
        for (int i = 0; i < parallaxes.Length; i++)
            parallaxes[i].Speed = parallaxCamera.velocity.x * PlayerMoveSpeed * -1;
    }
    
    public void TransitionToCamera(int CameraIndex)
    {
        previousCameraIndex = currentCameraIndex;

        PanelObjects[CameraIndex].SetActive(true);

        //  Cache the button position in normalized screen space coordinates.
        Vector2 transitionCenter = Cameras[currentCameraIndex].ScreenToViewportPoint(Input.mousePosition);

        playerAudio.randomizePitch(menuSound);
        int randomIndex = Random.Range(0, timeWarps.Length);
        menuSound.PlayOneShot(timeWarps[randomIndex], menuSound.volume * playerAudio.randomVolume());

        //  Perform the transition
        cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, Cameras[currentCameraIndex], Cameras[CameraIndex], transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });

        currentCameraIndex = CameraIndex;
    }

    public void BroadcastTransitionCompleteEvent()
    {
        PanelObjects[previousCameraIndex].SetActive(false);
    }  
}
