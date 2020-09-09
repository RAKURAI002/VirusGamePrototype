using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
/// <summary>
/// Implement BirthMark Effects.
/// </summary>
/// 

[System.Serializable]
public class BirthMarkSerializer : ISerializationCallbackReceiver
{
    [NonSerialized]
    public List<BirthMarkData> birthMarkDatas = new List<BirthMarkData>();

    [SerializeField]
    DictionaryStringToString datas;

    public void OnAfterDeserialize()
    {
        foreach(var data in datas)
        {
            Type type = Type.GetType(data.Value);
            birthMarkDatas.Add((BirthMarkData)JsonUtility.FromJson(data.Key, type));

        }
    }

    public void OnBeforeSerialize()
    {
        datas = new DictionaryStringToString();

        foreach (BirthMarkData birthMarkData in birthMarkDatas)
        {
            datas.Add(JsonUtility.ToJson(birthMarkData), birthMarkData.GetType().ToString());
            Debug.Log($"{datas.Keys}");

        }

    }
}


[System.Serializable]
public abstract class BirthMarkData 
{
    [SerializeField] public string name;
    [SerializeField] public int tier;
    [SerializeField] public int level;
    [SerializeField] public string spritePath;


    [SerializeField] public List<float> effectValues;

    public abstract BirthMarkData CreateBirthMarkObject<T>();


}

[System.Serializable]
public class IncreaseSTATSBirthMark : BirthMarkData
{
    
    [SerializeField] public FieldInfo statToIncrease;

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

[System.Serializable]
public class DoubleProductBirthMark : BirthMarkData
{
    [SerializeField] public Building.BuildingType buildingType;

    public override BirthMarkData CreateBirthMarkObject<T>()
    {
        return new ParticularEffectOnBuildingBirthMark();
    }

}

[System.Serializable]
public class AddMoreActionBirthMark : BirthMarkData
{

    public override BirthMarkData CreateBirthMarkObject<T>()
    {
        return new ParticularEffectOnBuildingBirthMark();
    }

}

