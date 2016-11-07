using UnityEngine;

public class TextChanger : MonoBehaviour
{
    UnityEngine.UI.Text text;
    void Awake()
    {
        text = GetComponent<UnityEngine.UI.Text>();

    }
    public void ChangeSliderText(float sliderVal)
    {
        text.text = ((int)(sliderVal*100)).ToString();
    }
        
}
