using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System;
public class PointTimer : Timer
{
    Builder builder;

    float productionPoint;

    long finishPoint;
    long timer;
    long timerTemp;

    int speedUpCost;

    NotificationPanel notificationPanel;

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.OnCharacterAssigned += OnCharacterAssigned;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (EventManager.Instance)
        {
            EventManager.Instance.OnCharacterAssigned -= OnCharacterAssigned;
        }
    }

    void Start()
    {
        builder = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == activityInformation.builderReferenceID);
        isFinished = false;
        finishPoint = activityInformation.finishPoint;

        notificationPanel = Resources.FindObjectsOfTypeAll<NotificationPanel>()[0];

        GetProductionPoint();
        InvokeRepeating(nameof(IncreaseCurrentPoint), 0f, 1f);

    }

    void Update()
    {
        if (isFinished)
        {
            return;
        }

        timer = (long)((activityInformation.finishPoint - activityInformation.currentPoint) / productionPoint);
        if (CheckCompleteTimer())
        {
            return;
        }

        if (slider != null)
        {
            InitializeSlider();

            int hours = Mathf.FloorToInt(timer / 3600);
            int minutes = Mathf.FloorToInt(timer % 3600 / 60);
            int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

            if (timerTemp != timer)
            {
                timerTemp = timer;
                slider.GetComponent<Slider>().value = activityInformation.currentPoint;
                slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }

            int speedUpCostTemp = speedUpCost;

            speedUpCost = ItemManager.Instance.GetSpeedUpCost(timer * 20);
            if (speedUpCostTemp != speedUpCost)
            {
                notificationPanel.ChangeSpeedUpCost(activityInformation, speedUpCost);
            
            }
        }
    }

    bool CheckCompleteTimer()
    {
        if (activityInformation.currentPoint >= finishPoint)
        {
            CancelInvoke("IncreaseCurrentPoint");
            isFinished = true;
            activityInformation.isFinished = true;
            builder.TeamLockState.Remove(activityInformation.teamNumber);

            EventManager.Instance.ActivityFinished(activityInformation);
            return true;
        }

        return false;
    }

    void IncreaseCurrentPoint()
    {
        activityInformation.currentPoint += productionPoint;
    }

    void GetProductionPoint()
    {
        float productionPointTemp = (float)(LoadManager.Instance.allBuildingData[builder.Type].production[builder.Level]["Production"]);

        if (builder.CharacterInBuilding[activityInformation.teamNumber] != null)
        {
            productionPointTemp += builder.CharacterInBuilding[activityInformation.teamNumber].Characters.Sum(
                c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));
        }
        productionPoint = productionPointTemp;
    }


    public override void InitializeSlider()
    {
        slider.name = activityInformation.activityName + "Slider";
        slider.GetComponent<Slider>().maxValue = activityInformation.finishPoint;

        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        slider.GetComponent<Slider>().value = activityInformation.currentPoint;
        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

    }

    void OnCharacterAssigned()
    {
        GetProductionPoint();
    }

    public override void ForceFinish()
    {
        activityInformation.currentPoint = finishPoint;
    }


}
