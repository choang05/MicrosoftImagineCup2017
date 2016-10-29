using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour {

    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;

    Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
            Pause();
        }
	}

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Lowpass();
    }

    void Lowpass()
    {
        if (Time.timeScale == 0)
            paused.TransitionTo(.01f);
        else
            unpaused.TransitionTo(.01f);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
