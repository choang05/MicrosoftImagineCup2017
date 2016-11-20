using UnityEngine;

public class TextChanger : MonoBehaviour
{
    private UnityEngine.UI.Text text;
    
    public void ChangeSliderText(float sliderVal)
    {
        text = GetComponent<UnityEngine.UI.Text>();
        text.text = ((int)(sliderVal+80)).ToString();
    }
        
}
