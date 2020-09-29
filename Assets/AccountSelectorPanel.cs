using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
public class AccountSelectorPanel : MonoBehaviour
{
    void Start()
    {
        
    }

    public void SetAccountInformation(PlayerData playerData2)
    {
        PlayerData playerData1 = LoadManager.Instance.playerData;
        transform.Find($"Progress{1}/PlayerName").GetComponent<Text>().text = $"{playerData1.name}  Level {playerData1.level}";

        StringBuilder information = new StringBuilder();
        information.AppendLine($"Building : {playerData1.buildingInPossession.Count}");
        information.AppendLine($"Character : {playerData1.characterInPossession.Count}");
        information.AppendLine($"Diamond : {playerData1.resourceInPossession["Diamond"].Amount}");
        information.AppendLine($"Last Login : {(new DateTime(playerData1.lastLoginTime)).Date}");
        transform.Find($"Progress{1}/PlayerInformation").GetComponent<Text>().text = information.ToString();

        transform.Find($"Progress{2}/PlayerName").GetComponent<Text>().text = $"{playerData2.name}  Level {playerData1.level}";

        StringBuilder information2 = new StringBuilder();
        information.AppendLine($"Building : {playerData2.buildingInPossession.Count}");
        information.AppendLine($"Character : {playerData2.characterInPossession.Count}");
        information.AppendLine($"Diamond : {playerData2.resourceInPossession["Diamond"].Amount}");
        information.AppendLine($"Last Login : {(new DateTime(playerData2.lastLoginTime)).Date}");
        transform.Find($"Progress{2}/PlayerInformation").GetComponent<Text>().text = information2.ToString();


        transform.Find($"Progress{1}").GetComponent<Button>().onClick.AddListener(() => { OnClickChangeAccount(playerData1); });
        transform.Find($"Progress{2}").GetComponent<Button>().onClick.AddListener(() => { OnClickChangeAccount(playerData2); });

    }

    public void OnClickChangeAccount(PlayerData playerData)
    {
        GameManager.Instance.ChangeAccount(playerData);
    }

}
