using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;
using System.Linq;
using UnityEngine.Events;
using System;

public class MainCanvas : MonoBehaviour
{
    /// Assign in Inspector
    public Button mapButton;
    public GameObject inventoryCanvas;
    public GameObject buildingShopCanvas;
    public GameObject toWorkCanvas;
    public GameObject[] characterCanvas;

    public GameObject resourcePanel;

    public GameObject confirmationPanel;
    public GameObject warningPanel;
    public GameObject notificationPanelContainer;
    public Text playerName;
    public Text playerLevel;

    public Animator selectButtonAnimator;
    /// --------------------------------------------------
    GameObject uiCanvas;

    bool selectButtonToggle;
    GameObject editBuildingPanel;
    GameObject finishedActivityAmountGO;
    GameObject waitingCharacterAmountGO;

    public static bool canvasActive { get; set; } /// Freeze action with Camera while Panel is opening.

    void OnEnable()
    {
        EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
        EventManager.Instance.OnActivityFinished += OnActivityFinished;
        EventManager.Instance.OnPlayerLevelUp += OnPlayerLevelUp;
        EventManager.Instance.OnResourceChanged += OnResourceChanged;
        EventManager.Instance.OnCharacterAssigned += OnCharacterAddEvent;
    }
    void OnCharacterAddEvent()
    {
        RefreshWaitingCharacterAmount();
    }
    void OnResourceChanged(string name)
    {
        Transform resourceGO = resourcePanel.transform.Find(name + "Panel");

        if(resourceGO)
        {
            resourceGO.gameObject.GetComponentInChildren<Text>().text = ItemManager.Instance.AllResources[name].Amount.ToString();
        }
    }
    void OnPlayerLevelUp(int level)
    {
        playerLevel.text = "Level " + level.ToString();
    }
    private void OnActivityAssigned(ActivityInformation activityInformation)
    {
        
    }

    private void OnActivityFinished(ActivityInformation activityInformation)
    {
        RefreshNotificationAmount();
    }

