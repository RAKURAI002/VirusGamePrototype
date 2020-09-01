using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Building 
{
    public Building()
    {
    }

    public Building(BuildingType type, List<DictionaryStringToInt> buildingCost, List<DictionaryStringToInt> production, List<DictionaryStringToInt> consuming, List<int> upgradePoint,
         int maxActiveAmount, int maxLevel, CharacterAmountDictionary maxExtraCharacterStored, string description, List<string> spritePath)
    {
        this.type = type;
        this.buildingCost = buildingCost;
        this.production = production;
        this.consuming = consuming;
        this.upgradePoint = upgradePoint;
        this.maxActiveAmount = maxActiveAmount;
        this.maxLevel = maxLevel;

        this.spritePath = spritePath;
        this.maxCharacterStored = maxExtraCharacterStored;
        this.description = description;

    }

    public Building(BuildingType type, List<DictionaryStringToInt> buildingCost, List<DictionaryStringToInt> production, List<DictionaryStringToInt> consuming, List<int> upgradePoint,
      int maxActiveAmount, int maxLevel, CharacterAmountDictionary maxExtraCharacterStored, string description, List<string> spritePath, List<int> maxProductionStored, string typeresourcePath)
    {
        this.type = type;
        this.buildingCost = buildingCost;
        this.production = production;
        this.consuming = consuming;
        this.upgradePoint = upgradePoint;
        this.maxActiveAmount = maxActiveAmount;
        this.maxLevel = maxLevel;
        this.spritePath = spritePath;
        this.maxCharacterStored = maxExtraCharacterStored;
        this.description = description;
        this.maxProductionStored = maxProductionStored;
        this.productionSpritePath = typeresourcePath;
    }
    [System.Serializable]
    public enum BuildingType
    {
        Unknown,
        Farm,
        Kitchen,
        Laboratory,
        MedicalCenter,
        QuarantineSite,
        Residence,
        TownBase,
        WaterTreatmentCenter,
        WareHouse,
        FishingPond,
        LaborCenter,
        Armory,
        TradingCenter,

        Mine,
        LumberYard

    }

    [SerializeField] public BuildingType type;
    [SerializeField] public List<DictionaryStringToInt> buildingCost;
    [SerializeField] public List<DictionaryStringToInt> production;
    [SerializeField] public List<DictionaryStringToInt> consuming;
    [SerializeField] public List<int> upgradePoint;
    [SerializeField] public int maxActiveAmount;
    [SerializeField] public int maxLevel;

    [SerializeField] public CharacterAmountDictionary maxCharacterStored;

    [SerializeField] public List<string> spritePath;
    [SerializeField] public string description;

    [SerializeField] public List<int> maxProductionStored;
    [SerializeField] public string productionSpritePath;

}
