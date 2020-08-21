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

    private void Awake()
    {

    }
    private void Start()
    {
        
        
        RefreshInformationCanvas();
    }

    void Update()
    {

    }
    private void OnEnable()
    {
        EventManager.Instance.OnCharacterAssigned += RefreshAssignUI;
        transform.Find("UpgradeInformationPanel").gameObject.SetActive(false);
        transform.Find("InformationPanel").gameObject.SetActive(true);
        buildingData = buildingData = LoadManager.Instance.allBuildingData[builder.Type];

        RefreshInformationCanvas();
    }
    private void OnDisable()
    {
        if(EventManager.Instance)
            EventManager.Instance.OnCharacterAssigned -= RefreshAssignUI;
    }
    public void ShowThisCanvas(Builder builder)
    {
        this.builder = builder;

        GameObject craftButtonGO = transform.Find("InformationPanel/BuildingOption/Craft").gameObject;
        if (builder.Type == Building.BuildingType.Kitchen || builder.Type == Building.BuildingType.MedicalCenter)
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
        gameObject.SetActive(false);
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
           // gameObject.SetActive(false);

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
        Image buildingImage = transform.Find("BuildImage").gameObject.GetComponent<Image>();
        Text buildingName = transform.Find("BuildName").gameObject.GetComponent<Text>();
        Text buildingDescription = transform.Find("InformationPanel/BuildDescription").gameObject.GetComponent<Text>();
        Text buildingLevel = transform.Find("BuildName/BuildLevel").gameObject.GetComponent<Text>();
        //Text buildingStatus = transform.Find("InformationPanel/BuildStatus").gameObject.GetComponent<Text>();


        buildingImage.sprite = builder.representGameObject.GetComponent<SpriteRenderer>().sprite;
        buildingName.text = builder.Type.ToString();
       // buildingDescription.text = buildingData..ToString();
        buildingLevel.text = "Level : " + builder.Level.ToString();
        /*
        foreach (KeyValuePair<string, int> resource in buildingData.production[builder.Level])
        {
                buildingStatus.text = LoadManager.Instance.allResourceData[resource.Key].Name + " : " + resource.Value.ToString() ; /// ********************************************
        }*/
        
        return;
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