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
        /*
        public override string ToString()
        {
            return string.Concat(this.GetType().GetFields().Select(f => f.Name));
        }*/
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
        public BirthMark(BirthMarkData birthMarkData)
        {
            this.name = birthMarkData.name;
            this.spritePath = birthMarkData.spritePath;
            this.tier = birthMarkData.tier;
            this.level = UnityEngine.Random.Range(1, birthMarkData.effectValues.Count + 1);
            this.type = birthMarkData.GetType().ToString();

        }

        [SerializeField] public  string name;
        [SerializeField] public string spritePath;
        [SerializeField] public string type;
        [SerializeField] public int tier;
        [SerializeField] public int level;

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
        level = 1;

        RandomCharacterStats();
        RandomBirthMark();
        ApplyBirthMarkStats();
        equipments = new List<Equipment>();
        workStatus = WorkStatus.Idle;
        healthStatus = HealthStatus.Healthly;
        spritePath = "Sprites/Character/Character" + UnityEngine.Random.Range(1, 10).ToString();
        effects = new List<Resource.Effect>();



    }
    void ApplyBirthMarkStats()
    {
        BirthMark[] statsBirthMarks = birthMarks.Where(bm => bm.type == typeof(IncreaseSTATSBirthMark).ToString()).ToArray();
        foreach (BirthMark birthMark in statsBirthMarks)
        {
            BirthMarkData birthMarkData = LoadManager.Instance.allBirthMarkDatas[birthMark.name];

            FieldInfo fInfo = typeof(Character.AllStats).GetField(((IncreaseSTATSBirthMark)birthMarkData).statToIncrease.ToLower());

            int oldStat = (int)fInfo.GetValue(this.stats);
            fInfo.SetValue(this.stats, Mathf.RoundToInt( oldStat * (1 + birthMarkData.effectValues[birthMark.level - 1])));

        }

    }
    void RandomCharacterStats()
    {
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
    }
    public Character(string name, GenderType gender, string spritePath) : this(name)
    {
        this.gender = gender;
        this.spritePath = spritePath;

    }

    public int GenerateID()
    {
        if (CharacterManager.Instance.AllCharacters.Count == 0)
        {
            return Constant.IDMask.CHARACTER_ID_MASK + 1;

        }

        int maxIDCandidate1 = CharacterManager.Instance.AllCharacters.Select(c => c.id).Max();
        int maxIDCandidate2 = 0;
        if (CharacterManager.Instance.characterWaitingInLine.Count > 0)
        {
            maxIDCandidate2 = CharacterManager.Instance.characterWaitingInLine.Select(c => c.id).Max();
        }

        return Mathf.Max(maxIDCandidate1, maxIDCandidate2) + 1;

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
    [SerializeField] public int hitPoint;

    [SerializeField] public List<Resource.Effect> effects;

    [SerializeField] public string spritePath;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public int Experience { get { return experience; } }
    public GenderType Gender { get { return gender; } }
    public AllStats Stats { get { return stats; } set { stats = value; } }
    public List<BirthMark> BirthMarks { get { return birthMarks; } }
    public int WorkingPlaceID { get; set; }

    void RandomBirthMark()
    {
        int totalDivisor = 0;
        List<int> chanceTable = new List<int>();
        foreach(var chance in typeof(Constant.BirthMarkAmountChance).GetFields())
        {
            chanceTable.Add((int)chance.GetValue(null));
            totalDivisor += (int)chance.GetValue(null);

        }
         

        int randomBirthMarkAmount = UnityEngine.Random.Range(0, totalDivisor);
       // Debug.Log($"Total = {randomBirthMarkAmount} /  {totalDivisor}");
        for (int i = 0; i < chanceTable.Count; i++)
        {
            if(randomBirthMarkAmount < chanceTable[i])
            {
                AddBirthMark(i);
                break;

            }
            else
            {
                randomBirthMarkAmount -= chanceTable[i];

            }

        }


        return;

    }
    void AddBirthMark(int amount)
    {
         Debug.Log($"Start random {amount} BirthMark(s) . . .");
        birthMarks = new List<BirthMark>();
        if(amount == 0)
        {
            return;

        }

        int totalDivisor = 0;
        List<int> chanceTable = new List<int>();
        foreach (var chance in typeof(Constant.BirthMarkTierChance).GetFields())
        {
            chanceTable.Add((int)chance.GetValue(null));
            totalDivisor += (int)chance.GetValue(null);

        }
        
        for (int birthMarkAmount = 0 ; birthMarkAmount < amount ; birthMarkAmount++)
        {
            int randomTier = UnityEngine.Random.Range(0, totalDivisor);
            for (int birthMarkTier = 1; birthMarkTier <= chanceTable.Count; birthMarkTier++)
            {
                if (randomTier < chanceTable[birthMarkTier - 1])
                {
                    Debug.Log($"Random Tier {birthMarkTier} BirthMark to character.");
                    
                    List<BirthMarkData> bmData = LoadManager.Instance.allBirthMarkDatas.Where(bm => bm.Value.tier == birthMarkTier).Select(bm => bm.Value).ToList();
                    int randomEffect = UnityEngine.Random.Range(0, bmData.Count);
                    birthMarks.Add(new BirthMark(bmData[randomEffect]));

                    break;

                }
                else
                {
                    randomTier -= chanceTable[birthMarkAmount];

                }

            }
        }
        return;
    }
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
