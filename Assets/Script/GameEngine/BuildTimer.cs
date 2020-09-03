using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BuildTimer : MonoBehaviour
{
    [SerializeField] Builder thisBuilding;
    Building buildData;

    Builder laborCenter;
    GameObject timerCanvas;
    GameObject slider;

    public long timer { get; set; }
    public Vector3 timerOffset;

    [SerializeField] float productionPoint;


    private void Awake()
    {
        laborCenter = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter);

    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    void Start()
    {

        Initiate();
        InvokeRepeating("IncreaseCurrentPoint", 0f, 1f);
        CreateSlider();

        //  Debug.Log("Upgrading " + thisBuilding.representGameObject.name + " to level" + (thisBuilding.Level) + ". Need " + (pointLeft / TimeSpan.TicksPerSecond) + "s to finish constructing.");
    }

    void Update()
    {

        GetProductionPoint();


        if (CheckCompleteTimer())
        {
            return;
        }


        timer = (long)((thisBuilding.constructionStatus.finishPoint - thisBuilding.constructionStatus.currentPoint) / productionPoint);
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt(timer % 3600 / 60);
        int seconds = Mathf.FloorToInt(timer % 3600 % 60f);

        slider.GetComponentInChildren<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    void Initiate()
    {


        thisBuilding = BuildManager.Instance.AllBuildings.Single(builder => builder.representGameObject.name == this.gameObject.name);
        Debug.Log(thisBuilding.Type.ToString());
        thisBuilding.constructionStatus.isConstructing = true;


        buildData = LoadManager.Instance.allBuildingData[thisBuilding.Type];
        timerCanvas = GameObject.Find("TimerCanvas");
        timerOffset = new Vector3(0, -1, 0);
        Debug.Log(thisBuilding.constructionStatus.teamNumber);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        GetProductionPoint();
        if (thisBuilding.constructionStatus.finishPoint == 0)
        {
            thisBuilding.constructionStatus.currentPoint = 0;
            thisBuilding.constructionStatus.finishPoint = thisBuilding.constructionStatus.constructPointRequired;
        }
        else
        {
            Debug.Log($"Continue from last login : {((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) }s passed resulting in " +
                $"increase currentPoint = {thisBuilding.constructionStatus.currentPoint} + { (int)Math.Round(((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) * productionPoint)}");
            thisBuilding.constructionStatus.currentPoint += (int)Math.Round(((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond) * productionPoint);
        }
        thisBuilding.constructionStatus.finishPoint = buildData.upgradePoint[thisBuilding.Level];
        // pointLeft = pointFinish - thisBuilding.constructionStatus.currentPoint;
        return;
    }

    bool CheckCompleteTimer()
    {
        if (thisBuilding.constructionStatus.finishPoint <= thisBuilding.constructionStatus.currentPoint)
        {
            thisBuilding.Level++;
            Debug.Log($"Upgrade Task is completed. Now ID :{thisBuilding.representGameObject.name} level is {thisBuilding.Level}.");
            CancelConstructing();
            EventManager.Instance.ActivityFinished(new ActivityInformation() { activityType = ActivityType.Build});

            gameObject.GetComponent<BuildingBehavior>().UpdatePrefab();
            return true;
        }
        return false;
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

        thisBuilding.constructionStatus.currentPoint += productionPoint;
        slider.GetComponent<Slider>().value = thisBuilding.constructionStatus.currentPoint;
    }


    void CreateSlider()
    {
        GameObject sliderPrefab = Resources.Load("Prefabs/UI/TimeSlider") as GameObject;

        slider = Instantiate(sliderPrefab, gameObject.transform.position + timerOffset, Quaternion.identity);
        slider.transform.SetParent(timerCanvas.transform);
        slider.name = thisBuilding.representGameObject.name + "Slider";
        slider.GetComponent<Slider>().maxValue = thisBuilding.constructionStatus.finishPoint;
        slider.GetComponent<Slider>().value = thisBuilding.constructionStatus.currentPoint;
        slider.GetComponent<Slider>().interactable = false;

        return;
    }

    public void CancelConstructing()
    {
        thisBuilding.constructionStatus.isConstructing = false;
        thisBuilding.constructionStatus.finishPoint = 0;
        thisBuilding.constructionStatus.currentPoint = 0;
        if (laborCenter != null)
        {
            laborCenter.TeamLockState.Remove(thisBuilding.constructionStatus.teamNumber);
        }
        thisBuilding.constructionStatus.teamNumber = 0;

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
        //Debug.Log(thisBuilding.constructionStatus.teamNumber);
        if (laborCenter.CharacterInBuilding[thisBuilding.constructionStatus.teamNumber] != null)
        {

            productionPointTemp += laborCenter.CharacterInBuilding[thisBuilding.constructionStatus.teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));
        //    Debug.Log(laborCenter.CharacterInBuilding[thisBuilding.constructionStatus.teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.crafting * 0.8f / 3))));
        }
        productionPoint = productionPointTemp;
    }
}
