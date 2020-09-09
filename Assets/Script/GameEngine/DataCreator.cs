using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataCreator : MonoBehaviour
{
    void Start()
    {
        CreateBuildingJsonData();
        CreateEnemyData();
        CreateQuestJsonData();
        CreateResourceData();
        CreateBirthMarkData();
        CreateAchievementData();
    }

    List<string> GetSpritePath(string name)
    {

        List<string> spritePath = new List<string>();
        spritePath.Add($"Sprites/Building/{name}0");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");
        spritePath.Add($"Sprites/Building/{name}1");

        return spritePath;
    }
    void CreateEquipmentJsonData()
    {
        List<Equipment> equipment = new List<Equipment>();
        equipment.Add(new Equipment(1, "Item1", Item.RarityTier.Uncommon, "Too mighty blade.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item1"), new Character.AllStats { strength = 8, speed = -1 }));
        equipment.Add(new Equipment(2, "Item2", Item.RarityTier.UltraRare, "Seems like a MONKEY's heart ?", Equipment.EquipmentPosition.Body, ("Sprites/Equipments/Item2"), new Character.AllStats { strength = 20, speed = 10, immunity = 20 }));
        equipment.Add(new Equipment(3, "Item3", Item.RarityTier.Uncommon, "What is this ?", Equipment.EquipmentPosition.Face, ("Sprites/Equipments/Item3"), new Character.AllStats { strength = 1, perception = 3 }));
        equipment.Add(new Equipment(4, "Item4", Item.RarityTier.Rare, "Hmmm . . .", Equipment.EquipmentPosition.Leg, ("Sprites/Equipments/Item4"), new Character.AllStats { speed = 4, intelligence = 4 }));
        equipment.Add(new Equipment(5, "Item5", Item.RarityTier.Rare, "Unleash your TRUE power.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item5"), new Character.AllStats { strength = 3, intelligence = 3, luck = 3, perception = 3, immunity = 3, craftsmanship = 3, speed = 3 }));
        equipment.Add(new Equipment(6, "Item6", Item.RarityTier.Uncommon, "An ancient Sorcerer's boots.", Equipment.EquipmentPosition.Foot, ("Sprites/Equipments/Item6"), new Character.AllStats { strength = 2, intelligence = 5 }));
        equipment.Add(new Equipment(7, "Item7", Item.RarityTier.Rare, "That seems heavy.", Equipment.EquipmentPosition.Body, ("Sprites/Equipments/Item7"), new Character.AllStats { strength = 12, speed = -3 }));
        equipment.Add(new Equipment(8, "Item8", Item.RarityTier.Uncommon, "Can cut everything except trees.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item8"), new Character.AllStats { strength = 6, speed = -1 }));
        equipment.Add(new Equipment(9, "Item9", Item.RarityTier.Uncommon, "Make you more tanky.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item9"), new Character.AllStats { strength = 3 }));
        equipment.Add(new Equipment(10, "Item10", Item.RarityTier.Rare, "Smell bloody . . .", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item10"), new Character.AllStats { luck = -1, strength = 6, intelligence = 1 }));
        equipment.Add(new Equipment(11, "Item11", Item.RarityTier.SuperRare, "Better than your Nike :)", Equipment.EquipmentPosition.Foot, ("Sprites/Equipments/Item11"), new Character.AllStats { speed = 8 }));
        equipment.Add(new Equipment(12, "Item12", Item.RarityTier.Uncommon, "How this could wear as pants ?", Equipment.EquipmentPosition.Leg, ("Sprites/Equipments/Item12"), new Character.AllStats { }));
        equipment.Add(new Equipment(13, "Item13", Item.RarityTier.Uncommon, "A certain shield.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item13"), new Character.AllStats { strength = 4, speed = -1 }));
        equipment.Add(new Equipment(14, "Item14", Item.RarityTier.SuperRare, "Cheese? on head?", Equipment.EquipmentPosition.Head, ("Sprites/Equipments/Item14"), new Character.AllStats { perception = 15, luck = 15 }));
        equipment.Add(new Equipment(15, "Item15", Item.RarityTier.Rare, "A cursed crown of something. Bruhhh...", Equipment.EquipmentPosition.Head, ("Sprites/Equipments/Item15"), new Character.AllStats { strength = 10, intelligence = 10, immunity = 10, luck = -10 }));

        string equipmentDatas = JsonHelper.ToJson(equipment.ToArray(), true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Creating JSON data : " + equipmentDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/EquipmentData.json", equipmentDatas);

    }

    void CreateBuildingJsonData()
    {
        List<DictionaryStringToInt> resourceBuildingCost = new List<DictionaryStringToInt>();
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 10 }, { "Wood", 10 } });          //shop->lv1
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 50 }, { "Wood", 125 } });         //lv1->2
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 100 }, { "Wood", 250 } });        //lv2->3
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 200 }, { "Wood", 500 } });        //lv3->4
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 400 }, { "Wood", 1000 } });       //lv4->5
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 800 }, { "Wood", 2000 } });       //lv5->6
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 1600 }, { "Wood", 4000 } });      //lv6->7
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 3200 }, { "Wood", 8000 } });      //lv7->8
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 6400 }, { "Wood", 16000 } });     //lv8->9
        resourceBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 12800 }, { "Wood", 32000 } });    //lv9->10
        List<DictionaryStringToInt> craftBuildingCost = new List<DictionaryStringToInt>();
        craftBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 2500 }, { "Wood", 5000 } });         //shop->lv1
        craftBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 10000 }, { "Wood", 20000 } });       //lv1->2
        craftBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 30000 }, { "Wood", 50000 } });       //lv2->3
        List<DictionaryStringToInt> QuarantineSiteCost = new List<DictionaryStringToInt>();
        QuarantineSiteCost.Add(new DictionaryStringToInt() { { "Stone", 1000 }, { "Wood", 1000 } });          //shop->lv1
        QuarantineSiteCost.Add(new DictionaryStringToInt() { { "Stone", 10000 }, { "Wood", 15000 } });         //lv1->2
        QuarantineSiteCost.Add(new DictionaryStringToInt() { { "Stone", 11000 }, { "Wood", 20000 } });        //lv2->3
        QuarantineSiteCost.Add(new DictionaryStringToInt() { { "Stone", 12500 }, { "Wood", 25500 } });        //lv3->4
        QuarantineSiteCost.Add(new DictionaryStringToInt() { { "Stone", 14500 }, { "Wood", 315000 } });       //lv4->5
        List<DictionaryStringToInt> containBuildingCost = new List<DictionaryStringToInt>();
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 500 }, { "Wood", 500 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 1250 }, { "Wood", 2500 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 2500 }, { "Wood", 5000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 5000 }, { "Wood", 10000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 10000 }, { "Wood", 20000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 20000 }, { "Wood", 40000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 40000 }, { "Wood", 80000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 80000 }, { "Wood", 160000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 160000 }, { "Wood", 320000 } });
        containBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 320000 }, { "Wood", 640000 } });
        List<DictionaryStringToInt> labBuildingCost = new List<DictionaryStringToInt>();
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 500 }, { "Wood", 500 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 1000 }, { "Wood", 1600 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 1500 }, { "Wood", 2600 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 4500 }, { "Wood", 6100 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 10000 }, { "Wood", 12100 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 18000 }, { "Wood", 20600 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 28500 }, { "Wood", 31600 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 41500 }, { "Wood", 45100 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 57000 }, { "Wood", 61100 } });
        labBuildingCost.Add(new DictionaryStringToInt() { { "Stone", 75000 }, { "Wood", 79600 } });
        List<DictionaryStringToInt> townBaseCost = new List<DictionaryStringToInt>();
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 2500 }, { "Wood", 1250 }, { "Food", 1500 }, { "Water", 750 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 5000 }, { "Wood", 2500 }, { "Food", 3000 }, { "Water", 1500 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 10000 }, { "Wood", 5000 }, { "Food", 6000 }, { "Water", 3000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 20000 }, { "Wood", 10000 }, { "Food", 12000 }, { "Water", 6000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 40000 }, { "Wood", 20000 }, { "Food", 24000 }, { "Water", 12000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 80000 }, { "Wood", 40000 }, { "Food", 48000 }, { "Water", 24000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 160000 }, { "Wood", 80000 }, { "Food", 96000 }, { "Water", 48000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 320000 }, { "Wood", 160000 }, { "Food", 192000 }, { "Water", 96000 } });
        townBaseCost.Add(new DictionaryStringToInt() { { "Stone", 640000 }, { "Wood", 320000 }, { "Food", 384000 }, { "Water", 192000 } });
        List<DictionaryStringToInt> allProduction = new List<DictionaryStringToInt>();
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt());
        List<DictionaryStringToInt> allConsuming = new List<DictionaryStringToInt>();


        List<int> resourceUpgradePoint = new List<int>(){200,
                                            7200,
                                            14400,
                                            28800,
                                            57600,
                                            115200,
                                            230400,
                                            460800,
                                            921600,
                                            1843200, };
        List<int> craftUpgradePoint = new List<int>(){72000,
                                            360000,
                                            1080000 };
        List<int> QuarantineUpgradePoint = new List<int>(){144000,
                                            288000,
                                            576000,
                                            1152000,
                                            2304000 };
        List<int> containUpgradePoint = new List<int>(){6000,
                                            9000,
                                            18000,
                                            36000,
                                            72000,
                                            14400,
                                            28800,
                                            57600,
                                            1152000,
                                            2304000 };
        List<int> labUpgradePoint = new List<int>(){6000,
                                            13500,
                                            27000,
                                            54000,
                                            108000,
                                            216000,
                                            432000,
                                            864000,
                                            1728000,
                                            3456000 };
        List<int> townBaseUpgradePoint = new List<int>(){6000,
                                            36000,
                                            72000,
                                            144000,
                                            288000,
                                            576000,
                                            1152000,
                                            2304000,
                                            4608000,
                                            9216000 };


        CharacterAmountDictionary maxCharacterInTeam = new CharacterAmountDictionary();
        maxCharacterInTeam.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacterInTeam.Add(1, new MaxCharacterStored { amount = new List<int>() { 1 } });
        maxCharacterInTeam.Add(2, new MaxCharacterStored { amount = new List<int>() { 2 } });
        maxCharacterInTeam.Add(3, new MaxCharacterStored { amount = new List<int>() { 3 } });
        maxCharacterInTeam.Add(4, new MaxCharacterStored { amount = new List<int>() { 3, 1 } });
        maxCharacterInTeam.Add(5, new MaxCharacterStored { amount = new List<int>() { 3, 2 } });
        maxCharacterInTeam.Add(6, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });
        maxCharacterInTeam.Add(7, new MaxCharacterStored { amount = new List<int>() { 3, 3, 1 } });
        maxCharacterInTeam.Add(8, new MaxCharacterStored { amount = new List<int>() { 3, 3, 2 } });
        maxCharacterInTeam.Add(9, new MaxCharacterStored { amount = new List<int>() { 3, 3, 3 } });
        maxCharacterInTeam.Add(10, new MaxCharacterStored { amount = new List<int>() { 3, 3, 3 } });
        CharacterAmountDictionary maxCharacter = new CharacterAmountDictionary();
        maxCharacter.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(1, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(2, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(3, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(4, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(5, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(6, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(7, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(8, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(9, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(10, new MaxCharacterStored { amount = new List<int>() { 0 } });
        CharacterAmountDictionary maxCharacterInWorking = new CharacterAmountDictionary();
        maxCharacterInWorking.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacterInWorking.Add(1, new MaxCharacterStored { amount = new List<int>() { 1 } });
        maxCharacterInWorking.Add(2, new MaxCharacterStored { amount = new List<int>() { 2 } });
        maxCharacterInWorking.Add(3, new MaxCharacterStored { amount = new List<int>() { 3 } });
        maxCharacterInWorking.Add(4, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(5, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(6, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(7, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(8, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(9, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInWorking.Add(10, new MaxCharacterStored { amount = new List<int>() { 4 } });
        CharacterAmountDictionary maxCharacterInResident = new CharacterAmountDictionary();
        maxCharacterInResident.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacterInResident.Add(1, new MaxCharacterStored { amount = new List<int>() { 3 } });
        maxCharacterInResident.Add(2, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(3, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(4, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(5, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(6, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(7, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(8, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(9, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInResident.Add(10, new MaxCharacterStored { amount = new List<int>() { 4 } });
        CharacterAmountDictionary maxCharacterInCraft = new CharacterAmountDictionary();
        maxCharacterInCraft.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacterInCraft.Add(1, new MaxCharacterStored { amount = new List<int>() { 1 } });
        maxCharacterInCraft.Add(2, new MaxCharacterStored { amount = new List<int>() { 2 } });
        maxCharacterInCraft.Add(3, new MaxCharacterStored { amount = new List<int>() { 3 } });
        CharacterAmountDictionary maxCharacterInQZone = new CharacterAmountDictionary();
        maxCharacterInQZone.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacterInQZone.Add(1, new MaxCharacterStored { amount = new List<int>() { 1 } });
        maxCharacterInQZone.Add(2, new MaxCharacterStored { amount = new List<int>() { 2 } });
        maxCharacterInQZone.Add(3, new MaxCharacterStored { amount = new List<int>() { 3 } });
        maxCharacterInQZone.Add(4, new MaxCharacterStored { amount = new List<int>() { 4 } });
        maxCharacterInQZone.Add(5, new MaxCharacterStored { amount = new List<int>() { 4 } });
        List<int> farm_waterProductionStored = new List<int>(){0,
                                            600,
                                            1200,
                                            2800,
                                            5400,
                                            9000,
                                            13600,
                                            19200,
                                            25800,
                                            33400,
                                            42000 };
        List<int> woodProductionStored = new List<int>(){0,
                                            500,
                                            1000,
                                            2500,
                                            5000,
                                            8500,
                                            13000,
                                            18500,
                                            25000,
                                            32500,
                                            41000 };

        List<int> goldProductionStored = new List<int>(){0,
                                            60,
                                            120,
                                            180,
                                            240,
                                            300,
                                            360,
                                            420,
                                            480,
                                            540,
                                            600 };

        List<DictionaryStringToInt> farmResourceProduction = new List<DictionaryStringToInt>();
        farmResourceProduction.Add(new DictionaryStringToInt());
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 10 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 12 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 14 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 16 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 18 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 20 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 22 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 24 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 26 } });
        farmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 28 } });
        List<DictionaryStringToInt> mineResourceProduction = new List<DictionaryStringToInt>();
        mineResourceProduction.Add(new DictionaryStringToInt());
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 1 }, { "Stone", 6 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 2 }, { "Stone", 16 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 3 }, { "Stone", 26 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 4 }, { "Stone", 36 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 5 }, { "Stone", 46 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 6 }, { "Stone", 56 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 7 }, { "Stone", 66 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 8 }, { "Stone", 76 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 9 }, { "Stone", 86 } });
        mineResourceProduction.Add(new DictionaryStringToInt() { { "Gold", 10 }, { "Stone", 96 } });
        List<DictionaryStringToInt> residentResourceProduction = new List<DictionaryStringToInt>();
        residentResourceProduction.Add(new DictionaryStringToInt());
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 3 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 4 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 5 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 6 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 7 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 8 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 9 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 10 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 11 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 12 } });
        List<DictionaryStringToInt> laborProduction = new List<DictionaryStringToInt>();
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        laborProduction.Add(new DictionaryStringToInt() { { "Production", 20 } });
        List<DictionaryStringToInt> waterResourceProduction = new List<DictionaryStringToInt>();
        waterResourceProduction.Add(new DictionaryStringToInt());
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 10 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 12 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 14 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 16 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 18 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 20 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 22 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 24 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 26 } });
        waterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 28 } });
        List<DictionaryStringToInt> woodResourceProduction = new List<DictionaryStringToInt>();
        woodResourceProduction.Add(new DictionaryStringToInt());
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 6 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 16 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 26 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 36 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 46 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 56 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 66 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 76 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 86 } });
        woodResourceProduction.Add(new DictionaryStringToInt() { { "Wood", 96 } });

        List<DictionaryStringToInt> warehouseProduction = new List<DictionaryStringToInt>();
        residentResourceProduction.Add(new DictionaryStringToInt());
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 4000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 8000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 16000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 32000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 64000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 128000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 256000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 512000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 1024000 } });
        residentResourceProduction.Add(new DictionaryStringToInt() { { "Production", 2048000 } });

        string description = "NO !!!";
        List<Building> bu = new List<Building>();
        bu.Add(new Building(Building.BuildingType.Farm, resourceBuildingCost, farmResourceProduction, allConsuming, resourceUpgradePoint, 3, 10, maxCharacterInWorking, description, GetSpritePath("Farm"), farm_waterProductionStored, "Sprites/UI/FoodIcon"));
        bu.Add(new Building(Building.BuildingType.Fishery, craftBuildingCost, allProduction, allConsuming, craftUpgradePoint, 1, 3, maxCharacterInCraft, description, GetSpritePath("FishingPond")));
        bu.Add(new Building(Building.BuildingType.Kitchen, craftBuildingCost, laborProduction, allConsuming, craftUpgradePoint, 1, 3, maxCharacterInCraft, description, GetSpritePath("Kitchen")));
        bu.Add(new Building(Building.BuildingType.Laboratory, labBuildingCost, allProduction, allConsuming, labUpgradePoint, 1, 10, maxCharacterInWorking, description, GetSpritePath("Laboratory")));
        bu.Add(new Building(Building.BuildingType.LaborCenter, labBuildingCost, laborProduction, allConsuming, labUpgradePoint, 1, 10, maxCharacterInTeam, description, GetSpritePath("LaborCenter")));//***
        bu.Add(new Building(Building.BuildingType.MedicalCenter, craftBuildingCost, laborProduction, allConsuming, craftUpgradePoint, 1, 3, maxCharacterInCraft, description, GetSpritePath("MedicalCenter")));
        bu.Add(new Building(Building.BuildingType.QuarantineSite, QuarantineSiteCost, allProduction, allConsuming, QuarantineUpgradePoint, 1, 5, maxCharacterInQZone, description, GetSpritePath("QuarantineSite")));
        bu.Add(new Building(Building.BuildingType.Residence, containBuildingCost, residentResourceProduction, allConsuming, containUpgradePoint, 5, 10, maxCharacterInResident, description, GetSpritePath("Residence")));
        bu.Add(new Building(Building.BuildingType.TownBase, townBaseCost, allProduction, allConsuming, townBaseUpgradePoint, 1, 10, maxCharacterInTeam, description, GetSpritePath("TownBase")));//****
        bu.Add(new Building(Building.BuildingType.WareHouse, containBuildingCost, warehouseProduction, allConsuming, containUpgradePoint, 3, 10, maxCharacter, description, GetSpritePath("WareHouse")));
        bu.Add(new Building(Building.BuildingType.WaterTreatmentCenter, resourceBuildingCost, waterResourceProduction, allConsuming, resourceUpgradePoint, 3, 10, maxCharacterInWorking, description, GetSpritePath("WaterTreatmentCenter"), farm_waterProductionStored, "Sprites/UI/WaterIcon"));
        bu.Add(new Building(Building.BuildingType.Armory, craftBuildingCost, laborProduction, allConsuming, craftUpgradePoint, 1, 3, maxCharacterInCraft, description, GetSpritePath("Armory")));
        bu.Add(new Building(Building.BuildingType.TradingCenter, resourceBuildingCost, allProduction, allConsuming, townBaseUpgradePoint, 1, 10, maxCharacter, description, GetSpritePath("TradingCenter")));
        bu.Add(new Building(Building.BuildingType.Mine, resourceBuildingCost, mineResourceProduction, allConsuming, resourceUpgradePoint, 3, 3, maxCharacterInWorking, description, GetSpritePath("Mine"), goldProductionStored, "Sprites/UI/FoodIcon"));
        bu.Add(new Building(Building.BuildingType.LumberYard, resourceBuildingCost, woodResourceProduction, allConsuming, resourceUpgradePoint, 3, 10, maxCharacterInWorking, description, GetSpritePath("LumberYard"), woodProductionStored, "Sprites/UI/FoodIcon"));
        string buildingDatas = JsonHelper.ToJson(bu.ToArray(), true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Creating JSON data : " + buildingDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/BuildingData.json", buildingDatas);
    }

    void CreateQuestJsonData()
    {
        QuestData[] q = new QuestData[4];
        q[0] = new QuestData();
        q[0].questID = 1;
        q[0].questName = "Area1-1Normal";
        q[0].requireStats = new Character.AllStats { immunity = 4, strength = 5 };
        q[0].dropResourceName = new List<string>() { "Wood" };
        q[0].enemiesIDList = new List<int>() { 1 };
        q[0].duration = 30;

        q[1] = new QuestData();
        q[1].questID = 2;
        q[1].questName = "Area1-2Normal";
        q[1].requireStats = new Character.AllStats { immunity = 4, strength = 5 };
        q[1].dropResourceName = new List<string>() { "Wood" };
        q[1].enemiesIDList = new List<int>() { 1 };
        q[1].duration = 60;

        q[2] = new QuestData();
        q[2].questID = 3;
        q[2].questName = "Area1-3Normal";
        q[2].requireStats = new Character.AllStats { immunity = 4, strength = 5 };
        q[2].dropResourceName = new List<string>() { "Wood" };
        q[2].enemiesIDList = new List<int>() { 1, 2 };
        q[2].duration = 90;

        q[3] = new QuestData();
        q[3].questID = 4;
        q[3].questName = "Area1-1Hard";
        q[3].requireStats = new Character.AllStats { immunity = 4, strength = 5 };
        q[3].dropResourceName = new List<string>() { "Wood" };
        q[3].enemiesIDList = new List<int>() { 1, 2, 3 };
        q[3].duration = 120;

        string questDatas = JsonHelper.ToJson(q, true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Saving Data to JSON : " + questDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/QuestData.json", questDatas);
    }

    void CreateResourceData()
    {
        List<Resource> r = new List<Resource>();


        r.Add(new Resource(r.Count, "Wood", Item.RarityTier.Common, "Just a certain Wood.", Resource.ResourceType.Material, "Sprites/Resource/Wood"));
        r.Add(new Resource(r.Count, "Stone", Item.RarityTier.Common, "Just a certain Stone.", Resource.ResourceType.Material, "Sprites/Resource/Stone"));
        r.Add(new Resource(r.Count, "Water", Item.RarityTier.Common, "No water No life.", Resource.ResourceType.Material, "Sprites/Resource/Water"));
        r.Add(new Resource(r.Count, "Food", Item.RarityTier.Common, "Wanna starve to death?", Resource.ResourceType.Material, "Sprites/Resource/Food"));
        r.Add(new Resource(r.Count, "Fabric", Item.RarityTier.Common, "I'm so cold.", Resource.ResourceType.Material, "Sprites/Resource/Fabric"));
        r.Add(new Resource(r.Count, "Rubber", Item.RarityTier.Common, "Useless? who knows.", Resource.ResourceType.Material, "Sprites/Resource/Rubber"));
        r.Add(new Resource(r.Count, "Steel", Item.RarityTier.Uncommon, "Just a certain Wood.", Resource.ResourceType.Material, "Sprites/Resource/Steel"));
        r.Add(new Resource(r.Count, "Meteorite", Item.RarityTier.UltraRare, "BOOOOOM!!!", Resource.ResourceType.Material, "Sprites/Resource/Meteorite"));

        r.Add(new Resource(r.Count, "Wheat", Item.RarityTier.Common, "CoCoCrunch !?", Resource.ResourceType.Ingredient, "Sprites/Resource/Wheat"));
        r.Add(new Resource(r.Count, "Bread", Item.RarityTier.Common, "Low-Grade Bread.", Resource.ResourceType.Ingredient, "Sprites/Resource/Bread"));
        r.Add(new Resource(r.Count, "Meat", Item.RarityTier.Common, "YumYum . . .", Resource.ResourceType.Ingredient, "Sprites/Resource/Meat"));
        r.Add(new Resource(r.Count, "Burger", Item.RarityTier.Common, "American SPIRIT.", Resource.ResourceType.Consumable, "Sprites/Resource/Burger",
            new Resource.Effect() { name = "Burger Power", spritePath = "Sprites/Resource/Burger", stats = new Character.AllStats() { strength = 20 }, duration = 300 }));
        r.Add(new Resource(r.Count, "Golden Burger", Item.RarityTier.Uncommon, "GOLDEN American SPIRIT.", Resource.ResourceType.Consumable, "Sprites/Resource/Golden Burger",
            new Resource.Effect() { name = "Golden Burger Power", spritePath = "Sprites/Resource/GoldenBurger", stats = new Character.AllStats() { strength = 50, speed = 50, perception = 50 }, duration = 3600 }));

        r.Add(new Resource(r.Count, "Common Face Mask", Item.RarityTier.Common, "COUGH COUGH . . .", Resource.ResourceType.Gadget, "Sprites/Resource/Common Face Mask"));
        r.Add(new Resource(r.Count, "Ultra Instinct Face Mask", Item.RarityTier.UltraRare, "COUGH!! COUGH!! It's over 9000 !!!?? ", Resource.ResourceType.Gadget, "Sprites/Resource/Ultra Instinct Face Mask"));
        r.Add(new Resource(r.Count, "Medicine(maybe)", Item.RarityTier.Uncommon, "Everyone love this.", Resource.ResourceType.Medicine, "Sprites/Resource/Medicine(maybe)"));

        r.Add(new Resource(r.Count, "Leaf(maybe)", Item.RarityTier.Uncommon, "A certain leaf.", Resource.ResourceType.Ingredient, "Sprites/Resource/Leaf(maybe)"));




        r.Add(new Resource(r.Count, "Production", Item.RarityTier.Unknown, "Specific Building Production .", Resource.ResourceType.Special, "Sprites/Resource/Production"));
        r.Add(new Resource(r.Count, "Gold", Item.RarityTier.Unknown, "Specific Building Production .", Resource.ResourceType.Currency, "Sprites/Resource/Gold"));
        r.Add(new Resource(r.Count, "Diamond", Item.RarityTier.Unknown, "Specific Building Production .", Resource.ResourceType.Currency, "Sprites/Resource/Diamond"));

        r.Add(new Resource(r.Count, "Recipe:Bread", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.ConsumableRecipe, "Sprites/Resource/Recipe",
            new Item.CraftingData(new DictionaryStringToInt() { { "Wheat", 3 } }, 200)));

        r.Add(new Resource(r.Count, "Recipe:Burger", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.ConsumableRecipe, "Sprites/Resource/Recipe",
            new Item.CraftingData(new DictionaryStringToInt() { { "Bread", 2 }, { "Meat", 1 } }, 400)));

        r.Add(new Resource(r.Count, "Recipe:Golden Burger", Item.RarityTier.Uncommon, "Recipe.", Resource.ResourceType.ConsumableRecipe, "Sprites/Resource/Recipe",
            new Item.CraftingData(new DictionaryStringToInt() { { "Burger", 1 }, { "Gold", 9 } }, 1600)));

        r.Add(new Resource(r.Count, "Recipe:Ultra Instinct Face Mask", Item.RarityTier.UltraRare, "Recipe.", Resource.ResourceType.GadgetRecipe, "Sprites/Resource/Recipe",
            new Item.CraftingData(new DictionaryStringToInt() { { "Common Face Mask", 5 }, { "Fabric", 5 }, { "Rubber", 5 } }, 50000)));

        r.Add(new Resource(r.Count, "Recipe:Common Face Mask", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.GadgetRecipe, "Sprites/Resource/Recipe",
           new Item.CraftingData(new DictionaryStringToInt() { { "Fabric", 3 }, { "Rubber", 4 } }, 1000)));


        r.Add(new Resource(r.Count, "Recipe:Medicine(maybe)", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.MedicineRecipe, "Sprites/Resource/Recipe",
           new Item.CraftingData(new DictionaryStringToInt() { { "Leaf(maybe)", 4 } }, 1000)));




        /*  byte[] bytes = System.Text.Encoding.UTF8.GetBytes(resourceDatas);
        StringBuilder stringBuilder = new StringBuilder();
        bytes.ToList().ForEach(b => { stringBuilder.Append(b); });
        Debug.Log(stringBuilder.ToString());
        System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/ResourceData.byte", bytes);*/
        string resourceDatas = JsonHelper.ToJson(r.ToArray(), true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Creating JSON data : " + resourceDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/ResourceData.json", resourceDatas);

    }

    void CreateEnemyData()
    {
        List<Enemy> e = new List<Enemy>();

        e.Add(new Enemy(1, "Wolf Alpha", new Enemy.AllStats { attack = 5, defense = 3, hitPoint = 8, speed = 7, intelligence = 3 }));
        e.Add(new Enemy(2, "Wolf Beta", new Enemy.AllStats { attack = 2, defense = 7, hitPoint = 13, speed = 2, intelligence = 6 }));
        e.Add(new Enemy(3, "The Guardian", new Enemy.AllStats { attack = 0, defense = 20, hitPoint = 30, speed = 0, intelligence = 6 }));
        e.Add(new Enemy(4, "Something that already died", new Enemy.AllStats { attack = 1000, defense = 0, hitPoint = -1, speed = 0, intelligence = 6 }));
        e.Add(new Enemy(5, "Hatsune Miku (GOD form)", new Enemy.AllStats { attack = 99, defense = 99, hitPoint = 99, speed = 99, intelligence = 99 }));

        string enemyDatas = JsonHelper.ToJson(e.ToArray(), true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Saving Data to JSON : " + enemyDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/EnemyData.json", enemyDatas);
    }

    void CreateBirthMarkData()
    {
        List<BirthMarkData> birthMarkDatas = new List<BirthMarkData>();
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "StrengthBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark1",
            statToIncrease = typeof(Character.AllStats).GetField("strength"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "IntelligenceBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark2",
            statToIncrease = typeof(Character.AllStats).GetField("intelligence"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "CraftsmanshipBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark3",
            statToIncrease = typeof(Character.AllStats).GetField("craftsmanship"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
<<<<<<< HEAD
            name = "LuckBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark4",
            statToIncrease = typeof(Character.AllStats).GetField("luck"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "ImmunityBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark5",
            statToIncrease = typeof(Character.AllStats).GetField("immunity"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "PerceptionBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark6",
            statToIncrease = typeof(Character.AllStats).GetField("perception"),
            tier = 1
        });
        birthMarkDatas.Add(new IncreaseSTATSBirthMark()
        {
            name = "SpeedBM",
            effectValues = new List<float>() { 5, 7.5f, 10, 15, 20 },
            isAlliesAffected = false,
            spritePath = "Sprites/Character/BirthMarks/BirthMark7",
            statToIncrease = typeof(Character.AllStats).GetField("speed"),
            tier = 1
        });

        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Son of Farmer",
            effectValues = new List<float>() { 10, 15, 20, 25, 30 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark8",
            tier = 1,
            buildingType = Building.BuildingType.Farm
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Son of WaterFilter",
            effectValues = new List<float>() { 10, 15, 20, 25, 30 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark9",
            tier = 1,
            buildingType = Building.BuildingType.WaterTreatmentCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Son of GoldDigger",
            effectValues = new List<float>() { 10, 15, 20, 25, 30 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark10",
            tier = 1,
            buildingType = Building.BuildingType.Mine
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Farmer Master",
            effectValues = new List<float>() { 5, 7.5f, 10, 12.5f, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark11",
            tier = 2,
            buildingType = Building.BuildingType.Farm
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Water Filter Master",
            effectValues = new List<float>() { 5, 7.5f, 10, 12.5f, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark12",
            tier = 2,
            buildingType = Building.BuildingType.WaterTreatmentCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Gold Digger Master",
            effectValues = new List<float>() { 5, 7.5f, 10, 12.5f, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark13",
            tier = 2,
            buildingType = Building.BuildingType.Mine
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Son of Nurse",
            effectValues = new List<float>() { 10, 15, 20, 25, 30 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark14",
            tier = 2,
            buildingType = Building.BuildingType.Residence
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Monkey King",
            effectValues = new List<float>() { 5, 7.5f, 10, 12.5f, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark15",
            tier = 2,
            buildingType = Building.BuildingType.WaterTreatmentCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Economic Man",
            effectValues = new List<float>() { 4, 8, 12, 16, 20 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark16",
            tier = 2,
            buildingType = Building.BuildingType.LaborCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Cupid",
            effectValues = new List<float>() { 10, 25, 40, 55, 70 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark17",
            tier = 2,
            buildingType = Building.BuildingType.LaborCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Jack of All Trade",
            effectValues = new List<float>() { 10, 15, 20, 25, 30 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark18",
            tier = 3,
=======
            List<BirthMarkData> birthMarkDatas = new List<BirthMarkData>();
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "StrengthBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark1",
                statToIncrease = typeof(Character.AllStats).GetField("strength"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "IntelligenceBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark2",
                statToIncrease = typeof(Character.AllStats).GetField("intelligence"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "CraftsmanshipBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark3",
                statToIncrease = typeof(Character.AllStats).GetField("craftsmanship"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "LuckBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark4",
                statToIncrease = typeof(Character.AllStats).GetField("luck"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "ImmunityBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark5",
                statToIncrease = typeof(Character.AllStats).GetField("immunity"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "PerceptionBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark6",
                statToIncrease = typeof(Character.AllStats).GetField("perception"),
                tier = 1
            });
            birthMarkDatas.Add(new IncreaseSTATSBirthMark()
            {
                name = "SpeedBM",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.15f, 0.20f },
                isAlliesAffected = false,
                spritePath = "Sprites/Character/BirthMarks/BirthMark7",
                statToIncrease = typeof(Character.AllStats).GetField("speed"),
                tier = 1
            });

            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Son of Farmer",
                effectValues = new List<float>() { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark8",
                tier = 1,
                buildingType = Building.BuildingType.Farm
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Son of WaterFilter",
                effectValues = new List<float>() { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark9",
                tier = 1,
                buildingType = Building.BuildingType.WaterTreatmentCenter
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Son of GoldDigger",
                effectValues = new List<float>() { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark10",
                tier = 1,
                buildingType = Building.BuildingType.Mine
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Farmer Master",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.125f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark11",
                tier = 2,
                buildingType = Building.BuildingType.Farm
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Water Filter Master",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.125f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark12",
                tier = 2,
                buildingType = Building.BuildingType.WaterTreatmentCenter
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Gold Digger Master",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.125f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark13",
                tier = 2,
                buildingType = Building.BuildingType.Mine
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Son of Nurse",
                effectValues = new List<float>() { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark14",
                tier = 2,
                buildingType = Building.BuildingType.Residence
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Monkey King",
                effectValues = new List<float>() { 0.05f, 0.075f, 0.10f, 0.125f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark15",
                tier = 2,
                buildingType = Building.BuildingType.TownBase
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Economic Man",
                effectValues = new List<float>() { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark16",
                tier = 2,
                buildingType = Building.BuildingType.LaborCenter
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Cupid",
                effectValues = new List<float>() { 0.10f, 0.25f, 0.40f, 0.55f, 0.70f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark17",
                tier = 2,
                buildingType = Building.BuildingType.LaborCenter
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Jack of All Trade",
                effectValues = new List<float>() { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark18",
                tier = 3,

            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Nerd",
                effectValues = new List<float>() { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark19",
                tier = 3,
                buildingType = Building.BuildingType.Laboratory
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Iron Chef",
                effectValues = new List<float>() { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark20",
                tier = 3,
                buildingType = Building.BuildingType.Kitchen
            });
            birthMarkDatas.Add(new DoubleProductBirthMark()
            {
                name = "MultiCooker",
                effectValues = new List<float>() { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark21",
                tier = 4,
                buildingType = Building.BuildingType.Kitchen
            });
            birthMarkDatas.Add(new DoubleProductBirthMark()
            {
                name = "MultiSmith",
                effectValues = new List<float>() { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark22",
                tier = 4,
                buildingType = Building.BuildingType.Armory
            });
            birthMarkDatas.Add(new DoubleProductBirthMark()
            {
                name = "MultiFisher",
                effectValues = new List<float>() { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark23",
                tier = 4,
                buildingType = Building.BuildingType.Fishery
            });
            birthMarkDatas.Add(new DoubleProductBirthMark()
            {
                name = "MultiAlcemist",
                effectValues = new List<float>() { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f },
                spritePath = "Sprites/Character/BirthMarks/BirthMark24",
                tier = 4,
                buildingType = Building.BuildingType.MedicalCenter
            });
            birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
            {
                name = "Luck of The Sea",
                effectValues = new List<float>() { 0.80f, 0.90f, 1 },
                spritePath = "Sprites/Character/BirthMarks/BirthMark25",
                tier = 5,
                buildingType = Building.BuildingType.Fishery
            });
            birthMarkDatas.Add(new AddMoreActionBirthMark()
            {
                name = "777",
                effectValues = new List<float>() { 1, 1, 2 },
                spritePath = "Sprites/Character/BirthMarks/BirthMark26",
                tier = 5,

            });

            BirthMarkSerializer birthMarkSerializer = new BirthMarkSerializer();
            birthMarkSerializer.birthMarkDatas = birthMarkDatas;
            string birthMarkDatass = JsonUtility.ToJson(birthMarkSerializer, true);
            Debug.Log("Saving BirthMarkDatas Data to JSON : " + birthMarkDatass);
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/BirthMarkData.json", birthMarkDatass);
>>>>>>> parent of 23f75b0... Commit

        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Nerd",
            effectValues = new List<float>() { 3, 6, 9, 12, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark19",
            tier = 3,
            buildingType = Building.BuildingType.Laboratory
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Iron Chef",
            effectValues = new List<float>() { 3, 6, 9, 12, 15 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark20",
            tier = 3,
            buildingType = Building.BuildingType.Kitchen
        });
        birthMarkDatas.Add(new DoubleProductBirthMark()
        {
            name = "MultiCooker",
            effectValues = new List<float>() { 4, 8, 12, 16, 20 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark21",
            tier = 4,
            buildingType = Building.BuildingType.Kitchen
        });
        birthMarkDatas.Add(new DoubleProductBirthMark()
        {
            name = "MultiSmith",
            effectValues = new List<float>() { 4, 8, 12, 16, 20 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark22",
            tier = 4,
            buildingType = Building.BuildingType.Armory
        });
        birthMarkDatas.Add(new DoubleProductBirthMark()
        {
            name = "MultiFisher",
            effectValues = new List<float>() { 4, 8, 12, 16, 20 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark23",
            tier = 4,
            buildingType = Building.BuildingType.Fishery
        });
        birthMarkDatas.Add(new DoubleProductBirthMark()
        {
            name = "MultiAlcemist",
            effectValues = new List<float>() { 4, 8, 12, 16, 20 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark24",
            tier = 4,
            buildingType = Building.BuildingType.MedicalCenter
        });
        birthMarkDatas.Add(new ParticularEffectOnBuildingBirthMark()
        {
            name = "Luck of The Sea",
            effectValues = new List<float>() { 80, 90, 100 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark25",
            tier = 5,
            buildingType = Building.BuildingType.Fishery
        });
        birthMarkDatas.Add(new AddMoreActionBirthMark()
        {
            name = "777",
            effectValues = new List<float>() { 1, 1, 2 },
            spritePath = "Sprites/Character/BirthMarks/BirthMark26",
            tier = 5,

<<<<<<< HEAD
        });
=======
            string achievementsData = JsonHelper.ToJson<AchievementData>(achievements.ToArray(), true);
            Debug.Log("Saving achievementsData Data to JSON : " + achievementsData);
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/achievements.json", achievementsData);
<<<<<<< HEAD
>>>>>>> parent of 23f75b0... Commit

        BirthMarkSerializer birthMarkSerializer = new BirthMarkSerializer();
        birthMarkSerializer.birthMarkDatas = birthMarkDatas;
        string birthMarkDatass = JsonUtility.ToJson(birthMarkSerializer, true);
        Debug.Log("Saving BirthMarkDatas Data to JSON : " + birthMarkDatass);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/BirthMarkData.json", birthMarkDatass);

    }
=======
>>>>>>> parent of 23f75b0... Commit

    void CreateAchievementData()
    {
        List<AchievementData> achievements = new List<AchievementData>();
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 1000 Food.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 1000 Wood.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 1000 Stone.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "There are 5 peoples.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "Player lavel up to 5.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });
        achievements.Add(new AchievementData() { id = achievements.Count + 1 + Constant.IDMask.ACHIEVEMENT_ID_MASK, name = "More than 100 Gold.", rewards = new DictionaryStringToInt() { { "Stone", 1 }, { "Wood", 5 } }, condition = () => { return ItemManager.Instance.GetResourceAmount("Gold") > 100; } });



        Text text;
        //text.text = achievements[0].name;
        

        if(achievements[0].condition())
        {

        }

        string achievementsData = JsonHelper.ToJson<AchievementData>(achievements.ToArray(), true);
        Debug.Log("Saving achievementsData Data to JSON : " + achievementsData);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/achievements.json", achievementsData);

    }
}
