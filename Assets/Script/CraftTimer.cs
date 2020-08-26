using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CraftTimer : MonoBehaviour
{
    public ActivityInformation activityInformation { get; set; }
    public GameObject slider { get; set; }
    public bool isFinished;

    
    Resource resourceRecipe;
    Builder builder;

    long timer { get; set; }
    float productionPoint;
    long finishPoint;

    private void Awake()
    {
        slider = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        isFinished = false;
        Initiate();
        InvokeRepeating("IncreaseCurrentPoint", 0f, 1f);
    }

    // Update is called once per frame
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

        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    void Initiate()
    {
        builder = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.Kitchen);
        resourceRecipe = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == activityInformation.InformationID).Value;

        finishPoint = resourceRecipe.craftingData.point;

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
            productionPointTemp += builder.CharacterInBuilding[activityInformation.teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.crafting * 0.8f / 3)));
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


            Resource resource = LoadManager.Instance.allResourceData[resourceRecipe.Name.Replace("Recipe:", "")];
            ItemManager.Instance.AddResource(resource.Name, 1);

            EventManager.Instance.ActivityFinished(activityInformation);
            return true;
        }

        return false;
    }

    public void ForceFinish()
    {
        activityInformation.currentPoint = finishPoint;
    }
}
