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

    public bool isAvoidBattle; //กดเลือกมั้ย
    int turn; //เทิร์นแต่ละครั้ง
    int totalDamage;

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
            informationID = currentQuest.questID,

            AvoidBattle = isAvoidBattle

        });

        BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Add(_teamNumber);

        LoadManager.Instance.SavePlayerDataToJson();
  
    }

    public void FinishQuest(ActivityInformation quest)
    {
      //  Debug.Log(quest.activityID);
        QuestData questData = LoadManager.Instance.allQuestData[quest.informationID];
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

        DoBattle(characters, questLog, questData, quest);

        ShowResultPanel(GetQuestReward(characters, questLog, questData), questLog);

        BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);

        NotificationManager.Instance.RemoveActivity(quest);
        
        LoadManager.Instance.SavePlayerDataToJson();

        return;

    }
    void DoBattle(List<Character> characters, StringBuilder questLog, QuestData questData, ActivityInformation quest)
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.AddRange(LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)));

        Character leader = characters[0];

        int teamSpeed = characters.Sum(c => c.Stats.speed);
        int enemiesSpeed = enemies.Sum(e => e.Stats.speed);

        foreach (Enemy enemy in enemies)
        {
            questLog.AppendLine($"Wild {enemy.Name} appears !");
            if ((leader.Stats.intelligence >= enemies[0].Stats.intelligence * 3) && (teamSpeed > enemiesSpeed))  // ****************
            {


                Debug.Log($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}), \nteam successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
                questLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
            }

            //ถ้าไม่ได้ก็ต้องสู้จ้า
            else
            {
                questLog.AppendLine($"Start a battle with {enemy.Name} !");
                Debug.Log($"Start a battle with {enemy.Name} !");

                //ถ้าเลือก
                if (quest.AvoidBattle)
                {
                    questLog.AppendLine($"Try to run away from {enemy.Name} !");
                    //ถ้าหนีไม่ได้
                    if (!TryRunForLife(characters, enemy))
                    {

                        questLog.AppendLine($"Fail to run away from the {enemy.Name}!");

                        //เริ่มเทิร์น  
                        turn = 1;
                        while (!CalculateBattleDamage(leader, enemy, turn, questLog))
                        {

                            Debug.Log("Turn " + turn);
                            turn++;
                        }
                        questLog.AppendLine($"Battle finished. ");
                    }
                    else
                    {
                        Debug.Log("avoid success");
                        questLog.AppendLine($"Successfully run away from the {enemy.Name}");
                        questLog.AppendLine($"Battle finished. ");
                    }

                }
                else
                {
                    //เริ่มเทิร์น  
                    Debug.Log("No Avoid");
                    turn = 1;
                    while (!CalculateBattleDamage(leader, enemy, turn, questLog))
                    {

                        Debug.Log("Turn " + turn);
                        turn++;
                    }
                    questLog.AppendLine($"Battle finished. ");
                }
            }

        }

    }

    //ใหม่//
    bool TryRunForLife(List<Character> characters, Enemy enemy)
    {
        int chancePercent = Constant.RUNAWAY_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);

        if (randValue < chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TryRetreat(StringBuilder questLog)
    {
        int chancePercent = Constant.RETREAT_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);
        if (randValue < chancePercent)
        {
            questLog.AppendLine($"Successfully Retreat");
            return true;
        }
        else
        {
            questLog.AppendLine($"fail to Retreat");
            return false;
        }
    }

    bool TryDodge(StringBuilder questLog, Character character, Enemy enemy)
    {
        int chancePercent = Constant.DODGE_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);
        if (randValue < chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float WeakpointDamage(Character character, Enemy enemy, int damage, StringBuilder questLog)
    {
        int chancePercent = Constant.DODGE_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);
        float weakpointDamage = 0;
        if (randValue < chancePercent)  //ลืมอ่ะ
        {
            weakpointDamage = (float)damage * ((float)20 / 100);
            questLog.AppendLine($"Found weak spot. get more damage {weakpointDamage} damage.");
        }

        return weakpointDamage;

    }
    int CriticalAttack(Character character, int damage, StringBuilder questLog)
    {
        int critical = 0;
        int chancePercent = Constant.CRITICAL_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);
        //มี 45 เปอที่อาจเกิด damage เพิ่มได้
        if (randValue < chancePercent)
        {

            critical = damage;
            questLog.AppendLine($"get critical hit {critical} damage.");


        }

        return critical;
    }

    //โอกาสในการใช้ไอเทมที่แบกมาด้วย 
    void TryUseItem()
    {

    }
    bool CalculateBattleDamage(Character character, Enemy enemy, int turn, StringBuilder questLog)
    {
        
        /*  int n = 3;
         int damage = 0;
         //ถ้าเทิร์นเลขคู่จะเป็นตาเอเนมี่
          if (turn % 2 == 0)
         {
             damage = (int)Math.Floor(n * Math.Exp(0.004f * (enemy.Stats.attack - character.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));

             //จะพยายามหลบ
             if (!TryDodge(questLog, character, enemy))
             {
                 questLog.AppendLine($"Try to dodge but too bad ,{character.Name} fail");
                 character.currentHp -= damage;
                 totalDamage = totalDamage + damage;

             }
             else
             {
                 questLog.AppendLine($"Successfully dodge in turn {turn}");
             }

         }

         //ตาเรา
         else
         {

             damage = (int)Math.Floor(n * Math.Exp(0.004f * (character.Stats.attack - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));

             //มีโอกาสทำให้ดาเมจแรงขึ้น
             enemy.CurrentHp = enemy.CurrentHp - (damage + CriticalAttack(character, damage, questLog) + WeakpointDamage(character, enemy, damage, questLog));

         }*/

        //ถ้ามีใครเลือดน้อยกว่า 0 หรือตายก้จะจบ
      if (/*character.currentHp */ 2 >= 0 && enemy.CurrentHp >= 0)
        {
            if (/*character.currentHp */ 2 <= 2 && /*character.currentHp*/ 2 < enemy.CurrentHp)
            {
                questLog.AppendLine($"Too weak. Trying to retreat.");
                questLog.AppendLine($"Sufferd {totalDamage} damage.  {character.Name} has ....... Hp left.");
                return TryRetreat(questLog);

            }
            return false;
         }
        else
        {
            Debug.Log("total2" + totalDamage.ToString());
           questLog.AppendLine($"Sufferd {totalDamage} damage.  {character.Name} has ....... Hp left.");
            return true;
        }
    }

    Dictionary<string, int> GetQuestReward(List<Character> characters, StringBuilder questLog, QuestData currentQuest)
    {
        int itemCarryAmount = characters.Sum(c => c.Stats.strength);

        Dictionary<string, int> itemList = new Dictionary<string, int>();

        LootEvent(characters, itemList, questLog);

        foreach (string resourceName in currentQuest.dropResourceName)
        {
            Character character = characters[UnityEngine.Random.Range(0, characters.Count - 1)];
            int amount = character.Stats.perception; // ***************************
            questLog.AppendLine($"{character.Name} found {LoadManager.Instance.allResourceData[resourceName].Name} : {amount} unit(s)");
            ItemManager.Instance.AddResource(resourceName, amount);
            itemList.Add(resourceName, amount);

        }

        return itemList;

    }

    void LootEvent(List<Character> characters, Dictionary<string, int> itemList, StringBuilder questLog)
    {
        int charactersLuck = characters.Min(c => c.Stats.luck);
        //   int charactersobserve = characters.Min(c => c.Stats.observing);

        int randomItem = UnityEngine.Random.Range(1, 101);

        if (randomItem < 50)
        {
            Debug.Log("drop item");
            //สุ่มว่าจะดรอปอะไร
            int itemDrop = UnityEngine.Random.Range(0, LoadManager.Instance.allEquipmentData.Count);
            string itemName = LoadManager.Instance.allEquipmentData.SingleOrDefault(r => r.Value.ID == itemDrop).Key;

            //สุ่มเปอร์เซ็นดรอปสำเร็จ
            int chanceDrop = (int)LoadManager.Instance.allEquipmentData[itemName].Rarity;
            int randomChance = UnityEngine.Random.Range(0, 100);

            //สุ่มจำนวนที่จะดรอป ตาม luck 
            int amount = UnityEngine.Random.Range(1, charactersLuck);

            if (randomChance <= chanceDrop)
            {
                Debug.Log("drop item" + itemName.ToString());
                questLog.AppendLine($"luckily found {itemName} : {amount} unit(s)");
                itemList.Add(itemName, amount);
            }
        }
        else
        {
            Debug.Log("drop resource");
            //สุ่มว่าจะดรอปอะไร
            int itemDrop = UnityEngine.Random.Range(4, 7);
            string itemName = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == itemDrop).Key;

            //สุ่มเปอร์เซ็นดรอปสำเร็จ
            int chanceDrop = (int)LoadManager.Instance.allResourceData[itemName].Rarity;
            float randomChance = UnityEngine.Random.Range(0, 100);

            //สุ่มจำนวนที่จะดรอป ตาม luck 
            int amount = UnityEngine.Random.Range(1, charactersLuck);

            if (randomChance <= chanceDrop)
            {
                questLog.AppendLine($"luckily found {itemName} : {amount} unit(s)");
                itemList.Add(itemName, amount);
            }
        }

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
