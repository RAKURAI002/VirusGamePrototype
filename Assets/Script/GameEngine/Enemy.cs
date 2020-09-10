using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy 
{
    public Enemy(int id, string name, AllStats stats)
    {
        this.id = id;
        this.name = name;
        this.stats = stats;
    }
    [System.Serializable]
    public struct AllStats
    {
        public int attack;
        public int defense;
        public int speed;
        public int perspective;
        public int hitPoint;
        public int intelligence;
    }
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private AllStats stats;

    private int? currentHp;
    public int? CurrentHp { get { return currentHp == null ? stats.hitPoint : currentHp;   } set { currentHp = value; } }

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public AllStats Stats { get { return stats; } set { stats = value; } }


}
