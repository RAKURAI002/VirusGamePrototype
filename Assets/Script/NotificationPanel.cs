using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{

    List<GameObject> allActivitiesGO;
    private void Awake()
    {
        allActivitiesGO = new List<GameObject>();
    }
    void OnEnable()
    {
        EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
        EventManager.Instance.OnActivityFinished += OnActivityFinished;

        RefreshCanvas();


    }

    void OnDisable()
    {

        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityFinished;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Resources.Load("Prefabs/TownBasePrefab") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnActivityAssigned(ActivityInformation activityInformation)
    {
        GameObject activitySliderGO = Instantiate(Resources.Load("Prefabs/ActivitySliderPrefab") as GameObject, transform);
        allActivitiesGO.Add(activitySliderGO);
        switch (activityInformation.activityType)
        {
            case ActivityType.Quest:
                {
                    activitySliderGO.AddComponent<QuestTimer>();
                    break;
                }
            case ActivityType.Craft:
                {
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
        Debug.Log("Flag");
        RefreshCanvas();
        switch (activityInformation.activityType)
        {
            case ActivityType.Quest:
                {

                    break;
                }
            case ActivityType.Craft:
                {
                    break;
                }
            case ActivityType.Build:
                {
                    break;
                }
        }


    }

    void RefreshCanvas()
    {
        ClearActivitySlider();
        CreateActivitySlider();
    }
    void ClearActivitySlider()
    {
        foreach(Transform transform in transform.Find("ScrollPanel/Container"))
        {
            Destroy(transform.gameObject);
        }
    }
    void CreateActivitySlider()
    {  

        foreach(var activity in NotificationManager.Instance.ProcessingActivies)
        {
            GameObject activitySliderGO = Instantiate(Resources.Load("Prefabs/UI/ActivitySliderPrefab") as GameObject, transform.Find("ScrollPanel/Container"));
            
            activitySliderGO.transform.Find("Name").GetComponent<Text>().text = activity.Value.activityName;



            allActivitiesGO.Add(activitySliderGO);

            switch (activity.Value.activityType)
            {
               
                case ActivityType.Quest:
                    {
                        QuestTimer questTimer = NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activity.Value.activityID).GetComponent<QuestTimer>();
                        questTimer.slider = activitySliderGO.GetComponentInChildren<Slider>().gameObject;
                        if (activity.Value.isFinished)
                        {

                            Button finishButton = activitySliderGO.transform.Find("FinishButton").GetComponentInChildren<Button>();
                            finishButton.onClick.AddListener(() => { 
                                QuestManager.Instance.FinishQuest(activity.Value);
                                Destroy(questTimer.gameObject);
                                GameObject.FindObjectOfType<MainCanvas>().RefreshNotificationAmount();
                                gameObject.SetActive(false); });
                            finishButton.gameObject.SetActive(true);
                        }
                        break;
                    }
                case ActivityType.Craft:
                    {
                        break;
                    }
                case ActivityType.Build:
                    {
                        break;
                    }
            }
        }
    }

}
