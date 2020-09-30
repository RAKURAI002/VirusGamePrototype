﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;
public class ItemManager : SingletonComponent<ItemManager>
{
    /// Contains all of Player's Resources.
    private ResourceDictionary allResources;

    /// Contains all of Player's Equipments.
    private EquipmentDictionary allEquipments;

    float RESOURCE_CICLE_TIME = Constant.TimeCycle.RESOURCE_UPDATE_CYCLE;

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
    }
    #endregion

    public int GetResourceAmount(string name)
    {
        return allResources.ContainsKey(name) ? Mathf.FloorToInt(allResources[name].Amount) : 0;
    }

    public void OnClickCollectResource()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        string resourceName = buttonName.Replace("CollectButton", "");

        Building.BuildingType type;
        switch (resourceName)
        {
            case "Wood":
                {
                    type = Building.BuildingType.LumberYard;
                    break;
                }
            case "Stone":
                {
                    type = Building.BuildingType.Mine;
                    break;
                }
            case "Gold":
                {
                    type = Building.BuildingType.Mine;
                    break;
                }
            case "Water":
                {
                    type = Building.BuildingType.WaterTreatmentCenter;
                    break;
                }
            case "Food":
                {
                    type = Building.BuildingType.Farm;
                    break;
                }
            default:
                {
                    type = Building.BuildingType.Unknown;
                    break;
                }
        }

        List<Builder> builders = BuildingManager.Instance.AllBuildings.Where(b => b.Type == type).ToList();

        if(builders != null)
        {
            Building buildingData = LoadManager.Instance.allBuildingData[type];
            int amount = 0;
            foreach (var builder in builders)
            {
                int amountTemp = Mathf.FloorToInt(builder.currentProductionAmount);
                amount += amountTemp;
                builder.currentProductionAmount -= amountTemp;

            }

            ItemManager.Instance.AddResource(buildingData.production[1].First().Key, amount);
        }

    }

    public void AddEquipment(int id, int amount)
    {
        var equipment = LoadManager.Instance.allEquipmentData.SingleOrDefault(e => e.Value.ID == id);
        if (equipment.Equals(default(KeyValuePair<string, Equipment>)))
        {
            Debug.LogWarning($"There're no {id} ID in Equipment Data.");
            return;
        }

        AddEquipment(equipment.Key, amount);
        return;
    }

    public void AddEquipment(string name, int amount)
    {
        if (!allEquipments.ContainsKey(name))
        {
            Debug.Log($"No current Equipment data in player. Trying to Create new one . . .");
            if (LoadManager.Instance.allEquipmentData.ContainsKey(name))
            {

                allEquipments.Add(name, new Equipment(LoadManager.Instance.allEquipmentData[name]));
            }
            else
            {
                Debug.LogWarning($"There're no {name} Equipment data in game.");
                return;
            }
        }

        allEquipments[name].AllAmount += amount;
        Debug.Log($"Adding {allEquipments[name].Name} : {amount} unit(s) to Player. Now Player have {allEquipments[name].AllAmount}.");
        LoadManager.Instance.SavePlayerDataToFireBase();
        return;

    }

    public void AddResource(int id, int amount)
    {
        var resource = LoadManager.Instance.allResourceData.SingleOrDefault(r => r.Value.ID == id);
        if (resource.Equals(default(KeyValuePair<string, Resource>)))
        {
            Debug.LogWarning($"There're no {id} ID in Resource Data.");
            return;
        }

        AddEquipment(resource.Key, amount);
        return;

    }
    public bool AddResource(string name, int amount)
    {
        if (!LoadManager.Instance.allResourceData.ContainsKey(name))
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
        LoadManager.Instance.SavePlayerDataToFireBase();

        return true;
    }

    public bool TryConsumeResources(DictionaryStringToInt resources)
    {
        if (IsAffordable(resources))
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
        if (ConsumeResource(resource.Key, resource.Value))
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
        if(amount == 0)
        {
            return true;
        }

        if (!allResources.ContainsKey(name))
        {
            Debug.LogWarning($"There're NO {name} in Player Inventory");
            return false;
        }
        else if (allResources[name].Amount < amount)
        {
            Debug.LogWarning($"There're ONLY {allResources[name]} {name} in Player Inventory");
            return false;
        }
        else
        {
            allResources[name].Amount -= amount;
            EventManager.Instance.ResourceChanged(name);
            LoadManager.Instance.SavePlayerDataToFireBase();
        }

        return true;
    }

    public bool IsAffordable(DictionaryStringToInt cost)
    {
        foreach (KeyValuePair<string, int> resource in cost)
        {
            if (resource.Value > (ItemManager.Instance.AllResources.ContainsKey(resource.Key) ? ItemManager.Instance.AllResources[resource.Key].Amount : 0))
            {
                //Debug.Log($"Not enough Resource ({LoadManager.Instance.allResourceData[resource.Key].Name})");
                return false;
            }
        }
        return true;
    }
    public bool IsAffordable(string name, int amount)
    {
        if (amount > GetResourceAmount(name))
        {
            // Debug.Log($"Not enough Resource ({LoadManager.Instance.allResourceData[name].Name})");
            return false;
        }

        return true;
    }

    public int GetSpeedUpCost(float pointLeft)
    {
        int gemCost = Mathf.RoundToInt(pointLeft / 200);
        return gemCost > 2 ? gemCost : 0;
    }

    public void AddTest()
    {
        AddResource("Stone", 1000);
        AddResource("Wood", 1000);
        AddResource("Gold", 100);
        AddResource("Diamond", 100);
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

    }

}
