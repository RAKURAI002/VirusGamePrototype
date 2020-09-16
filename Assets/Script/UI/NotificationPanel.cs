﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class NotificationPanel : MonoBehaviour
{
    private void Awake()
    {
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

    public void OnActivityAssigned(ActivityInformation activityInformation)
    {
        RefreshCanvas();

    }
    public void OnActivityFinished(ActivityInformation activityInformation)
    {
        RefreshCanvas();
       
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
            activitySliderGO.name = activity.Value.activityID.ToString();
            Timer timer = (Timer)NotificationManager.Instance.gameObject.transform.Find("ActivitiesList/" + activity.Value.activityID).GetComponent(typeof(Timer));
            timer.Slider = activitySliderGO.GetComponentInChildren<Slider>().gameObject;


            if (!activity.Value.isFinished)
            {
                ChangeSpeedUpCost(activitySliderGO.transform.Find("SpeedUpButton").GetComponentInChildren<Text>(), ItemManager.Instance.GetSpeedUpCost(activity.Value.currentPoint));

                activitySliderGO.transform.Find("SpeedUpButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (ItemManager.Instance.TryConsumeResources("Diamond", ItemManager.Instance.GetSpeedUpCost(activity.Value.currentPoint)))
                    {
                        timer.ForceFinish();

                    }

                });
            }
            timer.Slider = activitySliderGO.GetComponentInChildren<Slider>().gameObject;
            switch (activity.Value.activityType)
            {
                case ActivityType.Quest:
                    {
                        if (activity.Value.isFinished)
                        {
                            activitySliderGO.GetComponentInChildren<Slider>().gameObject.SetActive(false);
                            activitySliderGO.transform.Find("SpeedUpButton").gameObject.SetActive(false);

                            Button finishButton = activitySliderGO.transform.Find("FinishButton").GetComponentInChildren<Button>();
                            finishButton.onClick.AddListener(() => {
                                QuestManager.Instance.FinishQuest(activity.Value);
                                Destroy(timer.gameObject);
                                GameObject.FindObjectOfType<MainCanvas>().RefreshNotificationAmount();
                                gameObject.SetActive(false); 
                            });

                            finishButton.gameObject.SetActive(true);
                        }
                        else
                        {

                        }                     

                        break;
                    }
                case ActivityType.Craft:
                    {
                       
                        break;
                    }
                case ActivityType.Pregnancy:
                    {
                        break;
                    }
                case ActivityType.Build:
                    {
                       
                        break;
                    }
                case ActivityType.CharacterGrowing:
                    {

                        break;
                    }
                default:
                    {
                        Debug.LogWarning($"{activity.Value.activityType} is currently unhandled.");
                        break;
                    }

            }
            timer.Slider = activitySliderGO.GetComponentInChildren<Slider>().gameObject;
        }
        
    }
    public void ChangeSpeedUpCost(ActivityInformation activity, int cost)
    {
        if(gameObject.activeSelf)
        {
            Text text = transform.Find("ScrollPanel/Container/" + activity.activityID + "/SpeedUpButton/Text").GetComponent<Text>();
            text.text = $"<color=blue>{cost}</color> D";
        }
       
    }

    public void ChangeSpeedUpCost(Text buttonText, int cost)
    {
        if (gameObject.activeSelf)
        {
            buttonText.text = $"<color=blue>{cost}</color> D";
        }

    }

}
