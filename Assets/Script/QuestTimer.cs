using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestTimer : MonoBehaviour
{
    public ActivityInformation activityInformation { get; set; }
    long timeFinish;
    long timeLeft;
    public bool isFinished;

    public GameObject slider { get; set; }

    private void Awake()
    {
        slider = null;
    }

    // Start is called before the first frame update
    void Start()
    {

     //   Debug.Log(activityInformation.activityName);

        isFinished = false;
        Initiate();
    }

    // Update is called once per frame
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
        if(slider == null)
        {
            return;
        }
        GetSlider();
        slider.GetComponent<Slider>().value = (((timeFinish - activityInformation.startPoint) - timeLeft) / TimeSpan.TicksPerSecond) + 1;
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
            FinishQuest();
            
            return true;
        }
        return false;
    }
    void Initiate()
    {
      //  Debug.Log("Start Quest : " + questData.duration);
      //  gameObject.GetComponent<Image>().color = Color.red;

        if (activityInformation.finishPoint == 0)
        {
            //playerData.questInProgress[questData.questID] = new ActivityInformation() { startTime = DateTime.Now.Ticks, finishTime = DateTime.Now.Ticks + (questData.duration * TimeSpan.TicksPerSecond)}; // ********************
           // Debug.Log($"{questData.duration} : { playerData.questInProgress[questData.questID].finishTime / TimeSpan.TicksPerSecond}");
        }
        timeFinish = activityInformation.finishPoint;
        timeLeft = timeFinish - DateTime.Now.Ticks;
        if (activityInformation.isFinished)
        {
         //   Debug.Log("SDFsdfewfs3b");

            Destroy(slider);
        }
        return;
    }
    void GetSlider()
    {/*
        string areaButtonName = (LoadManager.Instance.allQuestData[questData.questID].questName.Replace("Normal", "").Replace("Hard", ""));
        //   Debug.Log($"Finding { LoadManager.Instance.allQuestData.Single(q => q.questID == quest.Key).questName} : {GameObject.Find(LoadManager.Instance.allQuestData.Single(q => q.questID == quest.Key).questName).transform.parent.name}");

        int index = areaButtonName.IndexOf("-");
        if (index > 0)
            areaButtonName = areaButtonName.Substring(0, index);

        areaButtonName += "Button";
        Button areaButton = GameManager.FindInActiveObjectByName(areaButtonName).GetComponent<Button>();
        if(areaButton == null)
        {
            Debug.LogError("Can't find Button for Slider Parent.");
            return;
        }
        Debug.Log(areaButtonName);*/
  
       // slider.transform.localScale = Vector3.one * 1.2f;
        slider.name = activityInformation.activityName + "Slider";
        slider.GetComponent<Slider>().maxValue = ((timeFinish - activityInformation.startPoint) / TimeSpan.TicksPerSecond);
        slider.GetComponent<Slider>().value = timeLeft;
        slider.GetComponent<Slider>().interactable = false;

        return;
    }

    public void FinishQuest()
    {
     //   Debug.Log("Stopped QuestTimer");
        // playerData.questInProgress[questData.questID] = new QuestTime() { startTime = 0, finishTime = 0 };

        //***********  BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(teamNumber);
        Destroy(slider);

        
        
        isFinished = true;
        // Debug.Log(isFinished);
        activityInformation.isFinished = true;
        EventManager.Instance.ActivityFinished(activityInformation);

        return;
    }
}
