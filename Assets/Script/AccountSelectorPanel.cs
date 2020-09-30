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

    public IEnumerator SetAccountInformation(PlayerData playerData2)
    {
        yield return new WaitForEndOfFrame();

        Debug.Log($"{playerData2.name}");
        Debug.Log($"{1}");
        PlayerData playerData1 = LoadManager.Instance.playerData;

        transform.Find($"Progress{1}").GetComponent<Button>().onClick.AddListener(() => { OnClickChangeAccount(playerData1); });
        transform.Find($"Progress{2}").GetComponent<Button>().onClick.AddListener(() => { OnClickChangeAccount(playerData2); });
        Debug.Log($"{transform.Find($"Progress{1}").GetComponent<Button>().gameObject.name}");

        transform.Find($"Progress{1}/PlayerName").GetComponent<Text>().text = $"{playerData1.name}  Level {playerData1.level}";
        transform.Find($"Progress{2}/PlayerName").GetComponent<Text>().text = $"{playerData2.name}  Level {playerData1.level}";
        Debug.Log($"{transform.Find($"Progress{2}/PlayerInformation").name}");
        Debug.Log($"{2}");
        StringBuilder information = new StringBuilder();
        information.AppendLine($"Building : {playerData1.buildingInPossession.Count}");
        Debug.Log($"{playerData1.buildingInPossession.Count}");
        information.AppendLine($"Character : {playerData1.characterInPossession.Count}"); Debug.Log($"{playerData1.characterInPossession.Count}");
        int diamondAmount = playerData1.resourceInPossession.ContainsKey("Diamond") ? (int)(playerData1.resourceInPossession["Diamond"].Amount) : 0;
        information.AppendLine($"Diamond : {diamondAmount}");
        information.AppendLine($"Last Login : {(new DateTime(playerData1.lastLoginTime)).Date}");
        Debug.Log($"{transform.Find($"Progress{1}/PlayerInformation").name}");
        Debug.Log($"asdff {information.ToString()}");
        transform.Find($"Progress{1}/PlayerInformation").GetComponent<Text>().text = information.ToString();

       
        StringBuilder information2 = new StringBuilder();
        information.AppendLine($"Building : {playerData2.buildingInPossession.Count}");
        information.AppendLine($"Character : {playerData2.characterInPossession.Count}");
        information.AppendLine($"Diamond : {playerData2.resourceInPossession["Diamond"].Amount}");
        information.AppendLine($"Last Login : {(new DateTime(playerData2.lastLoginTime)).Date}");
        transform.Find($"Progress{2}/PlayerInformation").GetComponent<Text>().text = information2.ToString();

       
    }

    public void OnClickChangeAccount(PlayerData playerData)
    {
        Debug.Log($"uid {playerData}");
        gameObject.SetActive(false);
        GameManager.Instance.ChangeAccount(playerData);
    }

}
