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
    StringBuilder questLog;

    public void Initialize(QuestData _questData, List<Character> _characters)
    {
        characters = _characters;
        questData = _questData;
    }

    public string StartQuest()
    {
        questLog = new StringBuilder();

        questLog.AppendLine($"\n<color=red> QUEST LOG </color>");
        questLog.AppendLine($"Starting {questData.questName} . . .\n");

        StartBattleEvent();

        return questLog.ToString();
    }

    void StartBattleEvent()
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.AddRange(LoadManager.Instance.allEnemyData.Where(e => questData.enemiesIDList.Contains(e.ID)));

        Character leader = characters[0];
        foreach (Enemy enemy in enemies)
        {
            questLog.AppendLine($"Wild {enemy.Name} appears !");
            if (leader.Stats.intelligence >= enemies[0].Stats.intelligence * 4)  // ****************
            {
                questLog.AppendLine($"Thanks to leader({leader.Name}) intelligence({leader.Stats.intelligence}),\n team successfully avoid encountering {enemy.Name}({enemy.Stats.intelligence}).");

            }
            else
            {
                questLog.AppendLine($"Start a battle with {enemy.Name} !");
                int turn = 1;
                while (CalculateBattleDamage(leader, enemy, turn))
                {
                    turn++;

                }
                questLog.AppendLine($"Battle finished. ");

            }

        }

    }

    bool CalculateBattleDamage(Character character, Enemy enemy, int turn)
    {
        int n = 3;

        int attack = character.Stats.strength;
        int defence = character.Stats.strength;
        if (turn % 2 == 0)
        {
            int damage = (int)Math.Floor(n * Math.Exp(0.004f * (enemy.Stats.attack - defence)) * UnityEngine.Random.Range(0.8f, 1.2f));
            character.CurrentHitPoint -= damage;
            questLog.AppendLine($"{enemy.Name} turn : Dealing {damage} damage to {character.Name}. {character.Name} has {character.CurrentHitPoint} Hp left.");

        }
        else
        {
            int damage = (int)Math.Floor(n * Math.Exp(0.004f * (attack - enemy.Stats.defense)) * UnityEngine.Random.Range(0.8f, 1.2f));
            enemy.CurrentHp = enemy.CurrentHp - damage;
            questLog.AppendLine($"Turn {turn}({character.Name}) : Dealing {damage} damage to {enemy.Name}. {enemy.Name} has {enemy.CurrentHp} Hp left.");

        }

        if (character.CurrentHitPoint >= 0 && enemy.CurrentHp >= 0)
            return true;
        else
            return false;

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
