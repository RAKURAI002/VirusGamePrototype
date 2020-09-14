﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NotificationManager : SingletonComponent<NotificationManager>
{
    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

    }
  
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
        EventManager.Instance.OnActivityFinished += OnActivityFinished;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityFinished;
        }
            
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
        }
        secondCalled = true;
    }
    #endregion

    [SerializeField] private ActivityProgressDictionary processingActivies;
    public ActivityProgressDictionary ProcessingActivies { get {return processingActivies; } set { processingActivies = value; } }

    // Start is called before the first frame update
    void Start()
    {
        CheckActivities();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnInitialize()
    {
        processingActivies = new ActivityProgressDictionary();
        
    }
    void CheckActivities()
    {
        foreach (var activity in processingActivies)
        {
            switch (activity.Value.activityType)
            {
                case ActivityType.Quest:
                    {
                        GameObject ActivityTimerGO = new GameObject();
                        ActivityTimerGO.transform.SetParent(transform.Find("ActivitiesList"));
                        ActivityTimerGO.name = activity.Value.activityID.ToString();
                        QuestTimer questTimer = ActivityTimerGO.AddComponent<QuestTimer>();
                        questTimer.activityInformation = activity.Value;
                        break;
                    }
                case ActivityType.Craft:
                    {
                        GameObject ActivityTimerGO = new GameObject();
                        ActivityTimerGO.transform.SetParent(transform.Find("ActivitiesList"));
                        ActivityTimerGO.name = activity.Value.activityID.ToString();
                        CraftTimer craftTimer = ActivityTimerGO.AddComponent<CraftTimer>();
                        craftTimer.InitializeData(activity.Value);
                        break;
                    }
                case ActivityType.Build:
                    {
                        break;
                    }
            }
        }
    }

    public void AddActivity(ActivityInformation activityInformation)
    {
        activityInformation.activityID = GetActivityID();

        processingActivies.Add(activityInformation.activityID, activityInformation);

        EventManager.Instance.ActivityAssigned(activityInformation);
    }
    public void RemoveActivity(ActivityInformation activityInformation)
    {
       // Debug.Log(activityInformation.activityID);
        if(!processingActivies.Remove(activityInformation.activityID))
        {
            Debug.LogError("Remove Activity Failed.");
            //EventManager.Instance.ActivityFinished(activityInformation);
        }
      //  Debug.Log(processingActivies.Count);
        
    }
    public void OnActivityAssigned(ActivityInformation activityInformation)
    {
        switch (activityInformation.activityType)
        {

            case ActivityType.Quest:
                {
                    GameObject ActivityTimerGO = new GameObject(activityInformation.activityID.ToString());
                    ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));
                  
                    QuestTimer questTimer = ActivityTimerGO.AddComponent<QuestTimer>();
                    questTimer.activityInformation = activityInformation;
                    break;
                }
            case ActivityType.Craft:
                {
                    GameObject ActivityTimerGO = new GameObject(activityInformation.activityID.ToString());
                    ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));

                    CraftTimer craftTimer = ActivityTimerGO.AddComponent<CraftTimer>();
                    craftTimer.InitializeData(activityInformation);
                    break;
                }
            case ActivityType.Build:
                {
                    break;
                }
        }

    }
    public void OnActivityFinished(ActivityInformation activityInformation)
    {

        switch (activityInformation.activityType)
        {

            case ActivityType.Quest:
                {
                 
                    break;
                }
            case ActivityType.Craft:
                {
                    CraftTimer craftTimer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activityInformation.activityID).GetComponent<CraftTimer>();
                    Destroy(craftTimer.gameObject);
                    RemoveActivity(activityInformation);
                    break;
                }
            case ActivityType.Build:
                {
                    break;
                }
        }

    }
    public void RefreshFinishedQuestNotification()
    {/*
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            Debug.Log("Back to MainScane to refresh Quests' Notification");
            return;
        }
        GameObject questNotification = GameManager.FindInActiveObjectByName("FinishedQuestAmount");
        if (finishedQuest.Count == 0)
        {
            questNotification.SetActive(false);
        }
        else
        {
            questNotification.SetActive(true);
            questNotification.GetComponentInChildren<Text>().text = finishedQuest.Count.ToString();
        }*/

    }

    int GetActivityID()
    {
        return processingActivies.Count >= 2 ? (processingActivies.Aggregate((a, b) => a.Key > b.Key ? a : b).Key + 1) : processingActivies.Count;
        
    }


}
