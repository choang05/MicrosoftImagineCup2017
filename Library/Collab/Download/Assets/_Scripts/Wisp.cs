using UnityEngine;
using System.Collections;

public class Wisp : MonoBehaviour
{
    public int minimumCheckpointIDToEnable;

    public enum WispType { Past, Present, Future };
    public WispType wispType;

    //  User-Assigned
    public float minDistanceFromPlayer;
    public float followTime;
    public float wispFollowTime;

    //  Private
    private Transform playerTransform;
    private SphereCollider sphereColl;
    private Vector3 followVelocity = Vector3.zero;
    private Vector3 wispVelocity = Vector3.zero;

    //  References
    GameManager gameManager;

    public delegate void WispEvent(WispType wispType);
    public static event WispEvent OnWispAdd;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= OnPlayerSpawned;
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        sphereColl = GetComponent<SphereCollider>();
    }

	// Use this for initialization
	void Start ()
    {
        if (GameManager.CurrentCheckpointID < minimumCheckpointIDToEnable)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //  Update the GameManager
            switch (wispType)
            {
                case WispType.Past:
                    gameManager.hasPastWisp = true;
                    break;
                case WispType.Present:
                    gameManager.hasPresentWisp = true;
                    break;
                case WispType.Future:
                    gameManager.hasFutureWisp = true;
                    break;
            }

            if (OnWispAdd != null)
                OnWispAdd(wispType);
        }

        //wisp1.position = Vector3.zero;

        //StartCoroutine(CoWispFollow());
	}

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playerTransform.position);

        if (Vector2.Distance(transform.position, playerTransform.position) >= minDistanceFromPlayer)
            transform.position = Vector3.SmoothDamp(transform.position, playerTransform.position, ref followVelocity, followTime * Time.deltaTime);
    }

    /*IEnumerator CoWispFollow()
    {
        while(true)
        {
            Vector3 targetPosition = (Random.insideUnitSphere * sphereColl.radius) + transform.position;
            while (Vector3.Distance(wisp1.position, targetPosition) > 0.5f)
            {
                //Debug.Log(targetPosition);
                //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
                wisp1.transform.position = Vector3.SmoothDamp(wisp1.transform.position, targetPosition, ref wispVelocity, wispFollowTime * Time.deltaTime);
                yield return null;
            }
            //yield return new WaitForSeconds(0.2f);
        }
    }*/

    private void OnPlayerSpawned()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;

        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);

    }
}
