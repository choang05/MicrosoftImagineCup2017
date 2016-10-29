using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//  This script is used to assign a random color and it's indicator UI.
//  Simply attach this to any target that has the 'IndicatorTarget' component on it.

public class ColorAssigner : MonoBehaviour
{
    //  Settings & options
    [Header("Settings")]
    public Color Color = Color.red;
    [Tooltip("Use a random color. Will override 'NewColor'")]
    public bool RandomColor = false;
    [Tooltip("Should this gameobject be set to the new color?")]
    public bool ChangeGameobjectColor = true;
    [Tooltip("Should this gameobject's children be set to the new color?")]
    public bool ChangeChildrenColor = true;

    // Use this for initialization
	void Start ()
    {
        ChangeColor(Color, RandomColor, ChangeGameobjectColor, ChangeChildrenColor);
    }

    public void ChangeColor(Color newColor, bool random, bool changeGO, bool changeChildren)
    {
        //  Get a new random color if enabled
        if (random)
            newColor = new Color(UnityEngine.Random.Range(0f, 1f), Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

        //  Change color of this gameobject.
        if (changeGO)
        {
            //  Gameobject
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().material.color = newColor;
        }

        //  Change color of this gameobject's children
        if (changeChildren)
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
                renders[i].material.color = newColor;
        }
    }
}
