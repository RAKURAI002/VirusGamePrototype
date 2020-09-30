using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Linq;

public class QuestMechanic
{
    QuestData questData;
    List<Character> characters;
    List<Enemy> enemies;
    StringBuilder questLog;
    StringBuilder detailLog;

    List<float> characterOldHP;
    List<int> enemyActionPoint;
    List<int> characterActionPoint;

    bool isAvoidBattle;
    public bool isRetreat;

    public void Initialize(QuestData _questData, List<Character> _characters, bool _isAvoidBattle)
    {
        characters = _characters;
        questData = _questData;
        isAvoidBattle = _isAvoidBattle;
    }

    public void StartQuest(out string _questLog, out string _detailLog)
    {
        questLog = new StringBuilder();
        detailLog = new StringBuilder();

        questLog.AppendLine($"\n<color=red> QUEST LOG </color>");
        questLog.AppendLine($"Starting {questData.questName} . . .\n");

        StartQuestEvent();
        _questLog = questLog.ToString();
        _detailLog = detailLog.ToString();
    }

    void StartQuestEvent()
    {
        enemies = LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)).ToList();

        foreach (Enemy enemy in enemies)
        {
            enemies.Add(ObjectCopier.Clone(enemy));
            questLog.AppendLine($"Wild {enemy.Name} appears !");
        }

        Character leader = characters[0];
        int teamSpeed = characters.Sum(c => c.Stats.speed);
        int enemiesSpeed = enemies.Sum(e => e.Stats.speed);

        /// Try to avoid battle section
        if (leader.Stats.intelligence >= enemies[0].Stats.intelligence * 5 && (teamSpeed > enemiesSpeed))
        {
            questLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering enemies).");
            detailLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering enemies).");
        }
        else if(isAvoidBattle)
        {
            detailLog.AppendLine($"Try to run from {(enemies.Count > 1 ? "enemies" : "enemy")} !");
            if(TryRunForLife())
            {
                detailLog.AppendLine($"Successfully run away from the {(enemies.Count > 1 ? "enemies" : "enemy")}.");
            }
        }
        else
        {
            InitializeActionPoint();

            while (DoBattle())
            {

            }
        }

    }
    bool DoBattle()
    {
        int totalExp;
        float maxActionPoint = 100;
        int turn = 1;

        Character characterLeader = characters[0];
        float sumCharacterHP = characters.Sum(ch => ch.CurrentHitPoint > 0 ? ch.CurrentHitPoint : 0);
        float sumEnemyHP = enemies.Sum(e => e.CurrentHp > 0 ? (float)e.CurrentHp : 0);

        //เก็บคาร์ที่ยังมีชีวิต ไว้ไปใช้ข้างล่าง
        List<Character> aliveCharacter = new List<Character>();
        Character cLeader = characters[0];
        aliveCharacter = characters.Where(ch => ch.CurrentHitPoint > 0).ToList();

        //เก็บ aliveEnemy ไว้ไปใช้ข้างล่าง
        List<Enemy> aliveEnemy = new List<Enemy>();
        aliveEnemy = enemies.Where(e => e.CurrentHp > 0).ToList();
        while (IncreasectionPoint()) ;

        for (int j = 0; j < enemyActionPoint.Count; j++)
        {
            if (enemyActionPoint[j] >= maxActionPoint)
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
            if (characterActionPoint[j] >= maxActionPoint)
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
                        totalExp = questData.expReceived + enemies.Sum(e => e.CurrentHp <= 0 ? e.expReceived : 0);

                        detailLog.AppendLine($"\n{currentCharacter.Name} in turn {turn}. ");
                        detailLog.AppendLine($"Too panicked. Try to retreat.");
                        //หนีได้มั้ย ถ้าหนีได้มัน return true ออกจากลูปไปเลยจ้า
                        return TryRetreat(totalExp);

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
            totalExp = questData.expReceived + enemies.Sum(e => e.CurrentHp < 0 ? e.expReceived : 0);
            //  Debug.Log("go on");

            //โชว์ตอนสุดท้าย
            AppendCharacterSummary(totalExp);
            return true;
        }

    }
    bool TryRetreat(int exp)
    {
        int chancePercent = Constant.QuestMechanicConstant.RETREAT_CHANCE;
        int randValue = UnityEngine.Random.Range(1, 101);

        if (randValue <= chancePercent)
        {

            //  Debug.Log("Retreat");
            detailLog.AppendLine($"Successfully Retreat.");

            AppendCharacterSummary(exp);

            return true;
        }
        else
        {
            //   Debug.Log("nFail Retreat");
            detailLog.AppendLine($"Fail to Retreat.");

            return false;
        }
    }
    void StartTurnFor(Character character, Character charLeader, Enemy enemy, StringBuilder detailLog, bool whosTurn)
    {
        int n = 3;
        int damage = (int)Math.Floor(n * Math.Exp(0.004f * (character.Stats.strength - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));


        Debug.Log("start turn");

        if (!TryDodge(character, enemy, whosTurn))
        {
            if (whosTurn)
            {

                float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);

                character.CurrentHitPoint = character.CurrentHitPoint - totalDamage;
                detailLog.AppendLine($"{character.Name} HP remains {(character.CurrentHitPoint > 0 ? character.CurrentHitPoint : 0)} HP.");
                Debug.Log("damage and hp" + totalDamage.ToString() + " " + character.MaxHitPoint);
                //เช็คว่าตาต่อไปคาร์แรกเตอร์ควรจะหนีมั้ย
                isRetreat = CheckRetreat(totalDamage, character);
            }
            else //ตาคาร์แรคเตอร์ตี
            {
                float totalDamage = damage + WeakpointDamage(damage, charLeader, enemy, whosTurn, detailLog) + CriticalAttack(damage, detailLog, character, enemy, whosTurn);
                enemy.CurrentHp = enemy.CurrentHp - totalDamage;
                detailLog.AppendLine($"{enemy.Name} HP remains {(enemy.CurrentHp > 0 ? enemy.CurrentHp : 0)} HP.");
            }

        }
        else
        {
            detailLog.AppendLine($"Defender successfully dodge from attacker.");
        }


    }
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
    bool CheckRetreat(float damage, Character character)
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
            if (defenderPer > 3.5f * attackerPer)
            {
                chancePercent = Constant.QuestMechanicConstant.DODGE_CHANCE_DA;
            }
            else if ((defenderPer / attackerPer) < 1.3f)
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
    bool IncreasectionPoint()
    {
        bool isSomethingReachHundred = false;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyActionPoint[i] += enemies[i].Stats.speed;

            if (enemyActionPoint[i] >= 100)
            {
                isSomethingReachHundred = true;
            }
                
        }
        for (int i = 0; i < characters.Count; i++)
        {
            characterActionPoint[i] += characters[i].Stats.speed;

            if (characterActionPoint[i] >= 100)
            {
                isSomethingReachHundred = true;
            }
        }

        return isSomethingReachHundred;
    }

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

    void InitializeActionPoint()
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

    void AppendCharacterSummary(int exp)
    {

        questLog.AppendLine($"Battle finished. ");
        detailLog.AppendLine($"Battle finished. ");

        int i = 0;
        foreach (Character character in characters)
        {
            if (character.CurrentHitPoint <= 0)
            {
                CharacterManager.Instance.allDeadcharacter.Add(character, DateTime.Now.AddSeconds(20).Ticks);
                CharacterManager.Instance.AllCharacters.Remove(character);
                CharacterManager.Instance.CancelAssignWork(character);
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


    public Dictionary<string, int> GetQuestReward()
    {
        int itemCarryAmount = characters.Sum(c => c.Stats.strength);

        Dictionary<string, int> itemList = new Dictionary<string, int>();
        foreach (string resourceName in questData.dropResourceName)
        {
            Character character = characters[UnityEngine.Random.Range(0, characters.Count - 1)];
            int amount = character.Stats.perception; // ***************************
            questLog.AppendLine($"{character.Name} found {LoadManager.Instance.allResourceData[resourceName].Name} : {amount} unit(s)");
            ItemManager.Instance.AddResource(resourceName, amount);
            itemList.Add(resourceName, amount);

        }

        return itemList;
    }
}
