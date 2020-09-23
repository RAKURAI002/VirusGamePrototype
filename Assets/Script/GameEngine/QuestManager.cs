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
    int turn;
    List<float> characterOldHP;
    List<int> enemyActionPoint;
    List<int> characterActionPoint;
    public bool isRetreat;



    #region Unity Functions
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
 
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if (EventManager.Instance)
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
        foreach (Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Quest;

        }

        int minSpeed = characters.Aggregate((c, min) => (min.Stats.speed < c.Stats.speed ? min : c)).Stats.speed;
        int questDuration = currentQuest.duration; // *****************
        Debug.Log($"The least Character speed stats is {minSpeed} resulting in taking {questDuration} seconds for this quest.");

        NotificationManager.Instance.AddActivity(new ActivityInformation() {
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

        foreach (Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Working;

        }

        StringBuilder questLog;
        questLog = new StringBuilder();

        StringBuilder detailLog;
        detailLog = new StringBuilder();

        questLog.AppendLine($"\n<color=red> QUEST LOG </color>");
        questLog.AppendLine($"\nStarting {questData.questName} . . .");
        detailLog.AppendLine($"\n<color=red> FIGHT LOG </color>");
        detailLog.AppendLine($"\nStarting {questData.questName} . . .");

        DoBattle(characters, questLog, detailLog, questData, quest);


        ShowResultPanel(GetQuestReward(characters, questLog, questData), questLog, detailLog);

        BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);

        NotificationManager.Instance.RemoveActivity(quest);

        LoadManager.Instance.SavePlayerDataToJson();

        return;

    }
    void DoBattle(List<Character> characters, StringBuilder questLog, StringBuilder detailLog, QuestData questData, ActivityInformation quest)
    {

        List<Enemy> enemies = new List<Enemy>();

        foreach (Enemy enemy in LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)))
        {
            enemies.Add(ObjectCopier.Clone(enemy));
        }



        Character leader = characters[0];

        int teamSpeed = characters.Sum(c => c.Stats.speed);
        int enemiesSpeed = enemies.Sum(e => e.Stats.speed);



        foreach (Enemy enemy in enemies)
        {

            Debug.Log("enemy CurrentHp" + enemy.CurrentHp.ToString());
            questLog.AppendLine($"Wild {enemy.Name} appears !");
        }

        if ((leader.Stats.intelligence >= enemies[0].Stats.intelligence * 5) && (teamSpeed > enemiesSpeed))  // ****************
        {

            foreach (Enemy enemy in enemies)
            {

               // Debug.Log($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}), \nteam successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
                questLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
                detailLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");
            }
        }

        //ถ้าไม่ได้ก็ต้องสู้จ้า
        else
        {
            foreach (Enemy enemy in enemies)
            {
                questLog.AppendLine($"Start a battle with {enemy.Name} !");
                detailLog.AppendLine($"\nStart a battle with {enemy.Name} !");
            }

            InitActionPoint(characters, enemies);

            //ถ้าเลือก
            if (quest.AvoidBattle)
            {
                Debug.Log("AvoidBattle");

                detailLog.AppendLine($"Try to run away from {(enemies.Count > 1 ? "enemies" : "enemy")} !");
                //ถ้าหนีไม่ได้
                if (!TryRunForLife())
                {
                    Debug.Log("Fail to run away");

                    detailLog.AppendLine($"Fail to run away from the {(enemies.Count > 1 ? "enemies" : "enemy")}!");
                    //เริ่มเทิร์น  

                    while (!CalculateBattleTurn(characters, enemies, questLog, detailLog, questData));
                    turn = 0;

                }
                else
                {
                    Debug.Log("Avoid success.");

                    detailLog.AppendLine($"Successfully run away from the {(enemies.Count > 1 ? "enemies" : "enemy")}.");

                    ShowCharactersHP(characters, questData.EXPreceived, questLog, detailLog);


                }

            }
            else
            {
                //เริ่มเทิร์น  m
                Debug.Log("DidntAvoidBattle");
                while (!CalculateBattleTurn(characters, enemies, questLog, detailLog, questData)) ;
                turn = 0;
            }
        }

    }

    bool CalculateBattleTurn(List<Character> characters, List<Enemy> enemies, StringBuilder questLog, StringBuilder detailLog, QuestData questData)
    {

        int exp;
        float earliestReach = 100;
        float sumCharacterHP = characters.Sum(ch => ch.CurrentHitPoint > 0 ? ch.CurrentHitPoint : 0);
        float sumEnemyHP = enemies.Sum(e => e.CurrentHp > 0 ? (float)e.CurrentHp : 0);

        List<Character> aliveCharacter = new List<Character>();
        Character cLeader = characters[0];
        aliveCharacter = characters.Where(ch => ch.CurrentHitPoint > 0).ToList();


        List<Enemy> aliveEnemy = new List<Enemy>();
        aliveEnemy = enemies.Where(e => e.CurrentHp > 0).ToList();

        IncrementAll(characters, enemies);



        for (int j = 0; j < enemyActionPoint.Count; j++)
        {
            if (enemyActionPoint[j] >= earliestReach)
            {
                
                Enemy enemy = enemies[j];

                if (enemy.CurrentHp > 0 && aliveCharacter != null)
                {
                    // Debug.Log("enemy turn");
                 
                        turn++;
                        detailLog.AppendLine($"\n{enemies[j].Name} attacks the team in turn {turn}.");

                        int random = UnityEngine.Random.Range(0, aliveCharacter.Count - 1);
                        StartTurnFor(aliveCharacter[random], cLeader, enemies[j], detailLog, true);
                    
                        enemyActionPoint[j] -= 100;
                        
                        aliveCharacter = characters.Where(ch => ch.CurrentHitPoint > 0).ToList();
                    
                    // Debug.Log("attack character number" + (random + 1).ToString());
                    //   Debug.Log("enemy actionPoint remain" + actionPoint[j].ToString());
                }

            }
        }

        for (int j = 0; j < characterActionPoint.Count; j++)
        {
            if (characterActionPoint[j] >= earliestReach)
            {

                Character currentCharacter = characters[j];

                if (currentCharacter.CurrentHitPoint > 0 && sumEnemyHP > 0)
                {
               
                   // Debug.Log("currentCharacter turn");
                    characterActionPoint[j] -= 100;

        
                        if (isRetreat)
                        {
                            turn++;
                            //    Debug.Log("Too weak");

                            exp = questData.EXPreceived + enemies.Sum(e => e.CurrentHp <= 0 ? e.EXPreceived : 0);

                            detailLog.AppendLine($"\n{currentCharacter.Name} in turn {turn}. ");
                            detailLog.AppendLine($"Too panicked. Try to retreat.");
                            return TryRetreat(questLog, detailLog, characters,exp);

                        }

                    
                    if (aliveEnemy != null)
                    {
                        turn++;
                        detailLog.AppendLine($"\n{currentCharacter.Name} attacks enemy in turn {turn}.");
                        int random = UnityEngine.Random.Range(0, aliveEnemy.Count - 1);
                        StartTurnFor(currentCharacter, cLeader, aliveEnemy[random], detailLog, false);
                        aliveEnemy = enemies.Where(e => e.CurrentHp > 0).ToList();
                    }
                    //Debug.Log("character actionPoint remains" + actionPoint[j].ToString());  


                }
            }
        }


        if (sumCharacterHP > 0 && sumEnemyHP > 0)
        {
            return false;
        }
        else
        {

            exp = questData.EXPreceived + enemies.Sum(e => e.CurrentHp < 0 ? e.EXPreceived : 0);
            //  Debug.Log("go on");
            ShowCharactersHP(characters, exp, questLog, detailLog);
            return true;
        }



    }

    void ShowCharactersHP(List<Character> characters,int exp, StringBuilder questLog, StringBuilder detailLog)
    {
        int i = 0;
        //Debug.Log("ShowCharactersHP");

        questLog.AppendLine($"Battle finished. ");
        detailLog.AppendLine($"\nBattle finished. ");



        foreach (Character character in characters)
        {
            if (character.CurrentHitPoint <= 0)
            {
                CharacterManager.Instance.allDeadcharacter.Add(character, DateTime.Now.AddSeconds(20).Ticks);
                CharacterManager.Instance.AllCharacters.Remove(character);
                LoadManager.Instance.SavePlayerDataToJson();


                CharacterManager.Instance.CancleAssignWork(character, townBase);
            }
            else
            {
                    character.AddEXP(exp); 
            }
            questLog.AppendLine($"Sufferd {characterOldHP[i] - character.CurrentHitPoint} damage.  {character.Name} has  {(character.CurrentHitPoint < 0 ? 0 : character.CurrentHitPoint)} Hp left. And get {(character.CurrentHitPoint < 0 ? 0 : exp)} EXP. ");
            detailLog.AppendLine($"Sufferd {characterOldHP[i] - character.CurrentHitPoint} damage.  {character.Name} has {(character.CurrentHitPoint < 0 ? 0 : character.CurrentHitPoint)} Hp left.And get {(character.CurrentHitPoint < 0 ? 0 : exp)} EXP.");
            i++;
        }
    }


    void StartTurnFor(Character character, Character charLeader, Enemy enemy, StringBuilder detailLog, bool whosTurn)
    {
        int n = 3;
        int damage = (int)Math.Floor(n * Math.Exp(0.004f * (character.Stats.strength - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));


        Debug.Log("start turn");

        if (!TryDodge(character, enemy, whosTurn))
        {
        //มีโอกาสทำให้ดาเมจแรงขึ้น


        if (whosTurn)
        {
            float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);
            character.CurrentHitPoint = character.CurrentHitPoint - totalDamage;
            detailLog.AppendLine($"{character.Name} HP remains {(character.CurrentHitPoint > 0 ? character.CurrentHitPoint : 0)} HP.");
            Debug.Log("damage and hp" + totalDamage.ToString() + " " + character.MaxHitPoint);
            isRetreat = checkRetreat(totalDamage, character);
        }
        else
        {
            float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);
            enemy.CurrentHp = enemy.CurrentHp - totalDamage;
            detailLog.AppendLine($"{enemy.Name} HP remains {(enemy.CurrentHp > 0 ? enemy.CurrentHp : 0)} HP.");
        }


        }
       else
          {

              detailLog.AppendLine($"Defender successfully dodge fron attacker in turn {turn}");
          }
       

    }

    bool checkRetreat(float damage, Character character)
    {
        Debug.Log("damage and hpp" + damage.ToString() + " " + (((float)60 / 100) * (float)character.MaxHitPoint).ToString());
        if (damage >= ((float)60 / 100) * (float)character.MaxHitPoint)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //ใหม่//
    bool TryRunForLife()
    {
        int chancePercent = Constant.RUNAWAY_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);

        if (randValue <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TryRetreat(StringBuilder questLog, StringBuilder detailLog, List<Character> characters,int exp)
    {
        int chancePercent = Constant.RETREAT_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);
  
        if (randValue <= chancePercent)   
        {

          //  Debug.Log("Retreat");
            detailLog.AppendLine($"Successfully Retreat.");

            ShowCharactersHP(characters, exp, questLog, detailLog);
             
            return true;
        }
        else
        {
         //   Debug.Log("nFail Retreat");
            detailLog.AppendLine($"Fail to Retreat.");

            return false;
        }
    }

    //true คือ enemy attack
    bool TryDodge(Character character, Enemy enemy, bool whosTurn)
    {
        Double chancePercent = 0;
        int randValue = UnityEngine.Random.Range(1, 101);
        int attackerPer = (whosTurn ?  enemy.Stats.perception :  character.Stats.perception);
        int defenderPer = (whosTurn ?  character.Stats.perception :  enemy.Stats.perception);

        if (attackerPer > defenderPer)
        {
            if (attackerPer > 2 * defenderPer)
            {
                chancePercent = Constant.DODGE_CHANCE_AD;
            }
            else if ((attackerPer / defenderPer) < 1.2)
            {
                chancePercent = Constant.BASE_DODGE_CHANCE;
            }
            else
            {
                chancePercent = Constant.BASE_DODGE_CHANCE - (((Double)(attackerPer / defenderPer) - 1.2) * 1.25 * (Double)(Constant.MAX_ACCURACY_AD - Constant.MIN_ACCURACY_AD)) + Constant.MIN_ACCURACY_AD;
            }
        }
        else if (defenderPer > attackerPer)
        {
            if (defenderPer > 3.5 * attackerPer)
            {
                chancePercent = Constant.DODGE_CHANCE_DA;
            }
            else if ((defenderPer / attackerPer) < 1.3)
            {
                chancePercent = Constant.BASE_DODGE_CHANCE;
            }
            else
            {
                chancePercent = Constant.BASE_DODGE_CHANCE - (((Double)(defenderPer / attackerPer) - 1.3) * 0.4545 * (Double)(Constant.MAX_ACCURACY_DA - Constant.MIN_ACCURACY_DA)) + Constant.MIN_ACCURACY_DA;
            }
        }

        if (randValue <= chancePercent)
        {
            return true; 
        }
        else
        {
            return false;
        }
        
    }

    float WeakpointDamage(int damage, Character leader, Enemy enemy, bool whosTurn, StringBuilder detailLog)
    {

        int attackerInt= (whosTurn ? enemy.Stats.intelligence : leader.Stats.intelligence);
        int defenderInt = (whosTurn ? leader.Stats.intelligence : enemy.Stats.intelligence);

        float weakpointDamage = 0;

        if (  attackerInt > 2 * defenderInt)  //ลืมอ่ะ 
        {

            weakpointDamage = damage * (((float)attackerInt/defenderInt * 10)/100);
            detailLog.AppendLine($"Found weak spot. get more damage {weakpointDamage} damage.");
        }

        return weakpointDamage;

    }
    int CriticalAttack(int damage, StringBuilder detailLog, Character character, Enemy enemy, bool whosTurn )
    {
        int critical = 0;
        int  luckChance = (whosTurn ? enemy.Stats.luck : character.Stats.luck);
   
        int chancePercent = Constant.CRITICAL_CHANCE + luckChance;
        int randValue = UnityEngine.Random.Range(1, 101);
        //มี 45 เปอที่อาจเกิด damage เพิ่มได้
        if (randValue <= chancePercent)
        {

            critical = damage;
            detailLog.AppendLine($"get critical hit {critical} damage.");


        }

        return critical;
    }


   
  

    void InitActionPoint(List<Character> characters , List<Enemy> enemies)
    {

        enemyActionPoint = new List<int>();
        characterActionPoint = new List<int>();
        characterOldHP = new List<float>();

        foreach (Enemy enemy in enemies)
        {
            enemyActionPoint.Add(enemy.Stats.speed);
        }
        foreach (Character character in characters)
        {
       
            characterActionPoint.Add(character.Stats.speed);
            characterOldHP.Add(character.CurrentHitPoint);
        }

    }

     void IncrementAll(List<Character> characters, List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyActionPoint[i] += enemies[i].Stats.speed;
            Debug.Log("actionPoint enemy " + i + " " + enemyActionPoint[i].ToString());
        }
     
        for (int i = 0; i < characters.Count; i++)
        {
            characterActionPoint[i] += characters[i].Stats.speed;
            Debug.Log("actionPoint character " + i + " " + characterActionPoint[i].ToString());
        }
    }

    //true คิอ enemy attack
   

    Dictionary<string, int> GetQuestReward(List<Character> characters, StringBuilder questLog, QuestData currentQuest)
    {

     

        Dictionary<string, int> itemList = new Dictionary<string, int>();

        LootEvent(characters, itemList, questLog, currentQuest);

        if (characters.Sum(ch => (ch.CurrentHitPoint > 0 ? ch.CurrentHitPoint : 0)) > 0)
        {
            foreach (string resourceName in currentQuest.dropResourceName)
            {
                Character character = characters[UnityEngine.Random.Range(0, characters.Count - 1)];
                int amount = character.Stats.perception; // ***************************

                questLog.AppendLine($"Team found {LoadManager.Instance.allResourceData[resourceName].Name} : {amount} unit(s)");

                ItemManager.Instance.AddResource(resourceName, amount);
                if (itemList.Keys.Contains(resourceName) == true)
                {
                    itemList[resourceName] = itemList[resourceName] + amount;
                }
                else
                {
                    itemList.Add(resourceName, amount);
                }
            }

        }
        else 
        {
            questLog.AppendLine($"All characters died. No Item.");
        }
        return itemList;

    }

    void LootEvent(List<Character> characters, Dictionary<string, int> itemList, StringBuilder questLog, QuestData currentQuest)
    {
        int charactersLuck = characters.Min(c => c.Stats.luck);
        //   int charactersobserve = characters.Min(c => c.Stats.observing);

        int randomItem = UnityEngine.Random.Range(1, 101);

      
       
           foreach ( var item in currentQuest.dropResourceName)
            {
                 Debug.Log("drop item");
                Debug.Log("drop item" + item.ToString());
                //สุ่มเปอร์เซ็นดรอปสำเร็จ
                int chanceDrop = LoadManager.Instance.allResourceData.ContainsKey(item) == true ? (int)LoadManager.Instance.allResourceData[item].Rarity : (int)LoadManager.Instance.allEquipmentData[item].Rarity;
                int randomChance = UnityEngine.Random.Range(1, 101);

                //สุ่มจำนวนที่จะดรอป ตาม luck 
                int amount = UnityEngine.Random.Range(1, charactersLuck);

                if (randomChance <= chanceDrop)
                {
                    Debug.Log("drop item " + item.ToString() + " " + amount );
                    questLog.AppendLine($"luckily found {item} : {amount} unit(s)");
                     itemList.Add(item, amount);
                    

                }
            }
          
                
           
          
        
        

    }

    void ShowResultPanel(Dictionary<string, int> rewardList, StringBuilder questLog, StringBuilder detailLog)
    {
        ResultPanel resultPanel =  GameManager.FindInActiveObjectByName("ResultPanel").GetComponent<ResultPanel>() ;
        resultPanel.gameObject.SetActive(true);
        resultPanel.QuestLog = questLog.ToString();
        resultPanel.DetailQuestLog = detailLog.ToString();
        resultPanel.ShowResultPanel(rewardList);
        
    }
    public void OnQuestFinished(KeyValuePair<int, ActivityInformation> questData)
    {
    }

}
