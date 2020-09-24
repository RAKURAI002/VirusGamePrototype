using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;


#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

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

    [SerializeField] private ActivityProgressDictionary allProcessingActivies;
    [SerializeField] private DictionaryIntToString androidNotificationSchedule;

    public ActivityProgressDictionary ProcessingActivies { get { return allProcessingActivies; } set { allProcessingActivies = value; } }

    void Start()
    {
        CheckActivities();

    }

    protected override void OnInitialize()
    {
        allProcessingActivies = new ActivityProgressDictionary();
        androidNotificationSchedule = new DictionaryIntToString();

#if UNITY_ANDROID
        AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel()
        {
            Id = $"Channel1",
            Name = "AcitiviyFinished",
            Importance = Importance.High,
            Description = "AcitiviyFinishedChannel"

        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif

    }

    void CheckActivities()
    {
        foreach (var activity in allProcessingActivies)
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
                        PointTimer craftTimer = ActivityTimerGO.AddComponent<PointTimer>();
                        craftTimer.activityInformation = (activity.Value);
                        break;
                    }
                case ActivityType.Build:
                    {
                        Builder builder = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == activity.Value.informationID);
                        Debug.Log($"{builder.representGameObject.name}");
                        Debug.Log($"{builder.representGameObject.GetComponent<BuildTimer>().name}");
                        builder.representGameObject.GetComponent<BuildTimer>().activityInformation = activity.Value;
                        Debug.Log($"{builder.representGameObject.GetComponent<BuildTimer>().activityInformation.activityID}");
                        builder.representGameObject.GetComponent<BuildTimer>().UpdateNewFinishTime();


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
        if (!LoadManager.Instance.playerData.completeTutorial)
        {
            return;
        }

        activityInformation.activityID = GetActivityID();

        allProcessingActivies.Add(activityInformation.activityID, activityInformation);

        StartCoroutine(SetMobileNotificationSchedule(activityInformation));
        EventManager.Instance.ActivityAssigned(activityInformation);
    }

    public void RemoveActivity(ActivityInformation activityInformation)
    {
        if (!allProcessingActivies.Remove(activityInformation.activityID))
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

                    PointTimer craftTimer = ActivityTimerGO.AddComponent<PointTimer>();
                    craftTimer.activityInformation = (activityInformation);
                    break;
                }
            case ActivityType.Build:
                {
                    Builder builder = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == activityInformation.informationID);

                    builder.representGameObject.GetComponent<BuildTimer>().activityInformation = activityInformation;
                    Debug.Log($"{builder.representGameObject.GetComponent<BuildTimer>().activityInformation.activityID}");
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

    IEnumerator SetMobileNotificationSchedule(ActivityInformation activityInformation)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

#if UNITY_ANDROID
        Debug.Log($"{activityInformation.activityName} Notification in {new DateTime(activityInformation.finishTime)} : {activityInformation.finishTime}");
        AndroidNotification androidNotification = new AndroidNotification()
        {        
            Title = activityInformation.activityName,
            Text = $"{activityInformation.activityName} is already finished !",
            FireTime = new DateTime(activityInformation.finishTime),
            // ShouldAutoCancel = true
        };
        activityInformation.androidNotificationID = AndroidNotificationCenter.SendNotification(androidNotification, "Channel1");
#endif

    }

    public void UpdateMobileNotification(ActivityInformation activityInformation)
    {
#if UNITY_ANDROID
        AndroidNotification androidNotification = new AndroidNotification()
        {
            Title = activityInformation.activityName,
            Text = $"{activityInformation.activityName} is already finished !",
            FireTime = new DateTime(activityInformation.finishTime),
            // ShouldAutoCancel = true
        };
        Debug.Log($"Update noti {activityInformation.activityName} to {new DateTime(activityInformation.finishTime)}");
        AndroidNotificationCenter.UpdateScheduledNotification(activityInformation.androidNotificationID, androidNotification, "Channel1");
#endif
    }
    public void CancelMobileNotification(ActivityInformation activityInformation)
    {
        if (activityInformation == null)
            return;
#if UNITY_ANDROID
        Debug.Log($"{activityInformation.activityName} canceled.");
        AndroidNotificationCenter.CancelScheduledNotification(activityInformation.androidNotificationID);
#endif
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

                    PointTimer craftTimer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/"
                        + activityInformation.activityID).GetComponent<PointTimer>();
                    Destroy(craftTimer.gameObject);
                    RemoveActivity(activityInformation);

                    break;
                }
            case ActivityType.Build:
                {
                    RemoveActivity(activityInformation);
                    break;

                }
            case ActivityType.Pregnancy:
                {
                    Character character = CharacterManager.Instance.AllCharacters.SingleOrDefault(c => c.ID == activityInformation.informationID);

                    character.workStatus = Character.WorkStatus.Working;

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
        return (allProcessingActivies.Count == 0 ? Constant.IDMask.ACTIVITY_ID_MASK : allProcessingActivies.Select(pa => pa.Key).Max()) + 1;

    }


}
