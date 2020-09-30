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
    void AchievementQuestPanel()
    {
        GameObject container = transform.Find("QuestInfo/Container").gameObject;
        Debug.Log(LoadManager.Instance.allAchievementDatas.Count);

        foreach (var achievement in LoadManager.Instance.allAchievementDatas)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrfabs") as GameObject, container.transform);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = achievement.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(()=> { GetReward(achievement); });

        }

    }

    void DailyQuestPanel()
    {
        GameObject container = transform.Find("QuestInfo/Container").gameObject;
        Debug.Log(LoadManager.Instance.allDailyQuestDatas.Count);

        foreach (var daily in LoadManager.Instance.allDailyQuestDatas)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrfabs") as GameObject, container.transform);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = daily.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => { GetReward(daily); });

        }

    }

    void StoryQuestPanel()
    {
        GameObject container = transform.Find("QuestInfo/Container").gameObject;
        Debug.Log(LoadManager.Instance.allStoryQuestDatas.Count);

        foreach (var story in LoadManager.Instance.allStoryQuestDatas)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/AchievementPrfabs") as GameObject, container.transform);

            Text name = itemGO.GetComponentInChildren<Text>();
            name.text = story.name;

            Button button = itemGO.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => { GetReward(story); });

        }

    }

    public void ShowAchievementPanel()
    {
        //Debug.Log("Refreshing Canvas To Resources.");
        AchievementQuestPanel();

    }

    public void ShowDailyPanel()
    {
        //Debug.Log("Refreshing Canvas To Resources.");
        DailyQuestPanel();

    }

    public void ShowStoryPanel()
    {
        //Debug.Log("Refreshing Canvas To Resources.");
        StoryQuestPanel();

    }


    void GetReward(AchievementData quests)
    {
        string itemReward = quests.name;
        foreach (KeyValuePair<string, int> resourceName in quests.rewards)
        {
            int amount = resourceName.Value;
 
            ItemManager.Instance.AddResource(resourceName.Key, amount);            
        }
    }

}
