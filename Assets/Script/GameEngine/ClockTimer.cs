using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClockTimer : Timer
{
    public ActivityInformation activityInformation { get; set; }
    long timeFinish;
    long timeLeft;
    public bool isFinished;
    int speedUpCost;

    long timerTemp;

    
    private void Awake()
    {
        slider = null;
    }

    void Start()
    {
        isFinished = false;
        Initiate();

    }

    void Update()
    {
        if (isFinished)
        {
            return;
        }

        timeLeft = timeFinish - DateTime.Now.Ticks;

        // Debug.Log($"Time Left : {timeLeft} : {timeLeft / TimeSpan.TicksPerSecond }");
        if (CheckCompleteTimer())
        {
            return;
        }

        if (slider == null)
        {
            return;
        }
        
        GetSlider();
        slider.GetComponent<Slider>().value = (((timeFinish - activityInformation.startPoint) - timeLeft) / TimeSpan.TicksPerSecond) + 1;
        long timer = ((timeFinish - activityInformation.startPoint) / TimeSpan.TicksPerSecond) - (((timeFinish - activityInformation.startPoint) - timeLeft) / TimeSpan.TicksPerSecond);
        
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        int speedUpCostTemp = speedUpCost;
        activityInformation.currentPoint = timer;
        speedUpCost = ItemManager.Instance.GetSpeedUpCost(timer * 20);
        if (speedUpCostTemp != speedUpCost)
        {
            Resources.FindObjectsOfTypeAll<NotificationPanel>()[0].ChangeSpeedUpCost(activityInformation, speedUpCost);
        }

        if(timerTemp != timer)
        {
            timerTemp = timer;
            slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    public override void UpdateSlider()
    {
        long timer = ((timeFinish - activityInformation.startPoint) / TimeSpan.TicksPerSecond) - (((timeFinish - activityInformation.startPoint) - timeLeft) / TimeSpan.TicksPerSecond);

        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
        

    bool CheckCompleteTimer()
    {
        if (timeFinish <= DateTime.Now.Ticks)
        {
            FinishTimer();

            return true;
        }
        return false;
    }
    void Initiate()
    {
        timeFinish = activityInformation.finishPoint;
        timeLeft = timeFinish - DateTime.Now.Ticks;
        if (activityInformation.isFinished)
        {
            Destroy(slider);

        }
        return;
    }
    protected override void OnActivityAssigned(ActivityInformation activityInformation)
    {
        base.OnActivityAssigned(activityInformation);
        if (slider != null)
        {
            long timer = ((timeFinish - activityInformation.startPoint) / TimeSpan.TicksPerSecond) - (((timeFinish - activityInformation.startPoint) - timeLeft) / TimeSpan.TicksPerSecond);

            int hours = Mathf.FloorToInt(timer / 3600);
            int minutes = Mathf.FloorToInt(timer % 3600 / 60);
            int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

            slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
       
    }
    public override void ForceFinish()
    {
        FinishTimer();
    }
    void GetSlider()
    {
        slider.name = activityInformation.activityName + "Slider";
        slider.GetComponent<Slider>().maxValue = ((timeFinish - activityInformation.startPoint) / TimeSpan.TicksPerSecond);
        slider.GetComponent<Slider>().value = timeLeft;
        slider.GetComponent<Slider>().interactable = false;

        return;
    }

    public void FinishTimer()
    {
        Destroy(slider);

        isFinished = true;

        activityInformation.isFinished = true;

        EventManager.Instance.ActivityFinished(activityInformation);

        return;
    }
}
