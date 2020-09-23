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

public class FireBaseManager : SingletonComponent<FireBaseManager>
{

    Firebase.Auth.FirebaseAuth auth;
    public DatabaseReference reference;
    public string webClientId = "503284986617-pfqma7n52qicbe78jd44psvpem1me8sk.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    protected override void Awake()
    {
        base.Awake();
    
    }

    protected override void OnInitialize()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://virus-game-project.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }
    void Start()
    {
    }

    public async void SignInAsGuest()
    {
        await auth.SignInAnonymouslyAsync().ContinueWith<FirebaseUser>(task => {
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
        FirebaseDatabase.DefaultInstance.RootReference.Child("users/").Child(LoadManager.Instance.playerData.UID).SetRawJsonValueAsync(json);
    }

    public DataSnapshot[] ReceiveData()
    {
        DataSnapshot snapshot = null;
        FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync().ContinueWith(task => { 
            if (task.IsCompleted) { 
                snapshot = task.Result;
                Debug.Log(snapshot.Children.Where(ss => ss.Key == LoadManager.Instance.playerData.UID).ToList()[0].GetRawJsonValue()); 
            } });

        return snapshot.Children.Where(ss => ss.Key == LoadManager.Instance.playerData.UID).ToArray() ;//JsonHelper.ToJson(snapshot.Children.Where(ss => ss.Key == PlayerData.UID).ToArray());
    }
    IEnumerator ReceiveDataCoroutine()
    {
        Task<DataSnapshot> task = FirebaseDatabase.DefaultInstance.GetReference("users/").GetValueAsync();
        yield return null;
        DataSnapshot snapshot = task.Result;
        
    }
}
