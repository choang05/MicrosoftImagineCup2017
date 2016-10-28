using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    //  User Parameters variables
    public int impactForceThreshold;                //  The threshold reached to to kill player caused by colliding object's collision.impulse magnitude          	

    //  References
    private GameManager gameManager;
    private CharacterController2D charController;

    //  Animation
    private Animator animator;
    int drownTriggerHash = Animator.StringToHash("drownTrigger");

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        charController = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
    }

    #region DieByWater()
    public void DieByWater()
    {
        //  Stop player from moving
        charController.canMove = false;
        
        //  Animation
        animator.SetTrigger(drownTriggerHash);

        //Debug.Log("Player died by drowning!");
    }
    #endregion

    #region DieByImpact()
    public void DieByImpact()
    {
        ProcessRespawn();

        Debug.Log("Player died by impact!");
    }
    #endregion

    #region ProcessRespawn(): called from animation death events
    public void ProcessRespawn()
    {
        gameManager.StartCoRespawn(); 
    }
    #endregion

    #region ProcessImpact(): Evaluate collision impacts
    //  called when player impacted by colliding object
    public void ProcessImpact(Vector3 collisionForce)
    {
        //Debug.Log(collisionForce.magnitude);

        //  Evaluate force and see if its enough to kill the player
        if (collisionForce.magnitude >= impactForceThreshold)
        {
            DieByImpact();
        }
    }
    #endregion

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        //  If player collides with a trap, perform death function
        if (other.CompareTag(Tags.Trap))
            DieByImpact();
        else if (other.CompareTag(Tags.Water))
            DieByWater();
    }
}
