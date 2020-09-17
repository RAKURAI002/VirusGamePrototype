using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BuildTimer : Timer
{
    [SerializeField] Builder builder;
    Building buildData;

    Builder laborCenter;
    GameObject timerCanvas;

    public long timer { get; set; }
    public Vector3 timerOffset;

    long timerTemp;
    [SerializeField] float productionPoint;


    private void Awake()
    {
        laborCenter = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter);
        builder = BuildingManager.Instance.AllBuildings.Single(builder => builder.representGameObject.name == this.gameObject.name);
    }
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

        Initiate();
        InvokeRepeating(nameof(IncreaseCurrentPoint), 0f, 1f);
        CreateSlider();
        GetProductionPoint();
        //  Debug.Log("Upgrading " + thisBuilding.representGameObject.name + " to level" + (thisBuilding.Level) + ". Need " + (pointLeft / TimeSpan.TicksPerSecond) + "s to finish constructing.");
    }

    void Update()
    {
        if (CheckCompleteTimer())
        {
            return;
        }

        timer = (long)((builder.constructionStatus.finishPoint - builder.constructionStatus.currentPoint) / productionPoint);
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        if (timerTemp != timer)
        {
            timerTemp = timer;
            slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }
    void OnCharacterAssigned()
    {
        GetProductionPoint();

    }
    void Initiate()
    {
        builder.constructionStatus.isConstructing = true;

        buildData = LoadManager.Instance.allBuildingData[builder.Type];
        timerCanvas = GameObject.Find("TimerCanvas");
        timerOffset = new Vector3(0, -1, 0);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        GetProductionPoint();
        if (builder.constructionStatus.finishPoint == 0)
        {
            builder.constructionStatus.finishPoint = builder.constructionStatus.constructPointRequired;
        }
        else
        {
            Debug.Log($"Continue from last login : {((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) }s passed resulting in " +
                $"increase currentPoint = {builder.constructionStatus.currentPoint} + { (int)Math.Round(((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) * productionPoint)}");
            builder.constructionStatus.currentPoint += (int)Math.Round(((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) * productionPoint);
        }
        builder.constructionStatus.finishPoint = buildData.upgradePoint[builder.Level];
        // pointLeft = pointFinish - thisBuilding.constructionStatus.currentPoint;
        return;
    }

    bool CheckCompleteTimer()
    {
        if (builder.constructionStatus.finishPoint <= builder.constructionStatus.currentPoint)
        {
            builder.Level++;
            Debug.Log($"Upgrade Task is completed. Now ID :{builder.representGameObject.name} level is {builder.Level}.");
            CancelConstructing();
            EventManager.Instance.ActivityFinished(activityInformation);

            gameObject.GetComponent<BuildingBehavior>().UpdatePrefab();
            return true;
        }
        return false;
    }
    public override void ForceFinish()
    {
        builder.constructionStatus.currentPoint = builder.constructionStatus.finishPoint;
        return;
    }
    void IncreaseCurrentPoint()
    {
        {
            /* long timer = (long)((thisBuilding.constructionStatus.finishPoint - thisBuilding.constructionStatus.currentPoint) / productionPoint);
             int hours = Mathf.FloorToInt(timer / 3600);
             int minutes = Mathf.FloorToInt(timer % 3600 / 60);
             int seconds = Mathf.FloorToInt(timer % 3600 % 60f);
             Debug.Log($"{thisBuilding.constructionStatus.currentPoint} + {productionPoint} :  {thisBuilding.constructionStatus.finishPoint} ." +
                 $"{hours}.{minutes}.{seconds} ");/* Increasing : " +
                 $"{LoadManager.Instance.allBuildingData.Single(b => b.type == Building.BuildingType.LaborCenter).production[laborCenter.Level][-1]}" +
                 $" + AllStrength : {laborCenter.CharacterInBuilding.Sum(c => ((c.Stats.strength * 0.2f/8)))} + AllCraft : {laborCenter.CharacterInBuilding.Sum(c => ((c.Stats.crafting * 0.8f/3)))}" +
                 $" + AllSpeed {laborCenter.CharacterInBuilding.Sum(c => ((c.Stats.speed * 0.2f/8)))} = {productionPoint}");*/
        }

        builder.constructionStatus.currentPoint += productionPoint;
        slider.GetComponent<Slider>().value = builder.constructionStatus.currentPoint;
    }

    public override void InitializeSlider()
    {
        timer = (long)((builder.constructionStatus.finishPoint - builder.constructionStatus.currentPoint) / productionPoint);
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    void CreateSlider()
    {
        GameObject sliderPrefab = Resources.Load("Prefabs/UI/TimeSlider") as GameObject;

        slider = Instantiate(sliderPrefab, gameObject.transform.position + timerOffset, Quaternion.identity);
        slider.transform.SetParent(timerCanvas.transform);
        slider.name = builder.representGameObject.name + "Slider";
        slider.GetComponent<Slider>().maxValue = builder.constructionStatus.finishPoint;
        slider.GetComponent<Slider>().value = builder.constructionStatus.currentPoint;
        slider.GetComponent<Slider>().interactable = false;

        return;
    }

    public void CancelConstructing()
    {
        builder.constructionStatus.isConstructing = false;
        builder.constructionStatus.finishPoint = 0;
        builder.constructionStatus.currentPoint = 0;
        if (laborCenter != null)
        {
            laborCenter.TeamLockState.Remove(builder.constructionStatus.teamNumber);
        }
        builder.constructionStatus.teamNumber = 0;

        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        LoadManager.Instance.SavePlayerDataToJson();
        Destroy(slider);
        Destroy(this);

        return;
    }

    void GetProductionPoint()
    {
        if (laborCenter == null || laborCenter.Level == 0)
        {
            productionPoint = 5;
            return;
        }
        float productionPointTemp = (float)(LoadManager.Instance.allBuildingData[Building.BuildingType.LaborCenter].production[laborCenter.Level]["Production"]);
        if (laborCenter.CharacterInBuilding[builder.constructionStatus.teamNumber] != null)
        {
            productionPointTemp += laborCenter.CharacterInBuilding[builder.constructionStatus.teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));
       
        }
        productionPoint = productionPointTemp;
    }

}
