using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement BirthMark Effects using Factory Method Pattern.
/// </summary>
public enum BirthMarkType
{
    IncreaseSTATSBirthMark


}

public abstract class BirthMarkData
{
    [SerializeField] public string effectName;
    [SerializeField] public int tier;
    [SerializeField] public string spritePath;

    [SerializeField] public List<int> effectValues;

    public abstract BirthMarkData CreateBirthMarkObject<T>();


}

[System.Serializable]
public class IncreaseSTATSBirthMark : BirthMarkData
{
    
    [SerializeField] public Character.AllStats statToIncrease;

    [SerializeField] public bool isAlliesAffected;


    public IncreaseSTATSBirthMark()
    {

    }

    public override BirthMarkData CreateBirthMarkObject<T>()
    {
        return new IncreaseSTATSBirthMark();
    }

}

[System.Serializable]
public class ParticularEffectOnBuildingBirthMark : BirthMarkData
{
    [SerializeField] public Building.BuildingType buildingType;

    public override BirthMarkData CreateBirthMarkObject<T>()
    {
        return new ParticularEffectOnBuildingBirthMark();
    }

}


