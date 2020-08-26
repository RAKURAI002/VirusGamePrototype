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
        resourceInPossession = new ResourceDictionary();
        equipmentInPossession = new EquipmentDictionary();
        //questInProgress = new QuestDictionary();
        currentActivities = new ActivityProgressDictionary();
        expandedArea = new List<int>();
        level = 1;

    }

    [SerializeField] public string UID = "DEFAULT_UID";
    [SerializeField] public string name = "DEFAULT_NAME";
    [SerializeField] public int level;
    [SerializeField] public List<Builder> buildingInPossession;
    [SerializeField] public List<Character> characterInPossession;
    [SerializeField] public ResourceDictionary resourceInPossession;
    [SerializeField] public EquipmentDictionary equipmentInPossession;

    [SerializeField] public List<int> expandedArea;

    [SerializeField] public ActivityProgressDictionary currentActivities;

    [SerializeField] public bool completeTutorial;
    [SerializeField] public long lastLoginTime;

}
