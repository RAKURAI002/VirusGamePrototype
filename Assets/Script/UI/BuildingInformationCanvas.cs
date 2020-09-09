using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingInformationCanvas : MonoBehaviour
{
    Builder builder;
    Building buildingData;
    int x;
    private void Awake()
    {

    }
    private void Start()
    {
        RefreshInformationCanvas();
    }

    void Update()
    {
        UpdateSlider();
    }
    private void OnEnable()
    {
        EventManager.Instance.OnActivityFinished += OnActivityFinished;
        EventManager.Instance.OnCharacterAssigned += RefreshAssignUI;
        transform.Find("UpgradeInformationPanel").gameObject.SetActive(false);
        transform.Find("InformationPanel").gameObject.SetActive(true);
        buildingData = buildingData = LoadManager.Instance.allBuildingData[builder.Type];

        RefreshInformationCanvas();

       
    }
    private void OnDisable()
    {
        if(EventManager.Instance)
        {
            EventManager.Instance.OnActivityFinished -= OnActivityFinished;
            EventManager.Instance.OnCharacterAssigned -= RefreshAssignUI;
        }
           
    }

   void OnActivityFinished(ActivityInformation activity)
    {
        if(activity.activityType == ActivityType.Build)
            RefreshInformationCanvas();
    }
    public void ShowThisCanvas(Builder builder)
    {
        this.builder = builder;

        GameObject craftButtonGO = transform.Find("InformationPanel/BuildingOption/Craft").gameObject;
        if (builder.Type == Building.BuildingType.Kitchen || builder.Type == Building.BuildingType.MedicalCenter || builder.Type == Building.BuildingType.Armory)
        {
            craftButtonGO.SetActive(true);
        }
        else
        {
            craftButtonGO.SetActive(false);
        }

        gameObject.SetActive(true);
    }
    public void OnClickTryUpgradeButton()
    {
        transform.Find("UpgradeInformationPanel").gameObject.SetActive(true);
        transform.Find("InformationPanel").gameObject.SetActive(false);
        RefreshUpgradePanel();
    }
    public void OnClickAssignButton()
    {

        ShowAssignPanel();
    }
    public void OnClickCraftButton()
    {

        ShowCraftingPanelPanel();
    }
    public void OnClickDestroyButton()
    {
        BuildManager.Instance.RemoveBuilding(builder);
        GetComponent<ClosePanelHelper>().ForceClosePanel();

    }

    public void OnClickConfirmUpgrade()
    {
        ShowTeamSelectorPanel();


        //EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
    void ShowTeamSelectorPanel()
    {
        Builder laborCenter = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter);

        if (laborCenter != null)
        {
            TeamSelectorPanel teamSelectorPanel = Resources.FindObjectsOfTypeAll<TeamSelectorPanel>()[0];
            teamSelectorPanel.gameObject.SetActive(true);
            teamSelectorPanel.CreateTeamSelectorPanel(TeamSelectorPanel.Mode.Build, builder,
                LoadManager.Instance.allBuildingData[builder.Type].upgradePoint[builder.Level], TeamSelectorCallback, false);
        }
        else
        {
            Debug.Log($"No LaborCenter found. Using Default production point(5).");
            BuildManager.Instance.UpgradeBuilding(builder, 1);
            gameObject.GetComponent<ClosePanelHelper>().ForceClosePanel();

        }
    }
    void TeamSelectorCallback(int teamnumber)
    {
        BuildManager.Instance.UpgradeBuilding(builder, teamnumber);
        return;

    }
    void ShowAssignPanel()
    {
        GameObject assignPanel = transform.Find("AssignPanel").gameObject;
        assignPanel.SetActive(true);
        RefreshAssignUI();

    }

    void ShowCraftingPanelPanel()
    {
        GameObject craftingPanel = GameManager.FindInActiveObjectByName("CraftingPanel");
        craftingPanel.GetComponent<CraftingPanel>().ShowCraftingPanel(builder);

    }

    public void RefreshInformationCanvas()
    {
        GameObject destroyButtonGO = transform.Find("InformationPanel/BuildingOption/Destroy").gameObject;
        if (buildingData.IsStarterBuilding())
        {
            destroyButtonGO.SetActive(false);
        }
        else
        {
            destroyButtonGO.SetActive(true);
        }

        Image buildingImage = transform.Find("BuildImage").gameObject.GetComponent<Image>();
        Text buildingName = transform.Find("BuildName").gameObject.GetComponent<Text>();
        Text buildingDescription = transform.Find("InformationPanel/BuildDescription").gameObject.GetComponent<Text>();
        Text buildingLevel = transform.Find("BuildName/BuildLevel").gameObject.GetComponent<Text>();
        //Text buildingStatus = transform.Find("InformationPanel/BuildStatus").gameObject.GetComponent<Text>();


        buildingImage.sprite = builder.representGameObject.GetComponent<SpriteRenderer>().sprite;
        buildingName.text = builder.Type.ToString();
        buildingDescription.text = buildingData.description.ToString();
        buildingLevel.text = "Level : " + builder.Level.ToString();
        /*
        foreach (KeyValuePair<string, int> resource in buildingData.production[builder.Level])
        {
                buildingStatus.text = LoadManager.Instance.allResourceData[resource.Key].Name + " : " + resource.Value.ToString() ; /// ********************************************
        }*/

        
        return;
    }
    void UpdateSlider()
    {
        if (builder.constructionStatus.isConstructing)
        {
            Slider slider = transform.Find("TimerSlider").GetComponent<Slider>();
            slider.gameObject.SetActive(true);

            slider.maxValue = builder.constructionStatus.finishPoint;
            slider.value = builder.constructionStatus.currentPoint;

            long timer = GameObject.FindObjectOfType<BuildManager>().transform.Find("AllBuildings/" + builder.ID.ToString()).GetComponent<BuildTimer>().timer;
            int hours = Mathf.FloorToInt(timer / 3600);
            int minutes = Mathf.FloorToInt(timer % 3600 / 60);
            int seconds = Mathf.FloorToInt(timer % 3600 % 60f);


            Text text = slider.transform.GetComponentInChildren<Text>();
            text.text = "Upgrading : " + (hours == 0 ? "" : hours.ToString() + " HR ") + (minutes == 0 ? "" : minutes.ToString() + " M ") + seconds + " S";

            int speedUpCost = ItemManager.Instance.GetSpeedUpCost(builder.constructionStatus.finishPoint - builder.constructionStatus.currentPoint);
            Button speedUpButton = transform.Find("TimerSlider").GetComponentInChildren<Button>();
            speedUpButton.GetComponentInChildren<Text>().text = $"<color=blue>{speedUpCost.ToString()}</color>" + " Diamonds";
        }
        else
        {
            Slider slider = transform.Find("TimerSlider").GetComponent<Slider>();
            slider.gameObject.SetActive(false);
        }

    }
    void RefreshUpgradePanel()
    {
        Text buildingProduction = transform.Find("UpgradeInformationPanel/InformationPanel/BaseProduction").gameObject.GetComponent<Text>();
        Text newBuildingLevel = transform.Find("UpgradeInformationPanel/InformationPanel/UpBuildingLevel").gameObject.GetComponent<Text>();
        Text newBuildingProduction = transform.Find("UpgradeInformationPanel/InformationPanel/UpBaseProduction").gameObject.GetComponent<Text>();
        Text upgradeCost = transform.Find("UpgradeInformationPanel/InformationPanel/UpgradeCost").gameObject.GetComponent<Text>();

        Button upgradeButton = transform.Find("UpgradeInformationPanel/ConfirmButton").GetComponent<Button>();
        buildingProduction.text = "Base production\n";
        newBuildingLevel.text = "Level : ";
        newBuildingProduction.text = "New Base production\n";
        upgradeCost.text = "Upgrade Cost :\n";
        upgradeButton.interactable = true;
        foreach (KeyValuePair<string, int> resource in buildingData.buildingCost[builder.Level])
        {
           
            bool isAffordable = (resource.Value <= (ItemManager.Instance.AllResources.ContainsKey(resource.Key)? ItemManager.Instance.AllResources[resource.Key].Amount : 0));
            upgradeCost.text += "\n" + LoadManager.Instance.allResourceData[resource.Key].Name + " : " +   
            ((!isAffordable ? ($"<color=red>{resource.Value.ToString()}(Not Enough)</color>") : resource.Value.ToString()));
           
            if(!isAffordable)
            {
                upgradeButton.interactable = false;
            }
        }

        foreach (KeyValuePair<string, int> resource in buildingData.production[builder.Level])
        {
           
                buildingProduction.text += "\n" + LoadManager.Instance.allResourceData[resource.Key].Name + " : " + resource.Value.ToString();

          
        }

        if (builder.Level == builder.maxLevel)
        {
            newBuildingLevel.text += "<color=red>Max Upgrade</color>";
            newBuildingProduction.text += "\n<color=red>Max Upgrade</color>";
            upgradeButton.interactable = false;
        }
        else
        {
            foreach (KeyValuePair<string, int> resource in buildingData.production[builder.Level + 1])
            {
                newBuildingProduction.text += $"\n<color=blue>{LoadManager.Instance.allResourceData[resource.Key].Name} : {resource.Value.ToString()}</color>";
            }
            newBuildingLevel.text += $"{builder.Level} >> <color=blue>{builder.Level + 1}</color>";
        }
      
    }

    public void CloseUpgradePanel()
    {
        transform.Find("InformationPanel").gameObject.SetActive(true);
        Debug.Log(transform.Find("InformationPanel").gameObject.activeSelf);
        transform.Find("UpgradeInformationPanel").gameObject.SetActive(false);
        return;
    }
    public void RefreshAssignUI()
    {
        Debug.Log("Refreshing AssignWork Canvas . . .");

        ClearAssignUIOldData();
        CreateCharacterSlot();
        CreateAssignBuildingContainer();
    }

    void CreateAssignBuildingContainer()
    {
        GameObject container = gameObject.transform.Find("AssignPanel/TeamPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for BuildingPanel.");
            return;
        }

        

            Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

            Debug.Log("Try Creating " + builder.Type.ToString());

            GameObject assignPanelContainerGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelContainerPrefab") as GameObject, container.transform);
            assignPanelContainerGO.transform.Find("BuildingImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(buildData.spritePath[builder.Level]);
            assignPanelContainerGO.transform.Find("BuildingImage/BuildingName").GetComponent<Text>().text = builder.Type.ToString();
            assignPanelContainerGO.name = builder.ID.ToString();

            for (int i = 0; i < buildData.maxCharacterStored[builder.Level].amount.Count; i++)
            {
                assignPanelContainerGO.GetComponent<AssignSlotCreator>().CreateAssignSlot(assignPanelContainerGO.transform.Find("Container").gameObject, builder, i);
            }
        
    }
   

    void CreateCharacterSlot()
    {

        GameObject container = gameObject.transform.Find("AssignPanel/CharacterPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for Character.");
            return;
        }
        // Debug.Log(CharacterManager.Instance.AllCharacters.Count);
        foreach (Character character in CharacterManager.Instance.AllCharacters)
        {

            GameObject characterSlot = new GameObject();
            characterSlot.name = character.ID.ToString();

            characterSlot.transform.SetParent(container.transform);
            characterSlot.AddComponent<RectTransform>();
            characterSlot.AddComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
            characterSlot.AddComponent<DraggableItem>();

            if (character.workStatus == Character.WorkStatus.Working)
            {
                characterSlot.GetComponent<Image>().color = Color.blue;
            }
            if (character.workStatus == Character.WorkStatus.Quest)
            {
                characterSlot.GetComponent<Image>().color = Color.red;
            }
            else if (character.workStatus == Character.WorkStatus.Idle)
            {
                characterSlot.GetComponent<Image>().color = Color.white;
            }
        }

    }
    public void OnClickSpeedUpButton()
    {
        float pointLeft = builder.constructionStatus.finishPoint - builder.constructionStatus.currentPoint;

        int speedUpCost = ItemManager.Instance.GetSpeedUpCost(pointLeft);
        Button speedUpButton = transform.Find("TimerSlider").GetComponentInChildren<Button>();
        speedUpButton.GetComponentInChildren<Text>().text = speedUpCost.ToString() + " Diamonds";

        Debug.Log($"Point left : {pointLeft}, Need {speedUpCost} Diamonds to speed up this.");
        if(ItemManager.Instance.TryConsumeResources("Diamond", speedUpCost))
        {
            builder.constructionStatus.currentPoint += pointLeft;
        }
        else
        {
            Debug.LogWarning($"You need {speedUpCost} Diamonds to purchase this speed up.");
        }

        
    }
    void ClearAssignUIOldData()
    {
        GameObject assignContainer = gameObject.transform.Find("AssignPanel/TeamPanel/Container").gameObject;
        foreach (Transform transform in assignContainer.transform)
        {
            Destroy(transform.gameObject);
        }

        GameObject characterContainer = gameObject.transform.Find("AssignPanel/CharacterPanel/Container").gameObject;
        foreach (Transform transform in characterContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }

}