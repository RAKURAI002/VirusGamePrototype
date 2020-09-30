using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class AchievementData
{
    [SerializeField] public int id;
    [SerializeField] public string name;
    [SerializeField] public string description;
    [SerializeField] public DictionaryStringToInt rewards;
    [SerializeField] public Func<bool> condition;
    [SerializeField] public QuestType type;
    public enum QuestType
    {
        Daily,
        Achievement,
        Story
    }
}


