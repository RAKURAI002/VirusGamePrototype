using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Builder
{
    [System.Serializable]
    public struct Construction {
        public bool isConstructing;
        public int constructPointRequired;
        public int teamNumber;
        public float currentPoint;
        public float finishPoint;

    }
    public Builder()
    {     
    }
    public Builder(Building.BuildingType type)
    {
      //  this.id = BuildManager.Instance.AllBuildings.Count;
        this.type = type;
        InitializeData();

    }
    public Builder(Building.BuildingType type, Vector3 pos, GameObject representGameObject)
    {
        this.id = GenerateID();
        this.type = type;
        this.position = pos;
        this.representGameObject = representGameObject;
        this.level = 0;
        this.constructionStatus = new Construction { isConstructing = true, constructPointRequired = 10 };

        InitializeData();

    }
    public Builder(Building.BuildingType type, Vector3 pos, int level, GameObject representGameObject, Construction constructionStatus)
    {
        this.id = GenerateID();
        this.type = type;
        this.position = pos;
        this.representGameObject = representGameObject;
        this.level = level;
        this.constructionStatus = constructionStatus;

        InitializeData();

    }

    [SerializeField] private int id;
    [SerializeField] private Building.BuildingType type;
    [SerializeField] private Vector3 position;
    [SerializeField] private int level;
    [SerializeField] public Construction constructionStatus;
    [SerializeField] private int currentActiveAmount = 0;
    [SerializeField] private List<int> teamLockState;

    [SerializeField] private List<CharacterWrapper> characterTeamsInBuilding;

    [SerializeField] public float currentProductionAmount;

    [NonSerialized] public GameObject representGameObject;
    [NonSerialized] public int maxActiveAmount;
    [NonSerialized] public int maxLevel;

    public List<int> TeamLockState {
        get { return teamLockState ?? (teamLockState = new List<int>()); }
        set { teamLockState = value; }

    }
    public int GenerateID()
    {
        if (BuildManager.Instance.AllBuildings.Count == 0)
        {
            return Constant.IDMask.BUILDING_ID_MASK + 1;

        }

        return (BuildManager.Instance.AllBuildings.Select(b => b.id).Max() + 1 );

    }
    public List<CharacterWrapper> CharacterInBuilding
    {
        get
        {          
            if(characterTeamsInBuilding == null)
            {
                characterTeamsInBuilding = new List<CharacterWrapper>();
            }

            int maxTeam = LoadManager.Instance.allBuildingData[this.type].maxCharacterStored[level].amount.Count;
            for (int i = 0; i < maxTeam; i++)
            {
                    if (characterTeamsInBuilding.ElementAtOrDefault(i) == null)
                    {
                            characterTeamsInBuilding.Add(new CharacterWrapper());

                    }

            }

            return characterTeamsInBuilding ;

        }
        
        set
        {
            characterTeamsInBuilding = value;

        }

    }

    public int ID { get { return id; } }
    public Building.BuildingType Type { get { return type; }}
    public Vector3 Position { get { return position; } set { position = value; } }
    public int Level { get { return level; } set { level = value; } }
    public int CurrentActiveAmount { get { return currentActiveAmount; } set { currentActiveAmount = value; } }

    public void InitializeData()
    {
        Building buildingData = LoadManager.Instance.allBuildingData[this.Type];
        if (buildingData == null)
        {
            Debug.LogError("Can't Find Building data of " + this.type);
            return;

        }
        
        this.maxActiveAmount = buildingData.maxActiveAmount;
        this.maxLevel = buildingData.maxLevel;

    }

    public override string ToString() 
    {
        return ($"Type : {type.ToString()}, Position {position}, Level : {level}, CurrentActiveAmount : {currentActiveAmount}, Constructing Status : {constructionStatus.isConstructing} " +
            $" Current active amount : {CurrentActiveAmount}/{maxActiveAmount}, Represent GO : {representGameObject.name}");

    }


}
