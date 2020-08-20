using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public PlayerData()
    {
        buildingInPossession = new List<Builder>();
        characterInPossession = new List<Character>();
        resourceInPossession = new DictionaryStringToInt();
        equipmentInPossession = new EquipmentDictionary();
        //questInProgress = new QuestDictionary();
        currentActivities = new ActivityProgressDictionary();
        expandedArea = new List<int>();

        level = 1;
    }
    // public static string _UID { get; set; }
    [SerializeField] public string UID = "DEFAULT_UID";
    [SerializeField] public string name = "DEFAULT_NAME";
    [SerializeField] public int level;


   [SerializeField] public List<Builder> buildingInPossession;
    [SerializeField] public List<Character> characterInPossession;
    [SerializeField] public DictionaryStringToInt resourceInPossession;
    [SerializeField] public EquipmentDictionary equipmentInPossession;

    [SerializeField] public List<int> expandedArea;

  //  [SerializeField] public QuestDictionary questInProgress;

    [SerializeField] public ActivityProgressDictionary currentActivities;

    [SerializeField] public long lastLoginTime; 
}
