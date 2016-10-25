using UnityEngine;
using System.Collections;

public class ResolutionChanger : MonoBehaviour {

    public UnityEngine.UI.Slider slider;
    public UnityEngine.UI.Text resolutionText;
    public UnityEngine.UI.Toggle fullScreenToggle;
    
    public void ChangeResolution()
    {
        switch((int)slider.value)
        {
            case 1:
                Screen.SetResolution(800, 600, !GameManager.manager.IsWindowed);
                break;
            case 2:
                Screen.SetResolution(1024, 768, !GameManager.manager.IsWindowed);
                break;
            case 3:
                Screen.SetResolution(1280, 768, !GameManager.manager.IsWindowed);
                break;
            case 4:
                Screen.SetResolution(1366, 768, !GameManager.manager.IsWindowed);
                break;
        }
        
        
    }
}
