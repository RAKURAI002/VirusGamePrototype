﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using UnityEngine.SceneManagement;
public class ItemManager : SingletonComponent<ItemManager>
{
    /// Contains all of Player's Resources.
    private ResourceDictionary allResources;

    /// Contains all of Player's Equipments.
    private EquipmentDictionary allEquipments;

    /// 1 Resource update cycle's period.
    float RESOURCE_CICLE_TIME = 60f;

    public ResourceDictionary AllResources { get { return allResources; } set { allResources = value; } }
    public EquipmentDictionary AllEquipments { get { return allEquipments; } set { allEquipments = value; } }

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
       
    }
    protected override void OnInitialize()
    {
        allResources = new ResourceDictionary();
        allEquipments = new EquipmentDictionary();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
        }
        secondCalled = true;
    }
    void Start()
    {
       // InvokeRepeating("UpdateResourceEveryMinute", RESOURCE_CICLE_TIME, RESOURCE_CICLE_TIME);
    }
    #endregion

    public int GetResourceAmount(string name)
    {
        return allResources.ContainsKey(name) ? Mathf.FloorToInt(allResources[name].Amount) : 0;
    
    }
    /// <summary>
    /// Resource update cycle function. Called in Start.
    /// </summary>
    void UpdateResourceEveryMinute()
    {
        foreach(Builder builder in BuildManager.Instance.AllBuildings)
        {
            UpdateBuildingResource(builder);

        }

        CharacterResourceConsuming();
        return;

    }

    void CharacterResourceConsuming()
    {
        int decreaseAmount = CharacterManager.Instance.AllCharacters.Count;

        Debug.Log($"{RESOURCE_CICLE_TIME} seconds passed, Current character in possesion is {CharacterManager.Instance.AllCharacters.Count} " +
            $"resulting in LOSING Foods : {decreaseAmount} and Water {decreaseAmount}");

        ConsumeResource("Foods", decreaseAmount);
        ConsumeResource("Water", decreaseAmount);
        LoadManager.Instance.SavePlayerDataToJson();
        return;

    }

    void UpdateBuildingResource(Builder builder)
    {
        Building buildingData = LoadManager.Instance.allBuildingData[builder.Type];

        if (buildingData.production[builder.Level] == null)
        {
            Debug.LogWarning($"There're NO production available for {builder.Type} level {builder.Level}.");
            return;

        }

        foreach(KeyValuePair<string, int> baseProduction in buildingData.production[builder.Level])
        {
                int finalUpdatedAmount = baseProduction.Value;
                int characterStatsSum = 0;

                if(builder.CharacterInBuilding != null)
                {
                    characterStatsSum = builder.CharacterInBuilding.Sum(c => c.Characters.Sum( ch => ch.Stats.speed));
                    finalUpdatedAmount += characterStatsSum;
                }
                
                Debug.Log($"{RESOURCE_CICLE_TIME} seconds passed. Base Production of {builder.Type}[ID : {builder.ID}] is {baseProduction.Value} and sum of Character's Speed in building is {characterStatsSum} resulting in INCREASE " +
                    $"{LoadManager.Instance.allResourceData[baseProduction.Key].Name} : {finalUpdatedAmount}");
                AddResource(baseProduction.Key , finalUpdatedAmount);

        }
        return; 
        
    }

    public void AddEquipment(int id, int amount)
    {
        AddEquipment(LoadManager.Instance.allEquipmentData.SingleOrDefault( e => e.Value.ID == id).Key, amount);
        return;

    }
    public void AddEquipment(string name, int amount)
    {
        if (!allEquipments.ContainsKey(name))
        {
            try
            {
                allEquipments.Add(name, new Equipment(LoadManager.Instance.allEquipmentData[name]));
                Debug.Log($"No current Equipment data in player. Trying to Create new one . . .");

            }
            catch(KeyNotFoundException e)
            {
                Debug.LogError($"Can't find {name}'s data ::" + e.ToString());

            }

        }

        allEquipments[name].AllAmount += amount;
        Debug.Log($"Adding {allEquipments[name].Name} : {amount} unit(s) to Player. Now Player have {allEquipments[name].AllAmount}.");
        LoadManager.Instance.SavePlayerDataToJson();
        return;

    }
    public void AddResource(int id, int amount)
    {
        string name = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == id).Key;
        AddResource(name, amount);
        return;

    }
    public bool AddResource(string name, int amount)
    {
        if( !LoadManager.Instance.allResourceData.ContainsKey(name))
        {
            Debug.LogWarning($"There're no {name} data in game.");
            return false;

        }

        if (allResources.ContainsKey(name))
        {
            allResources[name].Amount += amount;

        }
        else
        {
            allResources.Add(name, new Resource(name, amount));

        }
        Debug.Log($"Adding {LoadManager.Instance.allResourceData[name].Name} : {amount} unit(s) to Player . . .");
        EventManager.Instance.ResourceChanged(name);
        LoadManager.Instance.SavePlayerDataToJson();

        return true;
    }

    public bool TryConsumeResources(DictionaryStringToInt resources)
    {
        if(IsAffordable(resources))
        {
            foreach (KeyValuePair<string, int> resource in resources)
            {
                ConsumeResource(resource);

            }

        }
        else
        {
            return false;

        }

        return true;  
    }
    public bool TryConsumeResources(string name, int amount)
    {
        if (IsAffordable(name, amount))
        {
                ConsumeResource(name, amount);

        }
        else
        {
            return false;

        }
        return true;
    }
    bool ConsumeResource(KeyValuePair<string, int> resource)
    {
        if(ConsumeResource(resource.Key, resource.Value))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    bool ConsumeResource(string name, int amount)
    {
        Debug.Log($"Consuming {name} : {amount} unit(s) from Player . . .");
        if (!allResources.ContainsKey(name))
        {
            Debug.LogError($"There're NO {name} in Player Inventory");
            return false;
        }
        else if(allResources[name].Amount < amount)
        {
            Debug.LogWarning($"There're ONLY {allResources[name]} {name} in Player Inventory");
            return false;
        }
        else
        {
            allResources[name].Amount -= amount;
            EventManager.Instance.ResourceChanged(name);
            LoadManager.Instance.SavePlayerDataToJson();
        }
        return true;

    }
    public bool IsAffordable(DictionaryStringToInt cost)
    {
        foreach (KeyValuePair<string, int> resource in cost)
        {
            if (resource.Value > (ItemManager.Instance.AllResources.ContainsKey(resource.Key) ? ItemManager.Instance.AllResources[resource.Key].Amount : 0))
            {
                Debug.Log($"Not enough Resource ({LoadManager.Instance.allResourceData[resource.Key].Name})");
                return false;
            }
        }
        return true;
    }
    public bool IsAffordable(string name, int amount)
    {
            if (amount > (ItemManager.Instance.AllResources.ContainsKey(name) ? ItemManager.Instance.AllResources[name].Amount : 0))
            {
                Debug.Log($"Not enough Resource ({LoadManager.Instance.allResourceData[name].Name})");
                return false;
            }
        
        return true;
    }
    public int GetSpeedUpCost(float pointLeft)
    {
        int gemCost = Mathf.RoundToInt(pointLeft / 200);
        return gemCost > 2 ? gemCost : 0;
    }
    /// Debug Functions.
    public void ShowResourceTest()
    {
        foreach(KeyValuePair<string, Resource> resource in allResources)
        {
            Debug.Log($"{LoadManager.Instance.allResourceData[resource.Key].Name } : {resource.Value}");
        }
        
    }
    public void AddTest()
    {
        AddResource(1, 10);
        AddResource(2, 10);
        AddResource(6, 10);
        AddResource(4, 10);
        AddResource(3, 10);
        AddResource(5, 10);
        AddResource("Stone", 10);
        AddResource("Wood", 10);
        AddResource("Gold", 10);
        AddResource("Diamond", 10);
        AddResource("Wheat", 10);
        AddResource("Meat", 10);
        AddResource("Burger", 5);
        AddResource("Golden Burger", 5);
        AddResource("Bread", 10);
        AddResource("Recipe:Bread", 10);
        AddResource("Recipe:Burger", 10);
        AddResource("Recipe:Golden Burger", 10);

        AddResource("Leaf(maybe)", 10);
        AddResource("Fabric", 10);
        AddResource("Rubber", 10);
        AddResource("Recipe:Common Face Mask", 10);
        AddResource("Recipe:Ultra Instinct Face Mask", 10);
        AddResource("Recipe:Medicine(maybe)", 10);


        AddEquipment(1, 1);
        AddEquipment(1, 1);
        AddEquipment(2, 1);
        AddEquipment(3, 1);

        AddEquipment(4, 1);
        AddEquipment(5, 1);
        AddEquipment(6, 1);
        AddEquipment(7, 1);
        AddEquipment(8, 1);
        AddEquipment(9, 1);
        AddEquipment(10, 1);
        AddEquipment(11, 1);

        AddEquipment(12, 1);
        AddEquipment(13, 1);
        AddEquipment(14, 1);
        AddEquipment(15, 1);
    }

}