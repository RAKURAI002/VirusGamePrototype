using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    [SerializeField] public int questID;
    [SerializeField] public string questName;
    [SerializeField] public Character.AllStats requireStats;
    [SerializeField] public List<string> dropResourceName;
    [SerializeField] public List<int> enemiesIDList;
    [SerializeField] public int duration;
}
