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
        this.behaviorType = GetBuilderBehaviorType();
    }

    public Building(BuildingType type, List<DictionaryStringToInt> buildingCost, List<DictionaryStringToInt> production, List<DictionaryStringToInt> consuming, List<int> upgradePoint,
      int maxActiveAmount, int maxLevel, CharacterAmountDictionary maxExtraCharacterStored, string description, List<string> spritePath, List<int> maxProductionStored, string typeresourcePath)
    : this(type, buildingCost, production, consuming, upgradePoint, maxActiveAmount, maxLevel, maxExtraCharacterStored, description, spritePath)
    {
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
        Fishery,
        LaborCenter,
        Armory,
        TradingCenter,

        Mine,
        LumberYard

    }
    public enum BuildingBehaviorType
    {
        Unknown,
        Normal,
        ResourceCollector,
        Crafting,
        ParticularFunction,
    }

    BuildingBehaviorType GetBuilderBehaviorType()
    {
        switch (this.type)
        {
            case Building.BuildingType.Armory: return BuildingBehaviorType.Crafting;
            case Building.BuildingType.Farm: return BuildingBehaviorType.ResourceCollector;
            case Building.BuildingType.Fishery: return BuildingBehaviorType.ParticularFunction;
            case Building.BuildingType.Kitchen: return BuildingBehaviorType.Crafting;
            case Building.BuildingType.Laboratory: return BuildingBehaviorType.ParticularFunction;
            case Building.BuildingType.LaborCenter: return BuildingBehaviorType.Normal;
            case Building.BuildingType.LumberYard: return BuildingBehaviorType.ResourceCollector;
            case Building.BuildingType.MedicalCenter: return BuildingBehaviorType.Crafting;
            case Building.BuildingType.Mine: return BuildingBehaviorType.ResourceCollector;
            case Building.BuildingType.QuarantineSite: return BuildingBehaviorType.ParticularFunction;
            case Building.BuildingType.Residence: return BuildingBehaviorType.ParticularFunction;
            case Building.BuildingType.TownBase: return BuildingBehaviorType.Normal;
            case Building.BuildingType.TradingCenter: return BuildingBehaviorType.Normal;
            case Building.BuildingType.WareHouse: return BuildingBehaviorType.Normal;
            case Building.BuildingType.WaterTreatmentCenter: return BuildingBehaviorType.ResourceCollector;
            default: return BuildingBehaviorType.Unknown;
        }

    }




    [SerializeField] public BuildingType type;
    [SerializeField] public List<DictionaryStringToInt> buildingCost;
    [SerializeField] public List<DictionaryStringToInt> production;
    [SerializeField] public List<DictionaryStringToInt> consuming;
    [SerializeField] public List<int> upgradePoint;
    [SerializeField] public int maxActiveAmount;
    [SerializeField] public int maxLevel;
    [SerializeField] public BuildingBehaviorType behaviorType;
    [SerializeField] public CharacterAmountDictionary maxCharacterStored;

    [SerializeField] public List<string> spritePath;
    [SerializeField] public string description;

    [SerializeField] public List<int> maxProductionStored;
    [SerializeField] public string productionSpritePath;

    public bool UnDestroyableBuilding()
    {
        return (type == BuildingType.LaborCenter || type == BuildingType.TownBase);
    }
}
