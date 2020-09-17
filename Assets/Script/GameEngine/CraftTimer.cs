﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CraftTimer : Timer
{  
    Resource resourceRecipe;
    Builder builder;

    long timerTemp;
    long timer;
    float productionPoint;
    long finishPoint;

    private void Awake()
    {
        slider = null;
    }

    void Start()
    {
        isFinished = false;
        Initiate();
        InvokeRepeating(nameof(IncreaseCurrentPoint), 0f, 1f);
    }

    void Update()
    {
        if (isFinished)
        {
            return;
        }
        GetProductionPoint();

        if (CheckCompleteTimer())
        {
            return;
        }
        if (slider == null)
        {
            return;
        }
        GetSlider();

        timer = (long)((activityInformation.finishPoint - activityInformation.currentPoint) / productionPoint);
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        if (timerTemp != timer)
        {
            timerTemp = timer;
            slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    public void InitializeData(ActivityInformation information)
    {
        this.builder = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == information.builderReferenceID);
        this.activityInformation = information;
    }
    void Initiate()
    {
        Resource resource = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == activityInformation.informationID).Value;
        resourceRecipe = LoadManager.Instance.allResourceData[$"Recipe:{resource.Name}"];
        finishPoint = resourceRecipe.craftingData.pointRequired;

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
            productionPointTemp += builder.CharacterInBuilding[activityInformation.teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));
        }
        productionPoint = productionPointTemp;
    }
    void GetSlider()
    {
        slider.name = activityInformation.activityName + "Slider";
        slider.GetComponent<Slider>().maxValue = activityInformation.finishPoint;
        slider.GetComponent<Slider>().value = activityInformation.currentPoint;
        slider.GetComponent<Slider>().interactable = false;

        return;
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
    public override void InitializeSlider()
    {
        timer = (long)((activityInformation.finishPoint - activityInformation.currentPoint) / productionPoint);
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public override void ForceFinish()
    {
        activityInformation.currentPoint = finishPoint;
    }
}
