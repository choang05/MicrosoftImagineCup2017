using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;

    private FacingDirection facingDirection;       
    enum FacingDirection { Left, Right }

    private float randomForce;

    //  Audio
    AudioSource audioSource;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {
        facingDirection = FacingDirection.Left;

        randomForce = Random.Range(0.0f, 6.0f);

        rigidBody2D.AddForce(Vector2.left * randomForce, ForceMode2D.Impulse);
        rigidBody2D.AddTorque(-5);

        StartCoroutine(FloatInWater());
        StartCoroutine(QuackRandomly());
	}

    IEnumerator FloatInWater()
    {
        while (true)
        {
            float idleDuration = Random.Range(2.0f, 10.0f);

            randomForce = Random.Range(0.0f, 6.0f);

            yield return new WaitForSeconds(idleDuration);

            transform.Rotate(new Vector3(0, 180, 0));

            //  Add force in oppisite direction
            if (facingDirection == FacingDirection.Left)
            {
                facingDirection = FacingDirection.Right;
                rigidBody2D.AddForce(Vector2.right * randomForce, ForceMode2D.Impulse);
                rigidBody2D.AddTorque(-5);
            }
            else
            {
                facingDirection = FacingDirection.Left;
                rigidBody2D.AddForce(Vector2.left * randomForce, ForceMode2D.Impulse);
                rigidBody2D.AddTorque(5);
            }


            //Debug.Log("Duck changed direction after" + idleDuration);
        }
    }

    IEnumerator QuackRandomly()
    {
        while (true)
        {
            float idleDuration = Random.Range(0.0f, 5.0f);

            yield return new WaitForSeconds(idleDuration);

            float pitch = Random.Range(.75f, 1.25f);

            audioSource.pitch = pitch;
            audioSource.Play();
        }
    }
}
