using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    //  User Parameters variables
    public int impactForceThreshold;                                //  The threshold reached to to kill player caused by colliding object's collision.impulse magnitude          	

    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    #region DieByWater()
    public void DieByWater()
    {
        //  Animation

        ProcessRespawn();

        Debug.Log("Player died by drowning!");
    }
    #endregion

    #region DieByImpact()
    public void DieByImpact()
    {
        ProcessRespawn();

        Debug.Log("Player died!");
    }
    #endregion

    #region ProcessRespawn()
    public void ProcessRespawn()
    {
        gameManager.Respawn(); 
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
