using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CraftingPanel : MonoBehaviour
{

    string currentSelectedItemName;
    Builder builder;

    private void OnEnable()
    {
        KeyValuePair<string, Resource> defaultCrafting = ItemManager.Instance.AllResources.FirstOrDefault(r => r.Value.type == Resource.ResourceType.Recipe);
        if (defaultCrafting.Value != null)
        {
            currentSelectedItemName = defaultCrafting.Value.Name;
        }
        else
        {
            currentSelectedItemName = null;
        }

        RefreshCraftingPanel();
        EventManager.Instance.OnResourceChanged += OnResourceChanged;
    }
    private void OnDisable()
    {
        if(EventManager.Instance)
             EventManager.Instance.OnResourceChanged -= OnResourceChanged;
    }

    public void ShowCraftingPanel(Builder builder)
    {
        this.builder = builder;
        gameObject.SetActive(true);
    }
    void RefreshCraftingPanel()
    {
        RefreshCraftableList();
        RefreshItemInformation();
    }
    void OnResourceChanged(string name)
    {
        RefreshCraftableList();
    }

    void RefreshCraftableList()
    {
        ClearCraftableListOldData();
        GameObject container = transform.Find("CraftableListPanel/Container").gameObject;
        foreach (KeyValuePair<string, Resource> resourceRecipe in ItemManager.Instance.AllResources.Where(r => r.Value.type == Resource.ResourceType.Recipe))
        {
            Resource resource = LoadManager.Instance.allResourceData[resourceRecipe.Key.Replace("Recipe:", "")];

            GameObject craftableItemGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
            craftableItemGO.name = resourceRecipe.Key;

            Image image = craftableItemGO.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(resource.spritePath);
         
            Button button = craftableItemGO.AddComponent<Button>();
            button.onClick.AddListener(() => { currentSelectedItemName = EventSystem.current.currentSelectedGameObject.name; RefreshItemInformation(); });

            Text amount = craftableItemGO.GetComponentInChildren<Text>();
            amount.text = ItemManager.Instance.GetResourceAmount(resource.Name).ToString();
        }
    }
    public void OnClickCraftButton()
    {
        TryCraftItem();
    }
    bool TryCraftItem()
    {
        Resource resourceRecipe = LoadManager.Instance.allResourceData[currentSelectedItemName];
        Resource resource = LoadManager.Instance.allResourceData[currentSelectedItemName.Replace("Recipe:", "")];
        if (ItemManager.Instance.IsAffordable(resourceRecipe.craftingData.craftingMaterials))
        {
            TeamSelectorPanel teamSelectorPanel = Resources.FindObjectsOfTypeAll<TeamSelectorPanel>()[0];
            teamSelectorPanel.CreateTeamSelectorPanel(TeamSelectorPanel.Mode.Craft,
                   BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.Kitchen),
                   resourceRecipe.craftingData.point, (_teamNumber)=> {
                       if (ItemManager.Instance.TryConsumeResources(resourceRecipe.craftingData.craftingMaterials))
                       {
                           RefreshCraftingPanel();

                       }

                       NotificationManager.Instance.AddActivity(new ActivityInformation()
                       {
                           activityName = ("Craft:" + resource.Name),
                           activityType = ActivityType.Craft,
                           startPoint = 0,
                           finishPoint = resourceRecipe.craftingData.point,
                           teamNumber = _teamNumber,
                           InformationID = resourceRecipe.ID
                       });

                       builder.TeamLockState.Add(_teamNumber);

                   }, false);

            /*
            if(ItemManager.Instance.TryConsumeResources(resourceRecipe.craftingData.craftingMaterials))
            {
               // ItemManager.Instance.AddResource(resource.Name, 1);
                RefreshCraftingPanel();
            }
            NotificationManager.Instance.AddActivity(new ActivityInformation()
            {
                activityName = ("Craft:" + resource.Name),
                activityType = ActivityType.Craft,
                startTime = DateTime.Now.Ticks,
                finishTime = DateTime.Now.Ticks + (resourceRecipe.craftingData.point * TimeSpan.TicksPerSecond),
            //    teamNumber = _teamNumber,
             //   InformationID = currentQuest.questID
            });
            */
        }
        else
        {
            Debug.LogError("Something happened here."); /// IsAffordable must be checked by RefreshItemInformation() before come to this function.
            return false;
        }
        return true;
    }

    void RefreshItemInformation()
    {
        ClearItemInformationOldData();

        if (currentSelectedItemName == null)
        {
            return;
        }
        Resource resource = LoadManager.Instance.allResourceData[currentSelectedItemName.Replace("Recipe:", "")];
        Resource resourceRecipe = LoadManager.Instance.allResourceData[currentSelectedItemName];
        GameObject informationPanel = transform.Find("ItemInformationPanel/ItemInformation").gameObject;
        informationPanel.transform.Find("ItemName").GetComponent<Text>().text = resource.Name;
        informationPanel.transform.Find("ItemRarity").GetComponent<Text>().text = resource.Rarity.ToString();
        informationPanel.transform.Find("ItemDescription").GetComponent<Text>().text = resource.description;
        informationPanel.transform.Find("ItemImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(resource.spritePath);

        GameObject container = transform.Find("ItemInformationPanel/ItemIngredientPanel/Container").gameObject;

        foreach (var component in resourceRecipe.craftingData.craftingMaterials)
        {
            Resource material = LoadManager.Instance.allResourceData[component.Key];
            GameObject ingredientPanel = Instantiate(Resources.Load("Prefabs/UI/IngredientPanelPrefab") as GameObject, container.transform);
            ingredientPanel.transform.Find("IngredientImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(material.spritePath);
            ingredientPanel.transform.Find("IngredientName").GetComponent<Text>().text = material.Name;
            ingredientPanel.transform.Find("IngredientAmount").GetComponent<Text>().text = $"{ItemManager.Instance.GetResourceAmount(component.Key)} / {component.Value}";
        }

        Building buildingData = LoadManager.Instance.allBuildingData[builder.Type];
        GameObject craftingTimePanel = transform.Find("ItemInformationPanel/ItemCraftingTimePanel").gameObject;
        craftingTimePanel.transform.Find("CookPoint").GetComponent<Text>().text = resourceRecipe.craftingData.point.ToString() + " Cookpoint";
        craftingTimePanel.transform.Find("CookTime").GetComponent<Text>().text = (resourceRecipe.craftingData.point / buildingData.production[builder.Level]["Production"]).ToString() + " s";

        Button confirmButton = transform.Find("ConfirmButton").GetComponent<Button>();
        if (!ItemManager.Instance.IsAffordable(resourceRecipe.craftingData.craftingMaterials))
        {
            confirmButton.interactable = false;
        }
        else
        {
            confirmButton.interactable = true;
        }

    }

    void ClearCraftableListOldData()
    {


        Transform container = transform.Find("CraftableListPanel/Container");

        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);
        }
    }
    void ClearItemInformationOldData()
    {
        Transform container = transform.Find("ItemInformationPanel/ItemIngredientPanel/Container");

        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);
        }
    }
}
