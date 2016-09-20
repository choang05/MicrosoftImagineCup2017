using UnityEngine;
using System.Collections;

public class WeightTrap : MonoBehaviour
{
    public float fallSpeed;
    public float returnSpeed;
    public float collideIdleTime;
    public float returnIdleTime;

    private Vector2 initialPos;
    private bool isCollided = false;
    private bool isFalling;

	// Use this for initialization
	void Start ()
    {
        initialPos = transform.position;

        StartCoroutine(PerformTrap());
	}

    //  idle time for weighted trap
    IEnumerator PerformTrap()
    {
        while (true)
        {
            //  if the weight trap has not collided with anything yet... keep falling
            while (!isCollided)
            {
                isFalling = true;

                transform.position += Vector3.down * Time.deltaTime * fallSpeed;

                yield return null;
            }

            //  if the weighted trap has collided, do idle time and return to initial position
            if (isCollided)
            {
                yield return new WaitForSeconds(collideIdleTime);

                while (Vector2.Distance(transform.position, initialPos) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, initialPos, Time.deltaTime * returnSpeed);
                    yield return null;
                }

                isCollided = false;

                yield return new WaitForSeconds(returnIdleTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) && isFalling)
        {

            //  Cache the player's character controller
            CharacterController2D charController = other.GetComponent<CharacterController2D>();

            //  Perform death
            charController.Die();
        }

        isFalling = false;
        isCollided = true;
    }
}
