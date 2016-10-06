using UnityEngine;
using System.Collections;

public class MiniMapDot : MonoBehaviour
{
    public WorldType thisWorldType;
    public enum WorldType { present, past, future };

    WorldChanger worldChanger;
    ColorAssigner colorAssigner;

    void Awake()
    {
        //  Find and assign references
        worldChanger = FindObjectOfType<WorldChanger>();
        colorAssigner = GetComponent<ColorAssigner>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //  Present
        if (thisWorldType == WorldType.present)
        {
            if (worldChanger.canSwitchPresent)
                colorAssigner.ChangeColor(Color.green, false, true, true);
            else
                colorAssigner.ChangeColor(Color.red, false, true, true);
        }
        //  Past
        else if (thisWorldType == WorldType.past)
        {
            if (worldChanger.canSwitchPast)
                colorAssigner.ChangeColor(Color.green, false, true, true);
            else
                colorAssigner.ChangeColor(Color.red, false, true, true);
        }
        //  Future
        else if (thisWorldType == WorldType.future)
        {
            if (worldChanger.canSwitchFuture)
                colorAssigner.ChangeColor(Color.green, false, true, true);
            else
                colorAssigner.ChangeColor(Color.red, false, true, true);
        }
    }
}
