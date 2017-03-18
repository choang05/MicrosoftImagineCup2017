using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public Text StatusText;
    public InputField usernameInput;
    public InputField passwordInput;

    private Button[] loginPanelButtons;
    private AccountManager accountManager;
    private MainMenuController mainMenuController;

    //  Events
    public delegate void LoginPanelEvent();
    public static event LoginPanelEvent OnPlayOffline;

    private void Awake()
    {
        accountManager = FindObjectOfType<AccountManager>();
        mainMenuController = FindObjectOfType<MainMenuController>();
        loginPanelButtons = GetComponentsInChildren<Button>();
    }

    private void OnEnable()
    {
        AccountManager.OnStatusUpdate += UpdateStatus;
    }

    private void OnDisable()
    {
        AccountManager.OnStatusUpdate -= UpdateStatus;
    }

    public void ProcessRegistration()
    {
        //StatusText.text = "Registering...";

        StartCoroutine(AnimateStatusText("Registering, please wait"));

        //  Disable buttons incase of spamming
        ToggleLoginButtons(false);
        toggleInputField(false);

        //  Check if Username & Password format is acceptable
        if (IsInputAccectable(usernameInput.text, passwordInput.text))
        {
            accountManager.RegisterUser(usernameInput.text, passwordInput.text);
        }
    }

    public void ProcessLogin()
    {
        //StatusText.text = "Logging in...";

        StartCoroutine(AnimateStatusText("Logging in, please wait"));

        //  Disable buttons incase of spamming
        ToggleLoginButtons(false);
        toggleInputField(false);

        //  Check if Username & Password format is acceptable
        if (IsInputAccectable(usernameInput.text, passwordInput.text))
        {
            accountManager.LoginUser(usernameInput.text, passwordInput.text);
        }
    }

    private bool IsInputAccectable(string username, string password)
    {
        //  Check if Username has spaces in it
        if (!username.Contains(" "))
        {
            //  Check if Username has reasonable length
            if (username.Length > 3 && username.Length <= 64)
            {
                //  Check if Password has spaces in it
                if (!password.Contains(" "))
                {
                    //  Check if Password has reasonable length
                    if (password.Length >= 4)
                    {
                        return true;
                    }
                    else
                    {
                        StopAllCoroutines();
                        StatusText.text = "Password is too short!";
                    }
                }
                else
                {
                    StopAllCoroutines();
                    StatusText.text = "Password cannot contain spaces!";
                }
            }
            else
            {
                StopAllCoroutines();
                StatusText.text = "Username is too short or too long!";
            }
        }
        else
        {
            StopAllCoroutines();
            StatusText.text = "Username cannot contain spaces!";
        }

        ToggleLoginButtons(true);
        toggleInputField(true);

        return false;
    }

    private void UpdateStatus(string status)
    {
        StopAllCoroutines();

        StatusText.text = status;

        //  ReEnable inputs
        ToggleLoginButtons(true);
        toggleInputField(true);
    }

    public void OnPlayOfflineButton()
    {
        if (OnPlayOffline != null) OnPlayOffline();

        toggleInputField(false);
        ToggleLoginButtons(false);
    }

    public void ToggleLoginButtons(bool isEnabled)
    {
        for (int i = 0; i < loginPanelButtons.Length; i++)
            loginPanelButtons[i].interactable = isEnabled;
    }

    public void toggleInputField(bool isEnabled)
    {
        usernameInput.interactable = isEnabled;
        passwordInput.interactable = isEnabled;
    }

    IEnumerator AnimateStatusText(string message)
    {
        StatusText.text = message;

        int dotCounter = 0;

        while (true)
        {

            StatusText.text += ".";
            dotCounter++;

            yield return new WaitForSeconds(0.5f);

            if (dotCounter > 3)
            {
                StatusText.text = message;
                dotCounter = 0;
            }
        }
    }
}
