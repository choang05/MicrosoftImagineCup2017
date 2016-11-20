using UnityEngine;
using System.Collections;

namespace BeautifyEffect
{
	public class Demo1 : MonoBehaviour
	{
		GUIStyle labelStyle;

		void OnGUI ()
		{
			Rect rect = new Rect (20, 20, Screen.width - 20, 30);
			GUI.Label (rect, "Press T to toggle Beautify on/off.");

			rect = new Rect (20, 40, Screen.width - 20, 30);
			GUI.Label (rect, "To customize the effects, select your camera and scroll down to Beautify component in the inspector.");

			if (labelStyle == null) {
				labelStyle = new GUIStyle(GUI.skin.label);
				labelStyle.fontStyle = FontStyle.Bold;
			}
			rect = new Rect (20, 60, Screen.width - 20, 30);
			if (Beautify.instance.enabled) {
				GUI.Label (rect, "BEAUTIFY ON", labelStyle);
			} else {
				GUI.Label (rect, "BEAUTIFY OFF", labelStyle);
			}
		}

		// Update is called once per frame
		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.T))  Beautify.instance.enabled = !Beautify.instance.enabled;
	
		}
	}
}
