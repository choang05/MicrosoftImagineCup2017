using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
    public float speedX;
    public float speedY;
    public float speedZ;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    transform.Rotate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime, Space.Self);
    }
}
