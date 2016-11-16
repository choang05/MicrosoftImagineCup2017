using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using CameraTransitions;

public class MainMenuChanger : MonoBehaviour
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

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        currentCameraIndex = 0;

        //  Disable all menu objects at start
        for (int i = 1; i < PanelObjects.Length; i++)
        {
            PanelObjects[i].SetActive(false);
        }
    }
    
    public void TransitionToCamera(int CameraIndex)
    {
        previousCameraIndex = currentCameraIndex;

        PanelObjects[CameraIndex].SetActive(true);

        cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, Cameras[currentCameraIndex], Cameras[CameraIndex], transitionDuration, new object[] { false, transitionEdgeSmoothness });

        currentCameraIndex = CameraIndex;
    }

    public void BroadcastTransitionCompleteEvent()
    {
        PanelObjects[previousCameraIndex].SetActive(false);
    }  
}
