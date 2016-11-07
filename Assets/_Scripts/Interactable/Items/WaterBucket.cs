using UnityEngine;
using System.Collections;

public class WaterBucket : MonoBehaviour
{
    public GameObject unfilledSprite;
    public GameObject filledSprite;
    public bool hasWater; 

    public void SetBucketWater(bool isFilled)
    {
        if (isFilled)
        {
            hasWater = true;

            unfilledSprite.SetActive(false);
            filledSprite.SetActive(true);
        }
        else
        {
            hasWater = false;

            unfilledSprite.SetActive(true);
            filledSprite.SetActive(false);
        }
    }
}
