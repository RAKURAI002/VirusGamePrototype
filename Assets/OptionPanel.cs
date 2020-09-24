using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System.Linq;
using Google;

public class OptionPanel : MonoBehaviour
{
    private void OnEnable()
    {
        
    }
    void Start()
    {
        
    }

    public void OnClickLinkAccountButton()
    {
        transform.Find("LinkAccountPanel").gameObject.SetActive(true);
    }
    public void OnClickTrySignInGoogleAccount()
    {
        Debug.Log($"Trying to sign-in Google Account");
        FireBaseManager.Instance.SignInWithGoogle();
    }
    public void OnClickTrySignInFacebookAccount()
    {
        Debug.Log($"Trying to sign-in Google Account");
    }
    public void OnClickDeleteData()
    {
        Debug.Log($"Signout");
        FirebaseAuth.DefaultInstance.SignOut();
    }
}
