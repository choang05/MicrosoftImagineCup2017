using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public float wanderRadius;

    private PolyNavAgent polyNavAgent;
    private PolyNav2D polyNav2D;
    private PolygonCollider2D polyColl2D;

    private void Awake()
    {
        polyNavAgent = GetComponent<PolyNavAgent>();
        polyNav2D = FindObjectOfType<PolyNav2D>();
        polyColl2D = GetComponent<PolygonCollider2D>();
    }

    // Use this for initialization
    void Start ()
    {
        //  Change color of butterfly, wings can be transparent
        SpriteRenderer[] spriteRends = GetComponentsInChildren<SpriteRenderer>();
        spriteRends[0].color = new Color(Random.value, Random.value, Random.value, 1);
        spriteRends[1].color = new Color(Random.value, Random.value, Random.value, Random.Range(0.5f, 1));

        //  Start wing flap animation at random start frame
        Animation anim = GetComponent<Animation>();
        anim["Butterfly_FlapWings"].time = Random.Range(1, anim.clip.length);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RandomlyWander();
    }

    private void RandomlyWander()
    {
        if (!polyNavAgent.hasPath)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle * wanderRadius;
            randomCirclePoint = new Vector3(randomCirclePoint.x + transform.position.x, randomCirclePoint.y + transform.position.y);

            //Debug.Log("random circle point: " + randomCirclePoint);

            //if (polyNav2D.PointIsValid(randomCirclePoint))
            //{
                //Debug.Log("Point is valid");

                polyNavAgent.SetDestination(randomCirclePoint);
            //}
        }
    }
}
