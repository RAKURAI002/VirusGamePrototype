using System.Collections;
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

    /// 1 Resource update cycle's period.
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
        StringBuilder log = new StringBuilder();
        if (buildingData.production[builder.Level] == null)
        {
            Debug.LogWarning($"There're NO production available for {builder.Type} level {builder.Level}.");
            return;

        }

        if (builder.CharacterInBuilding.Count != 1)
        {
            Debug.LogWarning($"Only single team Building can access this function.");
            return;

        }

        List<Character> characters = builder.CharacterInBuilding[0].Characters;
        foreach (KeyValuePair<string, int> baseProduction in buildingData.production[builder.Level])
        {
            log.AppendLine($"{RESOURCE_CICLE_TIME} seconds passed. Base Production of {builder.Type}[ID : {builder.ID}] is {baseProduction.Value}");

            Resource resource = LoadManager.Instance.allResourceData[baseProduction.Key];
            if (resource.type != Resource.ResourceType.Material)
            {
                return;

            }
            float finalUpdatedAmount = baseProduction.Value;
            Debug.Log($"{characters.Count}");
            foreach (Character character in characters)
            {
                float productionAmount = character.Stats.speed * 0.2f;

                List<Character.BirthMark> birthMarks = character.BirthMarks.Where(bm => bm.type == typeof(IncreaseProductionOnBuildingBirthMark).ToString()).ToList();
                Debug.Log(string.Concat(birthMarks.Select(b => b.type.ToString())));
                if (birthMarks.Count == 0)
                {
                    continue;

                }
                //  Debug.Log($"{character.ID} {character.Name}");
                Debug.Log($"{birthMarks.Count}");
                List<BirthMarkData> birthMarkDatas = new List<BirthMarkData>();
                birthMarks.ForEach((bm) =>
                {
                    BirthMarkData birthMarkData = LoadManager.Instance.allBirthMarkDatas[bm.name];
                    Debug.Log($"{birthMarkData.name}");
                    if (birthMarkData != null)
                    {
                        birthMarkDatas.Add(ObjectCopier.Clone<BirthMarkData>(birthMarkData));
                        birthMarkDatas[birthMarkDatas.Count - 1].level = bm.level;

                    }


                });

                log.AppendLine($"{character.Name} : speed = {character.Stats.speed} increases {character.Stats.speed * 0.2f}");
                Debug.Log($"{string.Concat(birthMarkDatas.Select(b => ((IncreaseProductionOnBuildingBirthMark)b).buildingType))}");
                birthMarkDatas.Where(bData => ((IncreaseProductionOnBuildingBirthMark)bData).buildingType.ToArray().Contains(builder.Type)).ToList().ForEach((bData) =>
                {
                    log.AppendLine($"Affected BirthMarks are {bData.name}(Level{bData.level}) increase {productionAmount * bData.effectValues[bData.level]}");
                    productionAmount += productionAmount * bData.effectValues[bData.level];

                });

                finalUpdatedAmount += productionAmount;
                

            } /// End of Character loop

            log.AppendLine($"Total production : {finalUpdatedAmount}");
            Debug.Log($"{log}");

            Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

            builder.currentProductionAmount += finalUpdatedAmount;
            if (builder.currentProductionAmount >= buildData.maxProductionStored[builder.Level])
            {
                builder.currentProductionAmount = buildData.maxProductionStored[builder.Level];

            }
            EventManager.Instance.ResourceChanged(baseProduction.Key);

        }
        return;

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
        LoadManager.Instance.SavePlayerDataToJson();
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
        LoadManager.Instance.SavePlayerDataToJson();

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
    /// Debug Functions.
    public void ShowResourceTest()
    {
        foreach (KeyValuePair<string, Resource> resource in allResources)
        {
            Debug.Log($"{LoadManager.Instance.allResourceData[resource.Key].Name } : {resource.Value}");
        }

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


        //AddEquipment(1, 1);
        //AddEquipment(1, 1);
        //AddEquipment(2, 1);
        //AddEquipment(3, 1);

        //AddEquipment(4, 1);
        //AddEquipment(5, 1);
        //AddEquipment(6, 1);
        //AddEquipment(7, 1);
        //AddEquipment(8, 1);
        //AddEquipment(9, 1);
        //AddEquipment(10, 1);
        //AddEquipment(11, 1);

        //AddEquipment(12, 1);
        //AddEquipment(13, 1);
        //AddEquipment(14, 1);
        //AddEquipment(15, 1);
    }

}
