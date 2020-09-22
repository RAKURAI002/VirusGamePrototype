/*using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FireBaseAuth : MonoBehaviour
{
    private FirebaseAuth auth;
    public InputField emailInput, passwordInput, cPasswordInput;
    public Button signUpButton, loginButton;
    public Text resultText;

    bool isLoginSucceded;

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        loginButton.onClick.AddListener(()=> LoginToFireBase(emailInput.text, passwordInput.text));
        signUpButton.onClick.AddListener(() => SignupToFireBase(emailInput.text, passwordInput.text));
    }
    private void Update()
    {
        if(isLoginSucceded)
        {
            Debug.Log("Loading new secene . . .");
            StartCoroutine(LoadNewScene());
            isLoginSucceded = false;
        }
    }
    private void SignupToFireBase(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            UpdateStatus("Please enter all information.");
            return;
        }
        if(passwordInput.text != cPasswordInput.text)
        {
            UpdateStatus("Password doesn't match.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync error: " + task.Exception);
                if (task.Exception.InnerExceptions.Count > 0)
                    UpdateStatus(task.Exception.InnerExceptions[0].Message);
                return;
            }

            FirebaseUser newUser = task.Result; // Firebase user has been created.
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            UpdateStatus("Signup Success");
        });
    }

    private void UpdateStatus(string status)
    {
        resultText.text = status; 
    }
    public void LoginToFireBase(string email, string password)
    {
        Debug.Log("Trying Login");
        isLoginSucceded = false;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            Debug.Log(task.Status);
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync error: " + task.Exception);
                if (task.Exception.InnerExceptions.Count > 0)
                    UpdateStatus(task.Exception.InnerExceptions[0].Message);
                return;
            }
            isLoginSucceded = true;
            FirebaseUser user = task.Result;

            LoadManager.Instance.playerData.UID = user.UserId;
            Debug.Log("Login Complete : " + LoadManager.Instance.playerData.UID);
            
        });
        if(isLoginSucceded)
        {

        }





    }
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync("MainScene");
        while (!async.isDone)
        {
            yield return null;
        }
    }
    IEnumerator waitTwoseconds()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("MainScene");
        yield return async;
    }
    void LoadScene()
    {
        SceneManager.LoadSceneAsync("MainScene");
    
    }
}
*/