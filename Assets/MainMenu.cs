using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

    public GameObject menu;
    private LinkedList<GameObject> navigation;
    void Awake()
    {
        navigation = new LinkedList<GameObject>();
        navigation.AddLast(menu);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (navigation.Count != 1)
            {
                navigation.Last.Value.SetActive(false);
                navigation.Last.Previous.Value.SetActive(true);
                navigation.RemoveLast();
            }
        }
    }
    public void AddNavigation(GameObject panel)
    {
        navigation.AddLast(panel);
    }
    public void RemoveNavigation()
    {
        navigation.RemoveLast();
    }
}
