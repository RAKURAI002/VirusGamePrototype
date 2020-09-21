using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();


    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }


    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception($"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}

[System.Serializable]
public class DictionaryIntToString : SerializableDictionary<int, string>
{ }
[System.Serializable] 
public class DictionaryIntToInt : SerializableDictionary<int, int> 
{ }
[System.Serializable]
public class DictionaryStringToString : SerializableDictionary<string, string>
{ }
[System.Serializable]
public class DictionaryStringToInt : SerializableDictionary<string, int>
{ }
[System.Serializable]
public class CharacterDictionary : SerializableDictionary<int, int>
{ }
[System.Serializable]
public class ResourceDictionary : SerializableDictionary<string, Resource>
{ }
[System.Serializable]
public class EquipmentDictionary : SerializableDictionary<string, Equipment>
{ }

[System.Serializable]
public class BuildingDictionary : SerializableDictionary<Building.BuildingType, Building>
{ }
public class BuilderDictionary : SerializableDictionary<int, Builder>
{ }
[System.Serializable]
public class QuestDataDictionary : SerializableDictionary<int, QuestData>
{ }
[System.Serializable]
public class BirthMarkDataDictionary : SerializableDictionary<string, BirthMarkData>
{ }
[System.Serializable]
public class EnemyDictionary : SerializableDictionary<int, Enemy>
{ }
[System.Serializable]
public class CharacterAmountDictionary : SerializableDictionary<int, MaxCharacterStored>
{ }
[System.Serializable]
public class ActivityProgressDictionary : SerializableDictionary<int, ActivityInformation>
{ }

[System.Serializable]
public enum ActivityType
{
    Quest,
    Build,
    Craft,
    Pregnancy,
    CharacterGrowing

}
[System.Serializable] 
public class ActivityInformation
{
    public int activityID;
    public string activityName;
    public ActivityType activityType;

    public int informationID;
    public int builderReferenceID;
    public int teamNumber;

    public bool isFinished;
    public long startPoint;
    public float currentPoint;
    public long requiredPoint;

    public long finishTime;

    public int androidNotificationID;


}

[System.Serializable]
public struct MaxCharacterStored
{
    public List<int> amount;
}

[System.Serializable]
public class QuestDictionary : SerializableDictionary<int, ActivityInformation>
{ }



