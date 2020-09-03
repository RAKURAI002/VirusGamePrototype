using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCreator : MonoBehaviour
{
    void Start()
    {
        CreateBuildingJsonData();
        // CreateEnemyData();
        CreateQuestJsonData();
        CreateResourceData();
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
        equipment.Add(new Equipment(1, "Item1", Item.RarityTier.Uncommon, "Too mighty blade.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item1"), new Character.AllStats { strength = 8, speed = -1} ));
        equipment.Add(new Equipment(2, "Item2", Item.RarityTier.UltraRare, "Seems like a MONKEY's heart ?", Equipment.EquipmentPosition.Body, ("Sprites/Equipments/Item2"), new Character.AllStats { strength = 20, speed = 10 , immunity = 20}));
        equipment.Add(new Equipment(3, "Item3", Item.RarityTier.Uncommon, "What is this ?", Equipment.EquipmentPosition.Face, ("Sprites/Equipments/Item3"), new Character.AllStats { strength = 1, perception = 3 }));
        equipment.Add(new Equipment(4, "Item4", Item.RarityTier.Rare, "Hmmm . . .", Equipment.EquipmentPosition.Leg, ("Sprites/Equipments/Item4"), new Character.AllStats { speed = 4, intelligence =  4 }));
        equipment.Add(new Equipment(5, "Item5", Item.RarityTier.Rare, "Unleash your TRUE power.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item5"), new Character.AllStats { strength = 3, intelligence = 3, luck = 3, perception = 3, immunity = 3, craftsmanship = 3, speed = 3 }));
        equipment.Add(new Equipment(6, "Item6", Item.RarityTier.Uncommon, "An ancient Sorcerer's boots.", Equipment.EquipmentPosition.Foot, ("Sprites/Equipments/Item6"), new Character.AllStats { strength = 2, intelligence = 5}));
        equipment.Add(new Equipment(7, "Item7", Item.RarityTier.Rare, "That seems heavy.", Equipment.EquipmentPosition.Body, ("Sprites/Equipments/Item7"), new Character.AllStats { strength = 12, speed = -3 }));
        equipment.Add(new Equipment(8, "Item8", Item.RarityTier.Uncommon, "Can cut everything except trees.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item8"), new Character.AllStats { strength = 6, speed = -1 }));
        equipment.Add(new Equipment(9, "Item9", Item.RarityTier.Uncommon, "Make you more tanky.", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item9"), new Character.AllStats { strength = 3 }));
        equipment.Add(new Equipment(10, "Item10", Item.RarityTier.Rare, "Smell bloody . . .", Equipment.EquipmentPosition.Hand, ("Sprites/Equipments/Item10"), new Character.AllStats { luck = -1, strength = 6 , intelligence = 1}));
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
        List<Building> bu = new List<Building>();
        //  bu.Add(new Building(Building.BuildingType.Farm, ));

        List<DictionaryStringToInt> allCost = new List<DictionaryStringToInt>();
        allCost.Add(new DictionaryStringToInt() { { "Stone", 2 }, { "Wood", 2 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 2 }, { "Wood", 3 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 3 }, { "Wood", 3 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 3 }, { "Wood", 4 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 4 }, { "Wood", 4 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 4 }, { "Wood", 5 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 5 }, { "Wood", 5 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 5 }, { "Wood", 6 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 6 }, { "Wood", 6 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 6 }, { "Wood", 7 } });
        allCost.Add(new DictionaryStringToInt() { { "Stone", 6 }, { "Wood", 7 } });

        List<DictionaryStringToInt> allProduction = new List<DictionaryStringToInt>();
        allProduction.Add(new DictionaryStringToInt());
        allProduction.Add(new DictionaryStringToInt() { { "Food", 2 }});
        allProduction.Add(new DictionaryStringToInt() { { "Food", 3 }});
        allProduction.Add(new DictionaryStringToInt() { { "Food", 3 }});
        allProduction.Add(new DictionaryStringToInt() { { "Food", 4 }, { "Water", 4 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 4 }, { "Water", 5 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 5 }, { "Water", 5 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 5 }, { "Water", 6 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 6 }, { "Water", 6 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 6 }, { "Water", 7 }, { "Production", 30 } });
        allProduction.Add(new DictionaryStringToInt() { { "Food", 6 }, { "Water", 7 }, { "Production", 30 } });

        List<DictionaryStringToInt> allConsuming = new List<DictionaryStringToInt>();
       

        List<int> upgradePoint = new List<int>(){200,
                                            300,
                                            300,
                                            300,
                                            300,
                                            300,
                                            600,
                                            600,
                                            600,
                                            600,
                                            600 };



        CharacterAmountDictionary maxCharacter = new CharacterAmountDictionary();
        maxCharacter.Add(0, new MaxCharacterStored { amount = new List<int>() { 0 } });
        maxCharacter.Add(1, new MaxCharacterStored { amount = new List<int>() { 1 } });
        maxCharacter.Add(2, new MaxCharacterStored { amount = new List<int>() { 2 } });
        maxCharacter.Add(3, new MaxCharacterStored { amount = new List<int>() { 3 } });
        maxCharacter.Add(4, new MaxCharacterStored { amount = new List<int>() { 3, 1 } });
        maxCharacter.Add(5, new MaxCharacterStored { amount = new List<int>() { 3, 2 } });
        maxCharacter.Add(6, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });
        maxCharacter.Add(7, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });
        maxCharacter.Add(8, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });
        maxCharacter.Add(9, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });
        maxCharacter.Add(10, new MaxCharacterStored { amount = new List<int>() { 3, 3 } });

        List<int> productionStored = new List<int>(){0,
                                            100,
                                            300,
                                            500,
                                            700,
                                            900,
                                            1100,
                                            1300,
                                            1500,
                                            1700,
                                            1900 };


        List<DictionaryStringToInt> FarmResourceProduction = new List<DictionaryStringToInt>();
        FarmResourceProduction.Add(new DictionaryStringToInt());
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 1 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 2 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 3 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 4 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 5 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 6 }});
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 7 }, { "Production", 30 } });
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 8 }, { "Production", 30 } });
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 9 }, { "Production", 30 } });
        FarmResourceProduction.Add(new DictionaryStringToInt() { { "Food", 10 }, { "Production", 30 } });



        List<DictionaryStringToInt> WaterResourceProduction = new List<DictionaryStringToInt>();
        WaterResourceProduction.Add(new DictionaryStringToInt());
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 1 }, { "Production", 20 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 2 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 3 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 4 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 5 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 6 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 7 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 8 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 9 }, { "Production", 30 } });
        WaterResourceProduction.Add(new DictionaryStringToInt() { { "Water", 10 }, { "Production", 30 } });


        string description = "NO !!!";

        bu.Add(new Building(Building.BuildingType.Farm, allCost, FarmResourceProduction, allConsuming, upgradePoint, 2, 9, maxCharacter, description, GetSpritePath("Farm"), productionStored, "Sprites/UI/FoodIcon"));
        bu.Add(new Building(Building.BuildingType.FishingPond, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("FishingPond")));
        bu.Add(new Building(Building.BuildingType.Kitchen, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("Kitchen")));
        bu.Add(new Building(Building.BuildingType.Laboratory, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("Laboratory")));
        bu.Add(new Building(Building.BuildingType.LaborCenter, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("LaborCenter")));//***
        bu.Add(new Building(Building.BuildingType.MedicalCenter, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("MedicalCenter")));
        bu.Add(new Building(Building.BuildingType.QuarantineSite, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("QuarantineSite")));
        bu.Add(new Building(Building.BuildingType.Residence, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("Residence")));
        bu.Add(new Building(Building.BuildingType.TownBase, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("TownBase")));//****
        bu.Add(new Building(Building.BuildingType.WareHouse, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("WareHouse")));
        bu.Add(new Building(Building.BuildingType.WaterTreatmentCenter, allCost, WaterResourceProduction, allConsuming, upgradePoint, 2, 9, maxCharacter, description, GetSpritePath("WaterTreatmentCenter"), productionStored, "Sprites/UI/WaterIcon"));
        bu.Add(new Building(Building.BuildingType.Armory, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("Armory")));
        bu.Add(new Building(Building.BuildingType.TradingCenter, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("TradingCenter")));
        bu.Add(new Building(Building.BuildingType.Mine, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("Mine")));

        bu.Add(new Building(Building.BuildingType.LumberYard, allCost, allProduction, allConsuming, upgradePoint, 1, 9, maxCharacter, description, GetSpritePath("LumberYard")));

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
        q[1].dropResourceName = new List<string>() { "Wood"};
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
            new Resource.Effect() { name = "Burger Power", spritePath = "Sprites/Resource/Burger", stats = new Character.AllStats() { strength = 20 }, duration = 300})) ;
        r.Add(new Resource(r.Count, "Golden Burger", Item.RarityTier.Uncommon, "GOLDEN American SPIRIT.", Resource.ResourceType.Consumable, "Sprites/Resource/Golden Burger",
            new Resource.Effect() { name = "Golden Burger Power", spritePath = "Sprites/Resource/GoldenBurger", stats = new Character.AllStats() { strength = 50, speed = 50, attack = 50, defense = 50, perception = 50 }, duration = 3600 }));      
        
        r.Add(new Resource(r.Count, "Common Face Mask", Item.RarityTier.Common, "COUGH COUGH . . .", Resource.ResourceType.Gadget, "Sprites/Resource/Common Face Mask"));
        r.Add(new Resource(r.Count, "Ultra Instinct Face Mask", Item.RarityTier.UltraRare, "COUGH!! COUGH!! It's over 9000 !!!?? ", Resource.ResourceType.Gadget, "Sprites/Resource/Ultra Instinct Face Mask"));
        r.Add(new Resource(r.Count, "Medicine(maybe)", Item.RarityTier.Uncommon, "Everyone love this.", Resource.ResourceType.Medicine, "Sprites/Resource/Medicine(maybe)"));

        r.Add(new Resource(r.Count, "Leaf(maybe)", Item.RarityTier.Uncommon, "A certain leaf.", Resource.ResourceType.Ingredient, "Sprites/Resource/Leaf(maybe)"));




        r.Add(new Resource(r.Count, "Production", Item.RarityTier.Unknown, "Specific Building Production .", Resource.ResourceType.Special, "Sprites/Resource/Production"));
        r.Add(new Resource(r.Count, "Gold", Item.RarityTier.Unknown, "Specific Building Production .", Resource.ResourceType.Currency, "Sprites/Resource/Gold"));
        r.Add(new Resource(r.Count, "Diamond", Item.RarityTier.Unknown, "Specific Building Production .",  Resource.ResourceType.Currency, "Sprites/Resource/Diamond"));

        r.Add(new Resource(r.Count, "Recipe:Bread", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.ConsumableRecipe, "Sprites/Resource/Recipe", 
            new Item.CraftingData(new DictionaryStringToInt() { { "Wheat", 3 } }, 200)));

        r.Add(new Resource(r.Count, "Recipe:Burger", Item.RarityTier.Common, "Recipe.", Resource.ResourceType.ConsumableRecipe, "Sprites/Resource/Recipe",
            new Item.CraftingData(new DictionaryStringToInt() { { "Bread", 2 }, { "Meat", 1} }, 400)));

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

        e.Add(new Enemy(1, "Wolf Alpha", new Enemy.AllStats { attack = 5, defense = 3, hitPoint = 8, speed = 7, intelligence = 3}));
        e.Add(new Enemy(2, "Wolf Beta", new Enemy.AllStats { attack = 2, defense = 7, hitPoint = 13, speed = 2, intelligence = 6 }));
        e.Add(new Enemy(3, "The Guardian", new Enemy.AllStats { attack = 0, defense = 20, hitPoint = 30, speed = 0, intelligence = 6 }));
        e.Add(new Enemy(4, "Something that already died", new Enemy.AllStats { attack = 1000, defense = 0, hitPoint = -1, speed = 0, intelligence = 6 }));
        e.Add(new Enemy(5, "Hatsune Miku (GOD form)", new Enemy.AllStats { attack = 99, defense = 99, hitPoint = 99, speed = 99, intelligence = 99 }));

        string enemyDatas = JsonHelper.ToJson(e.ToArray(), true);// Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented); //JsonUtility.ToJson(playerData, true); 
        Debug.Log("Saving Data to JSON : " + enemyDatas);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/EnemyData.json", enemyDatas);
    }
}
