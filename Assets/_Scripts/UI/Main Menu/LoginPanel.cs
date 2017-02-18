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
        StatusText.text = "Registering...";

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
        StatusText.text = "Logging in...";

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
                    if (password.Length > 6)
                    {
                        return true;
                    }
                    else
                    {
                        StatusText.text = "Password is too short!";
                    }
                }
                else
                    StatusText.text = "Password cannot contain spaces!";
            }
            else
                StatusText.text = "Username is too short or too long!";
        }
        else
            StatusText.text = "Username cannot contain spaces!";

        ToggleLoginButtons(true);
        toggleInputField(true);

        return false;
    }

    private void UpdateStatus(string status)
    {
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
}
