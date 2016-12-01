using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldIndicator : MonoBehaviour {

    public enum World { Past, Present, Future };
    public World world;

    public void OnEnable()
    {
        Checkpoint.OnCheckpointReached += ShowHideIndicators;
    }
    public void OnDisable()
    {
        Checkpoint.OnCheckpointReached -= ShowHideIndicators;
    }
    public void ShowHideIndicators(int checkpointID)
    {
        if(world == World.Present)
        {
            // Chad - code to grow present indicator size gradually using animation goes here. Scale the size of this gameobject to 1.5

        }
        if(checkpointID > 55 && checkpointID < 100)
        {
            if(world == World.Future)
            {
                // Chad - code to grow future indicator size gradually using animation goes here. Scale the size of this gameobject to 1.0

            }
        }
        else if(checkpointID >= 100)
        {
            if(world == World.Past || world == World.Future)
            {
                // Chad - code to grow past/future indicator size gradually using animation goes here. Scale the size of this gameobject to 1.0

            }
        }

    }
	
}
