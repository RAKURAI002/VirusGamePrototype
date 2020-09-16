using System;
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
    public ActivityProgressDictionary ProcessingActivies { get { return processingActivies; } set { processingActivies = value; } }

    void Start()
    {
        CheckActivities();
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
                        GameObject ActivityTimerGO = new GameObject(activity.Value.activityID.ToString());
                        ActivityTimerGO.transform.SetParent(transform.Find("ActivitiesList"));
                        ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
                        questTimer.activityInformation = activity.Value;
                        break;
                    }
                case ActivityType.Craft:
                    {
                        GameObject ActivityTimerGO = new GameObject(activity.Value.activityID.ToString());
                        ActivityTimerGO.transform.SetParent(transform.Find("ActivitiesList"));
                        CraftTimer craftTimer = ActivityTimerGO.AddComponent<CraftTimer>();
                        craftTimer.InitializeData(activity.Value);
                        break;
                    }
                case ActivityType.Build:
                    {
                        break;
                    }
                case ActivityType.Pregnancy:
                    {
                        GameObject ActivityTimerGO = new GameObject(activity.Value.activityID.ToString());
                        ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));
                        ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
                        questTimer.activityInformation = activity.Value;

                        break;
                    }
                case ActivityType.CharacterGrowing:
                    {
                        GameObject ActivityTimerGO = new GameObject(activity.Value.activityID.ToString());
                        ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));
                        ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
                        questTimer.activityInformation = activity.Value;

                        break;
                    }
                default:
                    {
                        Debug.LogWarning($"{activity.Value.activityType} is currently unhandled.");
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
        if (!processingActivies.Remove(activityInformation.activityID))
        {
            Debug.LogError("Remove Activity Failed.");

        }

    }
    public void OnActivityAssigned(ActivityInformation activityInformation)
    {
        switch (activityInformation.activityType)
        {

            case ActivityType.Quest:
                {
                    GameObject ActivityTimerGO = new GameObject(activityInformation.activityID.ToString());
                    ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));

                    ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
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
                    Debug.Log($"DDDDDDDDDDD");   
                    break;
                }
            case ActivityType.Pregnancy:
                {
                    GameObject ActivityTimerGO = new GameObject(activityInformation.activityID.ToString());
                    ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));

                    ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
                    questTimer.activityInformation = activityInformation;

                    break;
                }
            case ActivityType.CharacterGrowing:
                {
                    GameObject ActivityTimerGO = new GameObject(activityInformation.activityID.ToString());
                    ActivityTimerGO.transform.SetParent(NotificationManager.Instance.gameObject.transform.Find("ActivitiesList"));

                    ClockTimer questTimer = ActivityTimerGO.AddComponent<ClockTimer>();
                    questTimer.activityInformation = activityInformation;

                    break;
                }
            default:
                {
                    Debug.LogWarning($"{activityInformation.activityType} is currently unhandled.");
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
                    /// Update in Notification Panel
                    break;
                }
            case ActivityType.Craft:
                {
                    Resource resource = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == activityInformation.informationID).Value;

                    ItemManager.Instance.AddResource(resource.Name, 1);

                    CraftTimer craftTimer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activityInformation.activityID).GetComponent<CraftTimer>();
                    Destroy(craftTimer.gameObject);
                    RemoveActivity(activityInformation);

                    break;
                }
            case ActivityType.Build:
                {
                    break;
                }
            case ActivityType.Pregnancy:
                {
                    Character character = CharacterManager.Instance.AllCharacters.SingleOrDefault( c => c.ID == activityInformation.informationID);
                    character.workStatus = Character.WorkStatus.Idle;
                    CharacterManager.Instance.CreateChildCharacter();

                    ClockTimer timer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activityInformation.activityID).GetComponent<ClockTimer>();
                    Destroy(timer.gameObject);
                    RemoveActivity(activityInformation);
                    break;
                }
            case ActivityType.CharacterGrowing:
                {
                    ClockTimer timer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activityInformation.activityID).GetComponent<ClockTimer>();
                    Destroy(timer.gameObject);
                    
                    RemoveActivity(activityInformation);

                    Character character = CharacterManager.Instance.AllCharacters.SingleOrDefault(c => c.ID == activityInformation.informationID);
                    character.workStatus = Character.WorkStatus.Idle;

                    break;
                }
            default:
                {
                    Debug.LogWarning($"{activityInformation.activityType} is currently unhandled.");
                    break;
                }
        }

    }

    int GetActivityID()
    {
        return (processingActivies.Count == 0 ? Constant.IDMask.ACTIVITY_ID_MASK : processingActivies.Select(pa => pa.Key).Max()) + 1;

    }


}
