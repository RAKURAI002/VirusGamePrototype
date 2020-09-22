using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Text;
using System;

public class AchievementPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject container = transform.Find("AchievementInfo/Container").gameObject;
        Debug.Log(LoadManager.Instance.allAchievementData.Count);

        foreach (var achievement in LoadManager.Instance.allAchievementData)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrfabs") as GameObject, container.transform);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = achievement.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(()=> { GetReward(achievement); });

        }

    }



    void GetReward(AchievementData achievements)
    {
        string itemReward = achievements.name;
        foreach (KeyValuePair<string, int> resourceName in achievements.rewards)
        {
            int amount = resourceName.Value;
 
            ItemManager.Instance.AddResource(resourceName.Key, amount);            
        }
    }

}
