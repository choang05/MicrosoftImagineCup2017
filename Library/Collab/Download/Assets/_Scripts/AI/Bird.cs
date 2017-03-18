using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bird : MonoBehaviour
{
    //  User Defined Variables
    public Vector2 xJumpRange;
    public float jumpPower = 1;

    [Space(10)]
    [Header("Color")]
    public Renderer bodyRend;
    public Renderer frontWingRend;
    public Renderer backWingRend;
    public Renderer tailRend;

    //  Animation
    Animator animator;
    int isFlyingHash = Animator.StringToHash("isFlying");
    int flapWingTriggerHash = Animator.StringToHash("flapWingTrigger");

    //  Audio
    private AudioSource audioSource;
    public AudioClip barkAudio;
    public AudioClip flyingAudio;

    private FacingDirection facingDirection;
    enum FacingDirection { Left, Right }

    private State currentState;
    enum State { Idle, Flying }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        SetRandomColor();

        //StartCoroutine(CoIdle());
        StartCoroutine(CoBarkRandomly());
    }

    IEnumerator CoBarkRandomly()
    {
        while (true)
        {
            float idleDuration = Random.Range(0.0f, 5.0f);

            yield return new WaitForSeconds(idleDuration);

            float pitch = Random.Range(.7f, 1);

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(barkAudio);
        }
    }

    IEnumerator CoIdle()
    {
        while (true)
        {
            //  IDLE
            int randomActionID = Mathf.RoundToInt(Random.Range(0, 3));

            switch (randomActionID)
            {
                //  Do nothing
                case 0:
                    float idleDuration = Random.Range(0.0f, 3.0f);
                    yield return new WaitForSeconds(idleDuration);
                    break;
                case 1:
                    //  Flap wings
                    animator.SetTrigger(flapWingTriggerHash);
                    yield return new WaitForSeconds(.5f);
                    break;
                case 2:
                    //  Hop and rotate
                    int randomTurnID = Mathf.RoundToInt(Random.Range(0, 1));
                    if (randomTurnID == 0)
                        transform.Rotate(new Vector3(0, 180, 0));

                    Vector3 jumpPos = new Vector3(transform.position.x + Random.Range(xJumpRange.x, xJumpRange.y), transform.position.y, transform.position.z);
                    transform.DOJump(jumpPos, jumpPower, 1, .25f);

                    yield return new WaitForSeconds(.25f);
                    break;
            }
        }
    }

    public void FlyTo(Vector3 flyToPos, float flightDuration)
    {
        StartCoroutine(CoFlyTo(flyToPos, flightDuration));
    }

    IEnumerator CoFlyTo(Vector3 flyToPos, float flightDuration)
    {
        currentState = State.Flying;

        //  Animation
        animator.SetBool(isFlyingHash, true);

        transform.DOMove(flyToPos, flightDuration, false);

        //  Update facing direction
        if (transform.position.x < flyToPos.x && facingDirection == FacingDirection.Left)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            facingDirection = FacingDirection.Right;
        }
        else if (transform.position.x > flyToPos.x && facingDirection == FacingDirection.Right)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            facingDirection = FacingDirection.Left;
        }

        yield return new WaitForSeconds(flightDuration);

        currentState = State.Idle;

        //  Animation
        animator.SetBool(isFlyingHash, false);

        StartCoroutine(CoIdle());
    }

    void SetRandomColor()
    {
        //  Assign random color
        Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        bodyRend.material.color = newColor;

        //  Deviate color a bit
        newColor.r -= .01f;
        newColor.g -= .01f;
        newColor.b -= .01f;
    
        frontWingRend.material.color = newColor;
        backWingRend.material.color = newColor;
        tailRend.material.color = newColor;
    }
}
