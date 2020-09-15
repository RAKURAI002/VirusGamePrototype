using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using UnityEngine.UI;

public class QuestManager : SingletonComponent<QuestManager>
{
    #region Unity Functions
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if(EventManager.Instance)
        {


        }
           
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
            
        }

        if (scene.name == "WorldMap")
        {
            townBase = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);

        }

        secondCalled = true;
    }

    protected override void Awake()
    {
        base.Awake();
        
    }
    
    void Start()
    {
        townBase = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);
    
    }
 
    #endregion
    public string selectingLevel { get; set; }
    
    [SerializeField] Builder townBase;

    public void StartQuest(int _teamNumber)
    {
        if (townBase.CharacterInBuilding[_teamNumber].Characters.Count == 0)
        {
            Debug.LogWarning("You can't start the quest with no character in TownBase.");
            return;

        }

        QuestData currentQuest = LoadManager.Instance.allQuestData.SingleOrDefault(q => q.Value.questName == selectingLevel).Value;

        if (currentQuest == null)
        {
            Debug.LogError($"Can't find Quest name : {selectingLevel} in QuestData.");
            return;

        }
        Debug.Log(" Starting " + selectingLevel + " . . .");

        List<Character> characters = new List<Character>();
        characters.AddRange(townBase.CharacterInBuilding[_teamNumber].Characters);
        foreach(Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Quest;

        }

        int minSpeed = characters.Aggregate((c, min) => (min.Stats.speed < c.Stats.speed ? min : c)).Stats.speed;
        int questDuration = currentQuest.duration ; // *****************
        Debug.Log($"The least Character speed stats is {minSpeed} resulting in taking {questDuration} seconds for this quest.");

        NotificationManager.Instance.AddActivity(new ActivityInformation(){
            activityName = ("Quest:" + currentQuest.questName),
            activityType = ActivityType.Quest,
            startPoint = DateTime.Now.Ticks,
            finishPoint = DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond),
            teamNumber = _teamNumber,
            informationID = currentQuest.questID

        });

        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Add(_teamNumber);

        LoadManager.Instance.SavePlayerDataToJson();
  
    }

    public void FinishQuest(ActivityInformation quest)
    {
        QuestData questData = LoadManager.Instance.allQuestData[quest.informationID];
        List<Character> characters = new List<Character>();
        characters.AddRange(townBase.CharacterInBuilding[quest.teamNumber].Characters);

        foreach(Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Working;

        }

        QuestMechanic questMechanic = new QuestMechanic();

        StringBuilder questLog = new StringBuilder();

        questMechanic.Initialize(questData, characters);
        questLog = questMechanic.StartQuest();
        ShowResultPanel(questMechanic.GetQuestReward(), questLog);
        
        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);

        NotificationManager.Instance.RemoveActivity(quest);
        
        LoadManager.Instance.SavePlayerDataToJson();

        return;
        
    }

    void ShowResultPanel(Dictionary<string, int> rewardList, StringBuilder questLog)
    {
        ResultPanel resultPanel =  GameManager.FindInActiveObjectByName("ResultPanel").GetComponent<ResultPanel>() ;
        resultPanel.gameObject.SetActive(true);
        resultPanel.QuestLog = questLog.ToString();
        resultPanel.ShowResultPanel(rewardList);
        
    }
    public void OnQuestFinished(KeyValuePair<int, ActivityInformation> questData)
    {
    }

}
