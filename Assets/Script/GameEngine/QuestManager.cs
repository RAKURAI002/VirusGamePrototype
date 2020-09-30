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
    int turn; //ไว้โชว์ตอน detaillog
    List<float> characterOldHP; //ไว้โชว์ตอนท้าย
    //สำหรับทำ actiongate
    List<int> enemyActionPoint;
    List<int> characterActionPoint;
    //สำหรับเช็คว่าจะถอยมั้ย
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
        //ดึงข้อมูลตึก townBase ของเรา
        townBase = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);

    }

    #endregion
    public string selectingLevel { get; set; }

    [SerializeField] Builder townBase;

    //เริ่มเควส ใส่หมายเลขทีม
    public void StartQuest(int _teamNumber)
    {
        //ถ้าอิทีมนั้นเป็น 0 ทำไม่ได้จ้าไม่มีทีมเด้อ (ทีมในตึกนี้เท่านั้นที่ไว้ทำเควส)
        if (townBase.CharacterInBuilding[_teamNumber].Characters.Count == 0)
        {
            Debug.LogWarning("You can't start the quest with no character in TownBase.");
            return;

        }

        //ดึงข้อมูลของเควสที่เราทำ ดูจากชื่อ
        QuestData currentQuest = LoadManager.Instance.allQuestData.SingleOrDefault(q => q.Value.questName == selectingLevel).Value;

        //เช็คเผื่อไม่มีข้อมูลเควส เราจะได้รู้ได้ว่ามีผิดพลาด
        if (currentQuest == null)
        {
            Debug.LogError($"Can't find Quest name : {selectingLevel} in QuestData.");
            return;

        }
        Debug.Log(" Starting " + selectingLevel + " . . .");

        //สร้างลิส เก็บคาร์แรคเตอร์ของทีมที่เราส่งไปอ่ะ เอาไว้ใช้ทีหลัง
        List<Character> characters = new List<Character>();
        characters.AddRange(townBase.CharacterInBuilding[_teamNumber].Characters);

        //เปลี่ยนสถานะการทำางนของทุกคนเป็นทำเควส
        foreach (Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Quest;

        }


        int minSpeed = characters.Aggregate((c, min) => (min.Stats.speed < c.Stats.speed ? min : c)).Stats.speed;
        int questDuration = currentQuest.duration; // *****************
        Debug.Log($"The least Character speed stats is {minSpeed} resulting in taking {questDuration} seconds for this quest.");

        NotificationManager.Instance.AddActivity(new ActivityInformation()
        {
            activityName = ("Quest:" + currentQuest.questName),
            activityType = ActivityType.Quest,
            startPoint = DateTime.Now.Ticks,
            requiredPoint = DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond),
            teamNumber = _teamNumber,
            informationID = currentQuest.questID,
            //ใส่เพิ่มมา งงแต่น่าจะเอาไว้ใช้ตอนทำเนี่ยแหละ
            extraData = isAvoidBattle

        });

        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Add(_teamNumber);

        LoadManager.Instance.SavePlayerDataToFireBase();

    }

    //ตอนกดตอนสุดท้ายที่นับเวลาเสร็จ
    public void FinishQuest(ActivityInformation quest)
    {
        //  Debug.Log(quest.activityID);
        //ดึงข้อมูลเควสมา
        QuestData questData = LoadManager.Instance.allQuestData[quest.informationID];
        //สร้างลิส เก็บคาร์ทุกตัวในทีมที่ทำเควสนี้
        List<Character> characters = new List<Character>();
        characters.AddRange(townBase.CharacterInBuilding[quest.teamNumber].Characters);
        //เปลี่ยนสถานะลับเป็นปกติ
        foreach (Character character in characters)
        {
            character.workStatus = Character.WorkStatus.Working;

        }
        //เอาไว้ไปโชว์หน้า result panel
        StringBuilder questLog;
        questLog = new StringBuilder();
        //เอาไว้ไปโชว์หน้า result panel
        StringBuilder detailLog;
        detailLog = new StringBuilder();

        questLog.AppendLine($"\n<color=red> QUEST LOG </color>");
        questLog.AppendLine($"\nStarting {questData.questName} . . .");
        detailLog.AppendLine($"\n<color=red> FIGHT LOG </color>");
        detailLog.AppendLine($"\nStarting {questData.questName} . . .");

        //เกี่ยวกับการต่อสู้
        DoBattle(characters, questLog, detailLog, questData, quest);

        //โชว์ ResultPanel
        ShowResultPanel(GetQuestReward(characters, questLog, questData), questLog, detailLog);

        //เลิก lock team
        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);

        NotificationManager.Instance.RemoveActivity(quest);

        LoadManager.Instance.SavePlayerDataToFireBase();

        return;

    }
    void DoBattle(List<Character> characters, StringBuilder questLog, StringBuilder detailLog, QuestData questData, ActivityInformation quest)
    {
        //สร้างลืสมาเก็บศัตรูของเควสนั้นๆ
        List<Enemy> enemies = new List<Enemy>();

        foreach (Enemy enemy in LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)))
        {
            //ใช้การโคลน จะทำให้ทุกครั้งที่เข้าด่านเดิม ศัตรูจะเป็นเหมือนตัวใหม่เลือดเต้มตลอด
            enemies.Add(ObjectCopier.Clone(enemy));
        }


        //เอาลีดเดอร์มาคิดคำนวณอะไรด้านล่าง
        Character leader = characters[0];

        int teamSpeed = characters.Sum(c => c.Stats.speed);
        int enemiesSpeed = enemies.Sum(e => e.Stats.speed);


        //ลูปนี้ไว้ใช้กับ questLog
        foreach (Enemy enemy in enemies)
        {

            Debug.Log("enemy CurrentHp" + enemy.CurrentHp.ToString());
            questLog.AppendLine($"Wild {enemy.Name} appears !");
        }

        //ถ้าตรงตามเงื่อนไข จะถือว่าจบเควสนั้นเลย ได้ของ บลาๆ เพราะหลบหลีกได้
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
            //สำหรัวหน้า result panel
            foreach (Enemy enemy in enemies)
            {
                questLog.AppendLine($"Start a battle with {enemy.Name} !");
                detailLog.AppendLine($"\nStart a battle with {enemy.Name} !");
            }

            //ค่าเริ่มต้นของ ActionPoint
            InitActionPoint(characters, enemies);

            //ถ้าเลือกหลีกเลี่ยงการสู้
            if ((bool)quest.extraData)
            {
                Debug.Log("AvoidBattle");

                detailLog.AppendLine($"Try to run away from {(enemies.Count > 1 ? "enemies" : "enemy")} !");

                //ถ้าหนีไม่ได้
                if (!TryRunForLife())
                {
                    Debug.Log("Fail to run away");

                    detailLog.AppendLine($"Fail to run away from the {(enemies.Count > 1 ? "enemies" : "enemy")}!");

                    //เริ่มเทิร์น  
                    while (!CalculateBattleTurn(characters, enemies, questLog, detailLog, questData)) ;
                    //พอเสร็จเซทเทิร์นเป็น 0 ก่อน
                    turn = 0;

                }
                else
                {
                    Debug.Log("Avoid success.");

                    detailLog.AppendLine($"Successfully run away from the {(enemies.Count > 1 ? "enemies" : "enemy")}.");

                    //หนีได้ไปโชว์หน้าสรุปเลย
                    ShowCharactersHP(characters, questData.expReceived, questLog, detailLog);


                }

            }
            else //ถ้าไม่เลือกหลีกเลี่ยง มันจะต่อสู้เลย
            {
                //เริ่มเทิร์น  m
                Debug.Log("DidntAvoidBattle");
                while (!CalculateBattleTurn(characters, enemies, questLog, detailLog, questData)) ;
                turn = 0;
            }
        }

    }

    //ทำเกี่ยวกับ actiongate
    bool CalculateBattleTurn(List<Character> characters, List<Enemy> enemies, StringBuilder questLog, StringBuilder detailLog, QuestData questData)
    {
        // + exp ตอนจบ
        int exp;
        float earliestReach = 100;

        //ไว้เช็คว่าจะเลือกสู้ยัง
        float sumCharacterHP = characters.Sum(ch => ch.CurrentHitPoint > 0 ? ch.CurrentHitPoint : 0);
        float sumEnemyHP = enemies.Sum(e => e.CurrentHp > 0 ? (float)e.CurrentHp : 0);

        //เก็บคาร์ที่ยังมีชีวิต ไว้ไปใช้ข้างล่าง
        List<Character> aliveCharacter = new List<Character>();
        Character cLeader = characters[0];
        aliveCharacter = characters.Where(ch => ch.CurrentHitPoint > 0).ToList();

        //เก็บ aliveEnemy ไว้ไปใช้ข้างล่าง
        List<Enemy> aliveEnemy = new List<Enemy>();
        aliveEnemy = enemies.Where(e => e.CurrentHp > 0).ToList();

        //เพิ่ม actiongate ตาม speed ของแต่ละคน
        IncrementAll(characters, enemies);


        //กำหนดให้เริ่มที่ enemy ก่อน

        //เช็คว่าActionPoint ถึงขีดที่ตีได้ยัง
        for (int j = 0; j < enemyActionPoint.Count; j++)
        {
            if (enemyActionPoint[j] >= earliestReach)
            {

                //ตาตัวไหนตี
                Enemy enemy = enemies[j];

                //ถ้ายังเหลือฝั่งตรงข้ามที่ยังมีชีวิต และตัวเองก็ยังไม่ตาย
                if (enemy.CurrentHp > 0 && aliveCharacter != null)
                {
                    // Debug.Log("enemy turn");

                    turn++;
                    detailLog.AppendLine($"\n{enemies[j].Name} attacks the team in turn {turn}.");

                    //ตีเลยยย แรนด้อมตัวโดนตี
                    int random = UnityEngine.Random.Range(0, aliveCharacter.Count - 1);
                    StartTurnFor(aliveCharacter[random], cLeader, enemies[j], detailLog, true);

                    //ลบค่า ActionPoint ที่เอาไปใช้
                    enemyActionPoint[j] -= 100;

                    //เก็บค่า aliveCharacter เผื้อตายแล้ว
                    aliveCharacter = characters.Where(ch => ch.CurrentHitPoint > 0).ToList();

                    // Debug.Log("attack character number" + (random + 1).ToString());
                    //   Debug.Log("enemy actionPoint remain" + actionPoint[j].ToString());
                }

            }
        }

        //
        for (int j = 0; j < characterActionPoint.Count; j++)
        {
            //เช็คเหมือนที่เช็ค enemy ข้างบนเลย
            if (characterActionPoint[j] >= earliestReach)
            {

                Character currentCharacter = characters[j];

                if (currentCharacter.CurrentHitPoint > 0 && sumEnemyHP > 0)
                {

                    // Debug.Log("currentCharacter turn");
                    characterActionPoint[j] -= 100;

                    //เอาไว้เช็คว่า หนีได้มั้ย
                    if (isRetreat)
                    {
                        turn++;
                        //    Debug.Log("Too weak");

                        //รวม exp ที่ต้องได้
                        exp = questData.expReceived + enemies.Sum(e => e.CurrentHp <= 0 ? e.expReceived : 0);

                        detailLog.AppendLine($"\n{currentCharacter.Name} in turn {turn}. ");
                        detailLog.AppendLine($"Too panicked. Try to retreat.");
                        //หนีได้มั้ย ถ้าหนีได้มัน return true ออกจากลูปไปเลยจ้า
                        return TryRetreat(questLog, detailLog, characters, exp);

                    }

                    //ถ้าสู้ปกติ เช็คแบบข้างบนเลย ยังมีตัวที่ต้องสู้อยู่
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

        //ถ้ามีใครเลือดหมดคือจบการทำงาน
        if (sumCharacterHP > 0 && sumEnemyHP > 0)
        {
            return false;
        }
        else
        {
            //รวม exp ที่ต้องได้
            exp = questData.expReceived + enemies.Sum(e => e.CurrentHp < 0 ? e.expReceived : 0);
            //  Debug.Log("go on");

            //โชว์ตอนสุดท้าย
            ShowCharactersHP(characters, exp, questLog, detailLog);
            return true;
        }



    }

    //โชว์ใน log ตอนสุดท้ายเลย
    void ShowCharactersHP(List<Character> characters, int exp, StringBuilder questLog, StringBuilder detailLog)
    {
        int i = 0;
        //Debug.Log("ShowCharactersHP");

        questLog.AppendLine($"Battle finished. ");
        detailLog.AppendLine($"\nBattle finished. ");



        foreach (Character character in characters)
        {
            //ถ้าตายแล้วก็ไม่ได้อะไร แถมยังไปอยู่ในลิส allDeadcharacter รอเลือกว่าจะเอามั้ยไม่เอาอีกที
            if (character.CurrentHitPoint <= 0)
            {
                //ใส่เวลาด้วยว่าให้เวลากี่วันถึงหาย
                CharacterManager.Instance.allDeadcharacter.Add(character, DateTime.Now.AddDays(2).Ticks);
                CharacterManager.Instance.AllCharacters.Remove(character);
                EventManager.Instance.CharacterAssigned();
                LoadManager.Instance.SavePlayerDataToFireBase();

                //พอเลือดหมด แคนเซิลงาน คือหลุดจาก slot ที่เคยใส่ตัวละครเลย กันไม่ให้ส่งตัว 0 ไปซ้ำ
                CharacterManager.Instance.CancelAssignWork(character);
            }
            else //ถ้าไม่ตายก็ได้ exp 
            {
                character.AddEXP(exp);
            }

            //โชว์ว่าใครโดนไรเท่าไหร่ บลาๆ
            questLog.AppendLine($"Sufferd {characterOldHP[i] - character.CurrentHitPoint} damage.  {character.Name} has  {(character.CurrentHitPoint < 0 ? 0 : character.CurrentHitPoint)} Hp left. And get {(character.CurrentHitPoint < 0 ? 0 : exp)} EXP. ");
            detailLog.AppendLine($"Sufferd {characterOldHP[i] - character.CurrentHitPoint} damage.  {character.Name} has {(character.CurrentHitPoint < 0 ? 0 : character.CurrentHitPoint)} Hp left.And get {(character.CurrentHitPoint < 0 ? 0 : exp)} EXP.");
            i++;
        }
    }

    //คือเริ่มเเทิร์นของคนที่ได้ actiongate เต้มก่อน
    void StartTurnFor(Character character, Character charLeader, Enemy enemy, StringBuilder detailLog, bool whosTurn)
    {
        int n = 3;
        int damage = (int)Math.Floor(n * Math.Exp(0.004f * (character.Stats.strength - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));


        Debug.Log("start turn");

        //มีโอกาศที่ฝั่งตั้งรับหลบได้
        //ถ้าไม่ได้
        if (!TryDodge(character, enemy, whosTurn))
        {
            //เช็คว่าเทิร์นใคร true enemy
            if (whosTurn)
            {
                //ดาเมลรวมคือ + WeakpointDamage + CriticalAttack
                float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);
                //ลดเลือดเลยจ้า
                character.CurrentHitPoint = character.CurrentHitPoint - totalDamage;
                detailLog.AppendLine($"{character.Name} HP remains {(character.CurrentHitPoint > 0 ? character.CurrentHitPoint : 0)} HP.");
                Debug.Log("damage and hp" + totalDamage.ToString() + " " + character.MaxHitPoint);
                //เช็คว่าตาต่อไปคาร์แรกเตอร์ควรจะหนีมั้ย
                isRetreat = checkRetreat(totalDamage, character);
            }
            else //ตาคาร์แรคเตอร์ตี
            {
                float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);
                enemy.CurrentHp = enemy.CurrentHp - totalDamage;
                detailLog.AppendLine($"{enemy.Name} HP remains {(enemy.CurrentHp > 0 ? enemy.CurrentHp : 0)} HP.");
            }


        }
        else //เช็คว่าตีได้มั้ย
        {

            detailLog.AppendLine($"Defender successfully dodge fron attacker in turn {turn}");
        }


    }


    //ถ้าดาเมจ 60 เปอของเลือด
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

    //เช็คว่า avoid battle ได้มั้ย
    bool TryRunForLife()
    {
        int chancePercent = Constant.QuestMechanicConstant.RUNAWAY_CHANCE;
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

    //พยายามหนี
    bool TryRetreat(StringBuilder questLog, StringBuilder detailLog, List<Character> characters, int exp)
    {
        int chancePercent = Constant.QuestMechanicConstant.RETREAT_CHANCE;
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
        float chancePercent = 0;
        int randValue = UnityEngine.Random.Range(1, 101);
        int attackerPer = (whosTurn ? enemy.Stats.perception : character.Stats.perception);
        int defenderPer = (whosTurn ? character.Stats.perception : enemy.Stats.perception);

        if (attackerPer > defenderPer)
        {
            if (attackerPer > 2 * defenderPer)
            {
                chancePercent = Constant.QuestMechanicConstant.DODGE_CHANCE_AD;
            }
            else if ((attackerPer / defenderPer) < 1.2)
            {
                chancePercent = Constant.QuestMechanicConstant.BASE_DODGE_CHANCE;
            }
            else
            {
                chancePercent = Constant.QuestMechanicConstant.BASE_DODGE_CHANCE - (((attackerPer / defenderPer) - 1.2f) * 1.25f * (Constant.QuestMechanicConstant.MAX_ACCURACY_AD - Constant.QuestMechanicConstant.MIN_ACCURACY_AD)) + Constant.QuestMechanicConstant.MIN_ACCURACY_AD;
            }
        }
        else if (defenderPer > attackerPer)
        {
            if (defenderPer > 3.5 * attackerPer)
            {
                chancePercent = Constant.QuestMechanicConstant.DODGE_CHANCE_DA;
            }
            else if ((defenderPer / attackerPer) < 1.3)
            {
                chancePercent = Constant.QuestMechanicConstant.BASE_DODGE_CHANCE;
            }
            else
            {
                chancePercent = Constant.QuestMechanicConstant.BASE_DODGE_CHANCE - (((defenderPer / attackerPer) - 1.3f) * 0.4545f * (Constant.QuestMechanicConstant.MAX_ACCURACY_DA - Constant.QuestMechanicConstant.MIN_ACCURACY_DA)) + Constant.QuestMechanicConstant.MIN_ACCURACY_DA;
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

    // weakpoint เช็คตามเงื่อนไขเลยว่า ได้มั้ย
    float WeakpointDamage(int damage, Character leader, Enemy enemy, bool whosTurn, StringBuilder detailLog)
    {

        int attackerInt = (whosTurn ? enemy.Stats.intelligence : leader.Stats.intelligence);
        int defenderInt = (whosTurn ? leader.Stats.intelligence : enemy.Stats.intelligence);

        float weakpointDamage = 0;

        if (attackerInt > 2 * defenderInt)  //ลืมอ่ะ 
        {

            weakpointDamage = damage * (((float)attackerInt / defenderInt * 10) / 100);
            detailLog.AppendLine($"Found weak spot. get more damage {weakpointDamage} damage.");
        }

        return weakpointDamage;

    }

    //ทำคริ สุ่มเปอเอาว่าได้มั้ย
    int CriticalAttack(int damage, StringBuilder detailLog, Character character, Enemy enemy, bool whosTurn)
    {
        int critical = 0;
        int luckChance = (whosTurn ? enemy.Stats.luck : character.Stats.luck);

        int chancePercent = Constant.QuestMechanicConstant.CRITICAL_CHANCE + luckChance;
        int randValue = UnityEngine.Random.Range(1, 101);
        //มี 45 เปอที่อาจเกิด damage เพิ่มได้
        if (randValue <= chancePercent)
        {

            critical = damage;
            detailLog.AppendLine($"get critical hit {critical} damage.");


        }

        return critical;
    }



    //สร้างลิสของ ActionPoint ของทุกคน ใส่ค่าเริ่มต้นไว้เลยครับ 
    void InitActionPoint(List<Character> characters, List<Enemy> enemies)
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

    //เพิ่มค่า ActionPoint ตามสปีดของแต่ละตัว
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


    //ตอนรับของหลังจบด่าน
    Dictionary<string, int> GetQuestReward(List<Character> characters, StringBuilder questLog, QuestData currentQuest)
    {


        //เก็บของที่ได้ จำนวน
        Dictionary<string, int> itemList = new Dictionary<string, int>();

        //เช็คว่ามีโชคดีได้ของไรเพิ่มมั้ย
        LootEvent(characters, itemList, questLog, currentQuest);

        //ถ้ามีตัวรอดชีวิตถึงได้ของ
        if (characters.Sum(ch => (ch.CurrentHitPoint > 0 ? ch.CurrentHitPoint : 0)) > 0)
        {
            //ดรอปของค้าบ
            foreach (string resourceName in currentQuest.dropResourceName)
            {
                //ถ้า perception ของตัวที่สุ่มมามีเยอะ ก็ได้ของตามนั้นครับ
                Character character = characters[UnityEngine.Random.Range(0, characters.Count - 1)];
                int amount = character.Stats.perception; // ***************************

                questLog.AppendLine($"Team found {LoadManager.Instance.allResourceData[resourceName].Name} : {amount} unit(s)");

                //เพิ่มทรัพยากรให้เกมของเรา
                ItemManager.Instance.AddResource(resourceName, amount);

                //เช็คว่าใน itemList มีของนี้ยัง ถ้ามีก็บวกเพิ่ม ถ้าไม่มีก็แอดเพิ่ม กันซ้ำกับ lootevent
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
        else //ถ้าตายก็ไม่ได้อะไร
        {
            questLog.AppendLine($"All characters died. No Item.");
        }

        return itemList;

    }

    //ดู lootevent 
    void LootEvent(List<Character> characters, Dictionary<string, int> itemList, StringBuilder questLog, QuestData currentQuest)
    {


        int charactersLuck = characters.Min(c => c.Stats.luck);
        //   int charactersobserve = characters.Min(c => c.Stats.observing);


        //ดรอปของเพิ่มตามของด่าน
        foreach (var item in currentQuest.dropResourceName)
        {
            Debug.Log("drop item");
            Debug.Log("drop item" + item.ToString());

            //สุ่มเปอร์เซ็นดรอปสำเร็จตาม rarity
            int chanceDrop = LoadManager.Instance.allResourceData.ContainsKey(item) == true ? (int)LoadManager.Instance.allResourceData[item].Rarity : (int)LoadManager.Instance.allEquipmentData[item].Rarity;
            int randomChance = UnityEngine.Random.Range(1, 101);

            //สุ่มจำนวนที่จะดรอป ตาม luck 
            int amount = UnityEngine.Random.Range(1, charactersLuck);

            //ดูว่าสุ่มได้มั้ย
            if (randomChance <= chanceDrop)
            {
                Debug.Log("drop item " + item.ToString() + " " + amount);
                questLog.AppendLine($"luckily found {item} : {amount} unit(s)");
                //เพิ่มใน itemList ไว้โชว์ที่ result panel แล้วก็แอดทรัพยากรเพิ่ม
                itemList.Add(item, amount);


            }
        }

    }


    //ตอนกดรับของ
    void ShowResultPanel(Dictionary<string, int> rewardList, StringBuilder questLog, StringBuilder detailLog)
    {
        //เอาสคริปมา
        ResultPanel resultPanel = GameManager.FindInActiveObjectByName("ResultPanel").GetComponent<ResultPanel>();
        //ใส่ค่าให้ตัวแปรต่างๆ
        resultPanel.gameObject.SetActive(true);
        resultPanel.QuestLog = questLog.ToString();
        resultPanel.DetailQuestLog = detailLog.ToString();
        resultPanel.ShowResultPanel(rewardList);

    }
    public void OnQuestFinished(KeyValuePair<int, ActivityInformation> questData)
    {
    }

}

/*using System.Collections;
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
            requiredPoint = DateTime.Now.Ticks + (questDuration * TimeSpan.TicksPerSecond),
            teamNumber = _teamNumber,
            informationID = currentQuest.questID

        });

        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Add(_teamNumber);

        LoadManager.Instance.SavePlayerDataToFireBase();
  
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

        string questLog;
        string detailLog;
        questMechanic.Initialize(questData, characters, true);
        questMechanic.StartQuest(out questLog, out detailLog);

        ShowResultPanel(questMechanic.GetQuestReward(), questLog);
        
        BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase).TeamLockState.Remove(quest.teamNumber);

        NotificationManager.Instance.RemoveActivity(quest);
        
        LoadManager.Instance.SavePlayerDataToFireBase();

        return;
        
    }

    void ShowResultPanel(Dictionary<string, int> rewardList, string questLog)
    {
        ResultPanel resultPanel =  GameManager.FindInActiveObjectByName("ResultPanel").GetComponent<ResultPanel>() ;
        resultPanel.gameObject.SetActive(true);
        resultPanel.QuestLog = questLog.ToString();
        resultPanel.ShowResultPanel(rewardList);
        
    }
    public void OnQuestFinished(KeyValuePair<int, ActivityInformation> questData)
    {
    }

}*/
