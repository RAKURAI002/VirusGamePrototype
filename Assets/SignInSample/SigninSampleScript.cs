
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public class SigninSampleScript : MonoBehaviour
{
    public Text statusText;

    public string webClientId = "503284986617-pfqma7n52qicbe78jd44psvpem1me8sk.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    public void OnSignIn()
    {
        Debug.Log($"Try Sign in");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn");

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            Debug.Log($"Start ");
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                signInCompleted.SetException(task.Exception);
            }
            else
            {
                Debug.Log($"Token : {((Task<GoogleSignInUser>)task).Result.IdToken}");
                Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask => {
                Debug.Log($"{authTask.Status}");
                if (authTask.IsCanceled)
                {
                    Debug.Log($"canceled");
                    signInCompleted.SetCanceled();
                }
                else if (authTask.IsFaulted)
                {
                    Debug.Log($"{authTask.Exception}");
                    signInCompleted.SetException(authTask.Exception);
                }
                else
                {

                    signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                    Debug.Log($"Mail : {auth.CurrentUser.Email}");
                    Debug.Log($"{((Task<GoogleSignInUser>)task).Result.Email}");

                    Debug.Log($"Mail : {auth.CurrentUser.Email}");
                    AddStatusText("Welcome: " + task.Result.DisplayName + "!");
                    Firebase.Auth.FirebaseUser newUser = authTask.Result;

                        newUser.UpdateEmailAsync(((Task<GoogleSignInUser>)task).Result.Email).ContinueWith((d) => { Debug.Log($"{newUser.Email}"); }).Start();

                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                            newUser.DisplayName, newUser.UserId);
                        FirebaseDatabase.DefaultInstance.RootReference.Child($"{auth.CurrentUser.DisplayName}/").SetRawJsonValueAsync("");
                    }
                });
            }
        });


    }

    public void OnSignOut()
    {
        AddStatusText("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddStatusText("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    AddStatusText("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddStatusText("Canceled");
        }
        else
        {
            AddStatusText("Welcome: " + task.Result.DisplayName + "!");
        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }


    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddStatusText("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    private List<string> messages = new List<string>();
    void AddStatusText(string text)
    {
        if (messages.Count == 5)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        statusText.text = txt;
    }
    public void test()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("users/").SetRawJsonValueAsync("");

    }
}
