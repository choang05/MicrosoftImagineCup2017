using UnityEngine;
using System.Collections;

public class CameraHelper : MonoBehaviour
{
    public Camera PresentCamera;
    public Camera PastCamera;
    public Camera FutureCamera;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        PresentCamera.orthographicSize = mainCamera.orthographicSize;
        PastCamera.orthographicSize = mainCamera.orthographicSize;
        FutureCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
