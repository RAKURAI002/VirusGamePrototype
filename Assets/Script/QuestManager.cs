﻿using System.Collections;
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
        EventManager.Instance.OnQuestFinished += OnQuestFinished;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if(EventManager.Instance)
            EventManager.Instance.OnQuestFinished -= OnQuestFinished;
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
            townBase = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);

        }
        secondCalled = true;
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    
    void Start()
    {
        townBase = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);
    }

    void Update()
    {
        
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

        NotificationManager.Instance.AddActivity(new ActivityInformation()
        {
            activityName = ("Quest:" + currentQuest.questName),
            activityType = ActivityType.Quest,
            startTime = DateTime.Now.Ticks,
            finishTime = DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond),
            teamNumber = _teamNumber,
            InformationID = currentQuest.questID
        });

      //  long[] time = new long[2] { DateTime.Now.Ticks, DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond) };
     //   LoadManager.Instance.playerData.questInProgress.Add(currentQuest.questID, new ActivityInformation { teamNumber = _teamNumber, startTime = DateTime.Now.Ticks , finishTime = DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond) });
       // Debug.Log($" QID : {currentQuest.questID}   " + String.Join(" ", LoadManager.Instance.playerData.questInProgress[currentQuest.questID]));
        BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Add(_teamNumber);
       // StartQuestTimer(currentQuest);
        LoadManager.Instance.SavePlayerDataToJson();

        
    }
    /*
    public void OnClickConfirmFinishQuest(KeyValuePair<int, QuestInformation> questData)
    {
       
       
        FinishQuest(questData);
        //RefreshFinishedQuestNotification();
        Destroy(EventSystem.current.currentSelectedGameObject);
    }*/

    public void FinishQuest(ActivityInformation quest)
    {
        Debug.Log(quest.activityID);
        QuestData questData = LoadManager.Instance.allQuestData[quest.InformationID];
        List<Character> characters = new List<Character>();
        characters.AddRange(townBase.CharacterInBuilding[quest.teamNumber].Characters);

        foreach(Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Working;
        }

        StringBuilder questLog;
        questLog = new StringBuilder();

        questLog.AppendLine($"\n<color=red> QUEST LOG </color>");
        questLog.AppendLine($"Starting {questData.questName} . . .\n");
        


        DoBattle(characters, questLog, questData);

        ShowResultPanel(GetQuestReward(characters, questLog, questData), questLog);

       // Debug.Log(teamNumber);
        BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);
        // finishedQuest.Remove(quest.Key);
        NotificationManager.Instance.RemoveActivity(quest);
        
        LoadManager.Instance.SavePlayerDataToJson();

        return;
    }
    void DoBattle(List<Character> characters, StringBuilder questLog, QuestData questData)
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.AddRange(LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)));

        Character leader = characters[0];
        foreach (Enemy enemy in enemies)
        {

            Debug.Log($"Wild {enemy.Name} appears !");
            questLog.AppendLine($"Wild {enemy.Name} appears !");
            if (leader.Stats.intelligence >= enemies[0].Stats.intelligence * 3)  // ****************
            {
                Debug.Log($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}), \nteam successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
                questLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
            }
            else
            {
                questLog.AppendLine($"Start a battle with {enemy.Name} !");
                Debug.Log($"Start a battle with {enemy.Name} !");
                int turn = 1;
                while( CalculateBattleDamage(leader, enemy, turn, questLog))
                {
                   
                    Debug.Log("Turn " + turn);
                    turn++ ;
                }
                questLog.AppendLine($"Battle finished. ");

            }
        }
        




    }


    bool CalculateBattleDamage(Character character, Enemy enemy, int turn, StringBuilder questLog)
    {

        int n = 3;
       
        if(turn % 2 == 0)
        {
            int damage = (int)Math.Floor(n * Math.Exp(0.004f * (enemy.Stats.attack - character.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));

            character.currentHp -= damage;
            questLog.AppendLine($"{enemy.Name} turn : Dealing {damage} damage to {character.Name}. {character.Name} has {character.currentHp} Hp left.");
            Debug.Log($"Turn {turn}({enemy.Name}) : Dealing {damage} damage to {character.Name}. {character.Name} has {character.currentHp} Hp left.");
        }
        else
        {

            int damage = (int)Math.Floor(n * Math.Exp(0.004f * (character.Stats.attack - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));
            enemy.CurrentHp = enemy.CurrentHp - damage;
            questLog.AppendLine($"Turn {turn}({character.Name}) : Dealing {damage} damage to {enemy.Name}. {enemy.Name} has {enemy.CurrentHp} Hp left.");
            Debug.Log($"{character.Name} turn : Dealing {damage} damage to {enemy.Name}. {enemy.Name} has {enemy.CurrentHp} Hp left.");

           

        }
        if (character.currentHp >= 0 && enemy.CurrentHp >= 0)
            return true;
        else
            return false;
    }

    Dictionary<string, int> GetQuestReward(List<Character> characters, StringBuilder questLog, QuestData currentQuest)
    {
        int itemCarryAmount = characters.Sum(c => c.Stats.strength);

        Dictionary<string, int> itemList = new Dictionary<string, int>();
        foreach (string resourceName in currentQuest.dropResourceName)
        {
            Debug.Log(resourceName);
            Character character = characters[UnityEngine.Random.Range(0, characters.Count - 1)];
            int amount = character.Stats.observing; // ***************************
            questLog.AppendLine($"{character.Name} found {LoadManager.Instance.allResourceData[resourceName].Name} : {amount} unit(s)");
            ItemManager.Instance.AddResource(resourceName, amount);
            itemList.Add(resourceName, amount);

        }
        return itemList;
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
       // finishedQuest.Add(questData.Key, questData.Value);
       // RefreshFinishedQuestNotification();
      
    }

}
