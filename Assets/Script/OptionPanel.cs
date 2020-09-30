using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System.Linq;
using Google;
using UnityEngine.UI;

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
        

        List<IUserInfo> providerList = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.ToList();

        IUserInfo googleID = providerList.SingleOrDefault(info => info.ProviderId == "google.com");
        if(googleID != default(IUserInfo))
        {
            transform.Find("LinkAccountPanel/Container/GoogleButton/InformationText").GetComponent<Text>().text = googleID.DisplayName;
            transform.Find("LinkAccountPanel/Container/GoogleButton/SignOutButton").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("LinkAccountPanel/Container/GoogleButton/InformationText").GetComponent<Text>().text = "";
            transform.Find("LinkAccountPanel/Container/GoogleButton/SignOutButton").gameObject.SetActive(false);

        }

        transform.Find("LinkAccountPanel").gameObject.SetActive(true);
    }
  
    public void OnClickDeleteData()
    {
        Debug.Log($"Signout");
        FireBaseManager.Instance.SignOut();
        GameManager.Instance.ReloadGame();
    }
    public void OnClickSignInWithGoogle()
    {
        FireBaseManager.Instance.SignInWithGoogle();

    }
    public void OnClickSignInWithFacebook()
    {
        var s = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.ToList();
        foreach (var item in s)
        {
            Debug.Log($"{item.Email??""} {item.DisplayName ?? ""} {item.ProviderId ?? ""} {item.PhotoUrl.ToString() ?? ""}");
        }
    }
    public void OnClickSignOutGoogle()
    {
        Debug.Log($"Signing out Google . . . ");
        GoogleSignIn.DefaultInstance.SignOut();
        OnClickLinkAccountButton();
    }
}
