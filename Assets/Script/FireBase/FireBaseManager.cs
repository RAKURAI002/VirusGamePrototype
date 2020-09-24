using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System.Linq;
using System.Threading.Tasks;
using Google;
using System;
using UnityEngine.Networking;

public class FireBaseManager : SingletonComponent<FireBaseManager>
{
    Firebase.Auth.FirebaseAuth auth;
    public DatabaseReference reference;
    public string webClientId = "503284986617-pfqma7n52qicbe78jd44psvpem1me8sk.apps.googleusercontent.com";

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnInitialize()
    {
        Debug.Log($"Setup FireBase Default Instance.");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://virus-game-project.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

    }
    void Start()
    {
    }

    public Task<FirebaseUser> SignInAsGuest()
    {
        Debug.Log($"Now waiting for sign-in operation . . .");
        return auth.SignInAnonymouslyAsync().ContinueWith<FirebaseUser>(task =>
        {
            Debug.Log($"{task.Status}");
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return null;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return null;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            return task.Result;
        });

    }
    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };

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
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
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

                        Firebase.Auth.FirebaseUser newUser = authTask.Result;

                        newUser.UpdateEmailAsync(((Task<GoogleSignInUser>)task).Result.Email);

                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                            newUser.DisplayName, newUser.UserId);
                        FirebaseDatabase.DefaultInstance.RootReference.Child($"{auth.CurrentUser.DisplayName}/").SetRawJsonValueAsync("");
                    }
                });
            }
        });
    }

    public void SendData(string json)
    {
        Debug.Log($"Sending data to FireBase.");
        FirebaseDatabase.DefaultInstance.RootReference.Child("users/").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(json);
    }

    public async Task<DataSnapshot> ReceivePlayerData()
    {
        Debug.Log($"Accessing {FirebaseAuth.DefaultInstance.CurrentUser.UserId} node.");

        var ds = await FirebaseDatabase.DefaultInstance.RootReference.Child($"users/{FirebaseAuth.DefaultInstance.CurrentUser.UserId}").GetValueAsync();

        //Debug.Log($"{task.Result.GetRawJsonValue()}");
        //{FirebaseAuth.DefaultInstance.CurrentUser.UserId}
        return ds;// task.Result;
    }
    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }
}