    void OnDisable()
    {

        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityFinished;
            EventManager.Instance.OnPlayerLevelUp -= OnPlayerLevelUp;
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
            EventManager.Instance.OnCharacterAssigned -= OnCharacterAddEvent;
        }
    }

    private void Start()
    {
        finishedActivityAmountGO = GameManager.FindInActiveObjectByName("FinishedActivityAmount");
        waitingCharacterAmountGO = GameManager.FindInActiveObjectByName("WaitingCharacterAmount");
        playerName.text += $"<color=red>{LoadManager.Instance.playerData.name}</color>";
        playerLevel.text += LoadManager.Instance.playerData.level.ToString();
        selectButtonToggle = false;;
        editBuildingPanel = Resources.FindObjectsOfTypeAll<BuildingInformationCanvas>()[0].gameObject;

        uiCanvas = GameObject.Find("UICanvas");


        if (editBuildingPanel == null)
        {
            Debug.LogError("Can't find EditBuildingPanel");
        }
        RefreshNotificationAmount();
        RefreshWaitingCharacterAmount();
        UpdateResourcePanel();
    }
    void UpdateResourcePanel()
    {
        resourcePanel.transform.Find("GoldPanel").gameObject.GetComponentInChildren<Text>().text = ItemManager.Instance.GetResourceAmount("Gold").ToString();
        resourcePanel.transform.Find("DiamondPanel").gameObject.GetComponentInChildren<Text>().text = ItemManager.Instance.GetResourceAmount("Diamond").ToString();

        resourcePanel.transform.Find("WoodPanel").gameObject.GetComponentInChildren<Text>().text = ItemManager.Instance.GetResourceAmount("Wood").ToString();
        resourcePanel.transform.Find("FoodPanel").gameObject.GetComponentInChildren<Text>().text = ItemManager.Instance.GetResourceAmount("Food").ToString();
    
    }
    private void Update()
    {
        if (CameraPan.isPanning)
                return;
        if (!canvasActive && Input.GetMouseButtonUp(0) )
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = -Camera.main.transform.position.z;
                Vector3 mouseToWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
                mouseToWorldPoint.z = 0;

                /// Prioritize Raycast. ****** Wait for better algorithm. **********
                RaycastHit2D[] rayHits = Physics2D.RaycastAll(mouseToWorldPoint, Vector2.zero);
                int result = 0;
                GameObject resultGO = null;
                foreach(RaycastHit2D rayHit in rayHits)
                {
                    if (rayHit.collider != null)
                    {
                        if (rayHit.collider.gameObject.tag == "Building")
                        {
                            result = 1;
                            resultGO = rayHit.collider.gameObject;
                            break;
                        }
                        else if (rayHit.collider.gameObject.tag == "Fog")
                        {
                            result = 2;
                            resultGO = rayHit.collider.gameObject;
                            break;
                            
                        }
                        else if (rayHit.collider.gameObject.tag == "Tree")
                        {
                            result = 3;
                            resultGO = rayHit.collider.gameObject;
                        }

                    }
                }
                switch (result)
                {
                    case 1:
                        {
                            ShowEditBuildingPanel(resultGO);
                            break;
                        }
                    case 2:
                        {
                            ShowWarningPanel(resultGO);
                            break;
                        }
                    case 3:
                        {
                            ShowExpandAreaPanel(resultGO);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                

            }
        }
    }
    public void RefreshNotificationAmount()
    {
        int notiAmount = NotificationManager.Instance.ProcessingActivies.Where(pa => pa.Value.isFinished).ToList().Count;

        if (notiAmount == 0)
        {
            finishedActivityAmountGO.SetActive(false);

        }
        else
        {
            finishedActivityAmountGO.SetActive(true);
            finishedActivityAmountGO.GetComponentInChildren<Text>().text = notiAmount.ToString();

        }

    }
    public void RefreshWaitingCharacterAmount()
    {
        int characterAmount = CharacterManager.Instance.characterWaitingInLine.Count;

        if (characterAmount == 0)
        {
            waitingCharacterAmountGO.transform.parent.gameObject.SetActive(false);

        }
        else
        {
            waitingCharacterAmountGO.transform.parent.gameObject.SetActive(true);
            waitingCharacterAmountGO.GetComponentInChildren<Text>().text = characterAmount.ToString();

        }

    }
    void ShowExpandAreaPanel(GameObject selectedGameObject)
    {
        confirmationPanel.GetComponent<ExpandConfirmationPanel>().expandingAreaID = int.Parse(selectedGameObject.name);
        confirmationPanel.SetActive(true);
        canvasActive = true;

    }
    void ShowWarningPanel(GameObject selectedGameObject)
    {
        warningPanel.SetActive(true);
        warningPanel.GetComponentInChildren<WarningText>().SetWarningMessage($"You need to be Level {int.Parse(selectedGameObject.name) * 5} to unlock this.", 2);
    
    }
    void ShowEditBuildingPanel(GameObject selectedGameObject)
    {  
        BuildingInformationCanvas editBuilding = editBuildingPanel.GetComponent<BuildingInformationCanvas>();
        if (editBuilding == null)
        {
            Debug.LogError("Can't find EditBuilding Component.");
        }
        Builder selectedBuilding = BuildManager.Instance.AllBuildings.Single(b => b.representGameObject == selectedGameObject);
      /*
       if(selectedBuilding.Level == 0)
        {
            return;

        }*/
        editBuilding.ShowThisCanvas(selectedBuilding);
        canvasActive = true;

    }

    public void OnClickWorldMap()
    {
        Builder townBase = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase);
        if (townBase == null)
        {
            Debug.LogWarning("You NEED to create TownBase before proceed the Exploration.");
            return;

        }
        if(townBase.Level == 0)
        {
            Debug.LogWarning("You can't proceed the Exploration with level 0 TownBase !");
            return;
        }

        canvasActive = false;
        SceneManager.LoadScene("WorldMap");

    }

    public void OnClickBase()
    {
        SceneManager.LoadScene("BaseScene");

    }

    public void OnBuildingShop()
    {
        buildingShopCanvas.SetActive(true);

    }

    public void OnClickInventory()
    {
        inventoryCanvas.SetActive(true);
        canvasActive = true;

    }

    public void OnClickCharacter()
    {
        characterCanvas[0].SetActive(true);
        characterCanvas[1].SetActive(false);
        characterCanvas[2].SetActive(false);
        canvasActive = true;

    }

    public void OnClickNotification()
    {
        foreach (Transform child in notificationPanelContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        notificationPanelContainer.transform.parent.parent.gameObject.SetActive(true);
        canvasActive = true;

        int notiAmount = NotificationManager.Instance.ProcessingActivies.Where(pa => pa.Value.isFinished).ToList().Count;

        /*
        if (notiAmount == 0)
        {
            finishedQuestAmountGO.SetActive(false);

        }
        else
        {
            finishedQuestAmountGO.GetComponentInChildren<Text>().text = notiAmount.ToString();

        }*/

    }

    public void OnClickCharacterNotification()
    {
        GameObject characterNotification = transform.Find("AddCharacterPanel").gameObject;
        characterNotification.SetActive(true);
        canvasActive = true;

      //  int notiAmount = CharacterManager.Instance.characterWaitingInLine.ToList().Count;
/*
        if (notiAmount == 0)
        {
            characterNotification.SetActive(false);
        
        }
        else
        {
            characterNotificationButtonContainer.GetComponentInChildren<Text>().text = notiAmount.ToString();
        
        }*/



    }

    public void OnClickToWork()
    {
        toWorkCanvas.SetActive(true);
        canvasActive = true;

    }
    public void OnClickExitButton()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);

    }
    public void OpenMenu()
    {
        selectButtonToggle = !selectButtonToggle;
        selectButtonAnimator.SetBool("IsOpen", selectButtonToggle);
    }

    IEnumerator DelaySetCanvasActive(bool active)

    {
        yield return new WaitForEndOfFrame();
        canvasActive = active;
    }
}
