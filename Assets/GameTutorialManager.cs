using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;
using System;
public class GameTutorialManager : MonoBehaviour
{


    enum NameInputState 
    { 
        Unknown,
        Empty,
        ExceedMaximum,
        NotReachMinimum,
        Valid
    }

    public IEnumerator GetPlayerName()
    {
        Debug.Log($"Trying get Player name . . . ");
        GameObject EnterNamePanel = transform.Find("EnterNamePanel").gameObject;
        EnterNamePanel.SetActive(true);
        MainCanvas.FreezeCamera = true;
        NameInputState state = default;
        string name = "";
        InputField inputField = EnterNamePanel.GetComponentInChildren<InputField>();
        EnterNamePanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
            name = inputField.text;
            state = CheckValidName(name);

        });

        while(state != NameInputState.Valid)
        {
            yield return null;
        }
        Debug.Log($"Set Player name to {name}");
        LoadManager.Instance.playerData.name = name;
        EnterNamePanel.SetActive(false);
        MainCanvas.FreezeCamera = false;

    }
    NameInputState CheckValidName(string name)
    {
        NameInputState state;
        if (string.IsNullOrEmpty(name))
        {
            state = NameInputState.Empty;
        }
        else if(name.Length > 10)
        {
            state = NameInputState.ExceedMaximum;
        }
        else if(name.Length < 2)
        {
            state = NameInputState.NotReachMinimum;
        }
        else
        {
            state = NameInputState.Valid;
        }

        SetNameInputStateText(state);
        return state;

    }
    void SetNameInputStateText(NameInputState state)
    {
        Text statusText = transform.Find("EnterNamePanel/StatusText").GetComponent<Text>();
        switch (state)
        {
            case NameInputState.Empty: { statusText.text = "Please enter your name"; break; }
            case NameInputState.ExceedMaximum: { statusText.text = "Characters must not longer than 10."; break; }
            case NameInputState.NotReachMinimum: { statusText.text = "Characters must more than 2."; break; }
            default: { statusText.text = ""; break; }
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
