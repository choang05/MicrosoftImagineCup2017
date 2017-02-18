using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.QueryHelpers.Other;
using System.Linq;
using UnityEngine.UI;
using AzureServicesForUnity.Helpers;

public class AccountManager : MonoBehaviour
{
    //  Static instance of object to dnot destroy on load
    private static AccountManager control;

    //  User-defined parameters
    public bool DebugAzureServices = false;

    //  static user data
    public static bool IsLoggedIn;
    public static UserData CurrentUser;

    //  References
    private GameManager gameManager;

    //  Events
    public delegate void AccountStatusEvent(string status);
    public static event AccountStatusEvent OnStatusUpdate;
    public delegate void AccountEvent();
    public static event AccountEvent OnSignedIn;
    public static event AccountEvent OnSignedOut;

    private void Awake()
    {
        #region Dont Destroy On Load
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
            Destroy(gameObject);
        #endregion

        gameManager = FindObjectOfType<GameManager>();
    }

    public void Start()
    {
        Globals.DebugFlag = DebugAzureServices;

        if (Globals.DebugFlag)
            Debug.Log("instantiated Azure Services for Unity version " + Constants.LibraryVersion);

        //get the authentication token somehow...
        //e.g. for facebook, check the Unity Facebook SDK at https://developers.facebook.com/docs/unity
        EasyAPIs.Instance.AuthenticationToken = "";
        EasyTables.Instance.AuthenticationToken = "";

        //check here for more information regarding authentication and authorization in Azure App Service
        //https://azure.microsoft.com/en-us/documentation/articles/app-service-authentication-overview/
    }

    public void RegisterUser(string username, string password)
    {
        User newUser = new User();
        newUser.username = username;
        newUser.password = password;
        newUser.checkpointID = 0;

        EasyTableQueryHelper<User> queryHelper = new EasyTableQueryHelper<User>();

        //var query = queryHelper.Where(x => x.score > 500 || x.playername.StartsWith(pn)).OrderBy(x => x.score);
        var query = queryHelper.Where(selectResponse => selectResponse.username.Contains(newUser.username));

        EasyTables.Instance.SelectFiltered<User>(query, selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                //  if username doesn't exists in the database... register user
                if (!IsUsernameInDatabase(ref selectResponse, ref newUser.username))
                {
                    //InsertUserIntoDatabase(newUser);
                    //  Insert newUser into the database
                    EasyTables.Instance.Insert(newUser, insertResponse =>
                    {
                        if (insertResponse.Status == CallBackResult.Success)
                        {
                            //Debug.Log(string.Format("ID is {0}, username is {1}, password is {2}, checkpointID is {3}",
                            //    user.id, user.username, user.password, user.checkpointID));

                            //string result = "Registration completed";
                            //Debug.Log(result);

                            //StatusText.text = "Registration successful!";
                            if (OnStatusUpdate != null) OnStatusUpdate("Registration successful");
                        }
                        else
                            Debug.Log(insertResponse.Exception.Message);
                    });
                }
                else
                    if (OnStatusUpdate != null) OnStatusUpdate("Username not available");
                    //StatusText.text = "Username already taken!";
            }
            else
                Debug.Log(selectResponse.Exception.Message);
        });
    }

    public void LoginUser(string username, string password)
    {
        EasyTableQueryHelper<User> queryHelper = new EasyTableQueryHelper<User>();

        //var query = queryHelper.Where(x => x.score > 500 || x.playername.StartsWith(pn)).OrderBy(x => x.score);
        var query = queryHelper.Where(selectResponse => selectResponse.username.Contains(username));

        EasyTables.Instance.SelectFiltered<User>(query, selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                //  if username/password does exists in the database... log in user
                bool inputAcceptable = false;
                foreach (var item in selectResponse.Result.results)
                {
                    //  Check for the usernames found that only match the input username exactly
                    if (item.username.Equals(username, StringComparison.Ordinal))
                    {
                        if (item.password.Equals(password, StringComparison.Ordinal))
                        {
                            inputAcceptable = true;

                            IsLoggedIn = true;

                            CurrentUser.username = item.username;
                            CurrentUser.password = item.password;
                            CurrentUser.checkpointID = item.checkpointID;
                            CurrentUser.databaseID = item.id;

                            break;
                        }
                    }
                }
                if (inputAcceptable)
                {
                    //StatusText.text = "Login successful!";
                    if (OnStatusUpdate != null) OnStatusUpdate("Login successful");
                    if (OnSignedIn != null) OnSignedIn();
                }
                else
                    if (OnStatusUpdate != null) OnStatusUpdate("Username or password is inccorect!");
                    //StatusText.text = "Username or password is inccorect!";
            }
            else
                Debug.Log(selectResponse.Exception.Message);
        });
    }

    private bool IsUsernameInDatabase(ref CallbackResponse<SelectFilteredResult<User>> selectResponse, ref string username)
    {
        foreach (var item in selectResponse.Result.results)
        {
            //  Check for the usernames found that only match the input username exactly
            if (item.username.Equals(username, StringComparison.Ordinal))
            {
                //Debug.Log(item.username);
                return true;
            }
        }

        return false;
    }

    public void UpdateDatabaseUser()
    {
        if (!IsLoggedIn)
            return;

        //Debug.Log("saving player progress... " + CurrentUser.databaseID);

        EasyTables.Instance.SelectByID<User>(CurrentUser.databaseID, selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                User user = selectResponse.Result;
                user.checkpointID = CurrentUser.checkpointID;
                EasyTables.Instance.UpdateObject(user, updateResponse =>
                {
                    if (updateResponse.Status == CallBackResult.Success)
                    {
                        if (Application.isEditor) Debug.Log("Database id " + updateResponse.Result.id + " was updated");
                    }
                    else
                    {
                        Debug.Log(updateResponse.Exception.Message);
                    }
                });
            }
            else
            {
                Debug.Log(selectResponse.Exception.Message);
            }
        });

        //Debug.Log("Updating database...");
    }

    public void ProcessSignOut()
    {
        CurrentUser.username = null;
        CurrentUser.password = null;
        CurrentUser.databaseID = null;
        CurrentUser.checkpointID = -1;

        if (OnSignedOut != null) OnSignedOut();
    }
}

//Helper class for Easy Tables
/*[Serializable()]
public class Highscore : AzureObjectBase
{
    public int score;
    public string playername;
}*/

//Helper class for Easy Tables
[Serializable()]
public class User : AzureObjectBase
{
    public string username;
    public string password;
    public int checkpointID;
}

public struct UserData
{
    public string username;
    public string password;
    public int checkpointID;
    public string databaseID;
}

//Helper class for Easy APIs
[Serializable()]
public class CustomAPIReturnObject
{
    public string message;
    public int data;
}

