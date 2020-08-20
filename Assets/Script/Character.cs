using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Character 
{
    [System.Serializable]
    public struct AllStats {
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
        id = CharacterManager.Instance.AllCharacters.Count;
        this.name = name;
        gender = Convert.ToBoolean(UnityEngine.Random.Range(0, 2)) ? GenderType.Male : GenderType.Female;
        stats = new AllStats
        {
            speed = UnityEngine.Random.Range(1, 11),
            crafting = UnityEngine.Random.Range(1, 11),
            intelligence = UnityEngine.Random.Range(1, 11),
            healthy = UnityEngine.Random.Range(1, 11),
            luck = UnityEngine.Random.Range(1, 11),
            observing = UnityEngine.Random.Range(1, 11),
            strength = UnityEngine.Random.Range(1, 11),
            hitPoint = UnityEngine.Random.Range(20, 31),
            attack = UnityEngine.Random.Range(20, 31),
            defense = UnityEngine.Random.Range(20, 31)
        };
        birthMarks = new List<BirthMark>();
        birthMarks.Add(BirthMark.Arm);

        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        spritePath = "Sprites/Character/Character" + UnityEngine.Random.Range(1, 10).ToString();

        currentHp = stats.hitPoint;
    }


    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private GenderType gender;
    [SerializeField] private AllStats stats;
    [SerializeField] private List<BirthMark> birthMarks;

    [SerializeField] public List<Equipment> equipments;
    [SerializeField] public WorkStatus workStatus;
    [SerializeField] public HealthStatus healthStatus;
    [SerializeField] private int workingPlaceID = -1;

    [SerializeField] public int currentHp;

    [SerializeField] public string spritePath;

    public int ID { get { return id; } }
    public string Name{ get { return name; } }
    public GenderType Gender{ get { return gender; } }
    public AllStats Stats{ get { return stats; } set { stats = value; } }
    public List<BirthMark> BirthMarks{ get { return birthMarks; } }
    public int WorkingPlaceID { get; set; }

    public void IncreaseStats(AllStats stats)
    {
        
    }

    public override string ToString()
    {
        return ($"\n   Character Information :\nID : {id}\nName : {name}\nGender : {gender}\nStats : {stats}\nBirthMark(s) : {string.Join(" ", birthMarks)}\nEqiupment(s) : {string.Join(" ", equipments.Select( e => $"{e.ID}, {e.Name}"))}" +
            $"\nWorkStatus : {workStatus}\nHealthStatus : {healthStatus}");
    }


}
[System.Serializable]
public class CharacterWrapper
{
    [SerializeField] private List<Character> characters;


    public List<Character> Characters { get { return characters ?? (characters = new List<Character>()); }
        set { characters = value; }
    }
}
