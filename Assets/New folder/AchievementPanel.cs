using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Text;
using System;

public class AchievementPanel : MonoBehaviour
{

    void ShowAchievementPanel()
    {
        Transform container = transform.Find("QuestList/Container");
        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);
        }

        foreach (var achievement in LoadManager.Instance.allAchievementData)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrefab") as GameObject, container.transform);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = achievement.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(()=> { GetReward(achievement); });
        }
    }

    void ShowDailyQuestPanel()
    {
        Transform container = transform.Find("QuestList/Container");
        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);
        }

        foreach (var quest in LoadManager.Instance.allDailyQuestData)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrefab") as GameObject, container);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = quest.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => { GetReward(quest); });
        }
    }

    void ShowStoryQuestPanel()
    {
        Transform container = transform.Find("QuestList/Container");
        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);
        }

        foreach (var story in LoadManager.Instance.allStoryQuestData)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrefab") as GameObject, container);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = story.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => { GetReward(story); });
        }
    }

    public void OnClickAchievement()
    {
        ShowAchievementPanel();
    }

    public void OnClickDaily()
    {
        ShowDailyQuestPanel();
    }

    public void OnClickStory()
    {
        ShowStoryQuestPanel();
    }


    void GetReward(AchievementData quest)
    {
        string itemReward = quest.name;
        foreach (KeyValuePair<string, int> resourceName in quest.rewards)
        {
            int amount = resourceName.Value;
 
            ItemManager.Instance.AddResource(resourceName.Key, amount);            
        }
    }

}
