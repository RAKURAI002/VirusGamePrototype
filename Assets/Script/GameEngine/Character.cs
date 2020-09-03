using System;
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
        public int craftsmanship;
        public int intelligence;
        public int strength;
        public int perception;
        public int luck;
        public int immunity;

    }

    public enum GenderType
    {
        Unknown,
        Male,
        Female

    }
    [System.Serializable]
    public class BirthMark
    {
        [SerializeField] string name;
        [SerializeField] string spritePath;
        [SerializeField] int tier;
        [SerializeField] int level;
        [SerializeField] string effectName;

        

    }

    public enum BirthMarkPosition
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

        FieldInfo[] fInfos = stats.GetType().GetFields();
        for (int i = 0; i < fInfos.Length; i++)
        {
            FieldInfo fInfo = fInfos[i];
            fInfo.SetValue(this.stats, (int)fInfo.GetValue(this.stats) + 1);

        }
        for (int i = 0; i < (50 - fInfos.Length); i++)
        {
            FieldInfo fInfo = stats.GetType().GetFields()[UnityEngine.Random.Range(0, fInfos.Length)];
            fInfo.SetValue(this.stats, (int)fInfo.GetValue(this.stats) + 1);

        }

        level = 1;
        birthMarks = new List<BirthMarkPosition>();
        birthMarks.Add(BirthMarkPosition.Arm);

        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        spritePath = "Sprites/Character/Character" + UnityEngine.Random.Range(1, 10).ToString();
        effects = new List<Resource.Effect>();

    }
    public int GenerateID()
    {
        if (CharacterManager.Instance.AllCharacters.Count == 0)
        {
            return IDMask.CHARACTER_ID_MASK + 1;

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
        birthMarks = new List<BirthMarkPosition>();
        birthMarks.Add(BirthMarkPosition.Arm);

        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        this.spritePath = spritePath;
        effects = new List<Resource.Effect>();

    }

    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] public int level;
    [SerializeField] private int experience;
    [SerializeField] private GenderType gender;
    [SerializeField] private AllStats stats;
    [SerializeField] private List<BirthMarkPosition> birthMarks;

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
    public List<BirthMarkPosition> BirthMarks { get { return birthMarks; } }
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
        stats.craftsmanship += _stats.craftsmanship;
        stats.immunity += _stats.immunity;
        stats.intelligence += _stats.intelligence;
        stats.luck += _stats.luck;
        stats.perception += _stats.perception;
        stats.speed += _stats.speed;
        stats.strength += _stats.strength;

    }

    public void DecreaseStats(AllStats _stats)
    {
        stats.craftsmanship -= _stats.craftsmanship;
        stats.immunity -= _stats.immunity;
        stats.intelligence -= _stats.intelligence;
        stats.luck -= _stats.luck;
        stats.perception -= _stats.perception;
        stats.speed -= _stats.speed;
        stats.strength -= _stats.strength;

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
