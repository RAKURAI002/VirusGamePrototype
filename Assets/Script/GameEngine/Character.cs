using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class Character
{

    [System.Serializable]
    public class CharacterData
    {
        [SerializeField] public string name;
        [SerializeField] public GenderType gender;
        [SerializeField] public string spritePath;

    }

    [System.Serializable]
    public class AllStats
    {
        public int speed;
        public int crafting;
        public int intelligence;
        public int strength;
        public int observing;
        public int luck;
        public int healthy;

        public int hitPoint;
        public int attack;
        public int defense;

    }

    public enum GenderType
    {
        Unknown,
        Male,
        Female

    }
    public enum BirthMark
    {
        Unknown,
        Head,
        Shoulder,
        Back,
        Arm,
        Leg,
        Foot

            
    }
    public enum HealthStatus
    {
        Unknown,
        Healthly,
        Starve,
        Infected

    }
    public enum WorkStatus
    {
        Unknown,
        Idle,
        Working,
        Quest

    }

    public Character(string name)
    {
        id = GenerateID();
        this.name = name;
        gender = Convert.ToBoolean(UnityEngine.Random.Range(0, 2)) ? GenderType.Male : GenderType.Female;

        stats = new AllStats();

        for (int i = 0; i < 50; i++)
        {
            FieldInfo fInfo = stats.GetType().GetFields()[UnityEngine.Random.Range(0, 7)];
            fInfo.SetValue(this.stats, (int)fInfo.GetValue(this.stats) + 1);

        }

        level = 1;
        birthMarks = new List<BirthMark>();
        birthMarks.Add(BirthMark.Arm);

        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        spritePath = "Sprites/Character/Character" + UnityEngine.Random.Range(1, 10).ToString();
        effects = new List<Resource.Effect>();
        currentHp = stats.hitPoint;

    }
    public int GenerateID()
    {
        if (CharacterManager.Instance.AllCharacters.Count == 0)
        {
            return 1;

        }

        int maxIDCandidate1 = CharacterManager.Instance.AllCharacters.Select(c => c.id).Max();
        int maxIDCandidate2 = 0;
        if(CharacterManager.Instance.characterWaitingInLine.Count > 0)
        {
            maxIDCandidate2 = CharacterManager.Instance.characterWaitingInLine.Select(c => c.id).Max();
        }
       

        return Mathf.Max(maxIDCandidate1, maxIDCandidate2) + 1;

    }

    public Character(string name, GenderType gender, string spritePath)
    {
        id = GenerateID();
        this.name = name;
        this.gender = gender;

        stats = new AllStats();

        for (int i = 0; i < 50; i++)
        {
            FieldInfo fInfo = stats.GetType().GetFields()[UnityEngine.Random.Range(0, 7)];
            fInfo.SetValue(this.stats, (int)fInfo.GetValue(this.stats) + 1);

        }

        level = 1;
        birthMarks = new List<BirthMark>();
        birthMarks.Add(BirthMark.Arm);

        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        this.spritePath = spritePath;
        effects = new List<Resource.Effect>();
        currentHp = stats.hitPoint;

    }

    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] public int level;
    [SerializeField] private int experience;
    [SerializeField] private GenderType gender;
    [SerializeField] private AllStats stats;
    [SerializeField] private List<BirthMark> birthMarks;

    [SerializeField] public List<Equipment> equipments;
    [SerializeField] public WorkStatus workStatus;
    [SerializeField] public HealthStatus healthStatus;
    [SerializeField] private int workingPlaceID = -1;

    [SerializeField] public int statsUpPoint;
    [SerializeField] public int currentHp;

    [SerializeField] public List<Resource.Effect> effects;


    [SerializeField] public string spritePath;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public int Experience { get { return experience; } }
    public GenderType Gender { get { return gender; } }
    public AllStats Stats { get { return stats; } set { stats = value; } }
    public List<BirthMark> BirthMarks { get { return birthMarks; } }
    public int WorkingPlaceID { get; set; }


    public void AddEXP(int exp)
    {
        this.experience += exp;

        if (experience >= level * 5 * (level + 1))
        {
            level++;
            statsUpPoint += 3;
        }

    }
    public void IncreaseStats(AllStats _stats)
    {
        stats.crafting += _stats.crafting;
        stats.healthy += _stats.healthy;
        stats.intelligence += _stats.intelligence;
        stats.luck += _stats.luck;
        stats.observing += _stats.observing;
        stats.speed += _stats.speed;
        stats.strength += _stats.strength;

        stats.attack += _stats.attack;
        stats.defense += _stats.defense;
        stats.hitPoint += _stats.hitPoint;

    }

    public void DecreaseStats(AllStats _stats)
    {
        stats.crafting -= _stats.crafting;
        stats.healthy -= _stats.healthy;
        stats.intelligence -= _stats.intelligence;
        stats.luck -= _stats.luck;
        stats.observing -= _stats.observing;
        stats.speed -= _stats.speed;
        stats.strength -= _stats.strength;

        stats.attack -= _stats.attack;
        stats.defense -= _stats.defense;
        stats.hitPoint -= _stats.hitPoint;

    }


    public override string ToString()
    {
        return ($"\n   Character Information :\nID : {id}\nName : {name}\nGender : {gender}\nStats : {stats}\nBirthMark(s) : {string.Join(" ", birthMarks)}\nEqiupment(s) : {string.Join(" ", equipments.Select(e => $"{e.ID}, {e.Name}"))}" +
            $"\nWorkStatus : {workStatus}\nHealthStatus : {healthStatus}");

    }

}
[System.Serializable]
public class CharacterWrapper
{
    [SerializeField] private List<Character> characters;

    public List<Character> Characters
    {
        get { return characters ?? (characters = new List<Character>()); }
        set { characters = value; }

    }

}
