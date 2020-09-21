using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class TeamSelectorPanel : MonoBehaviour
{
    Mode mode;
    Builder builder;
    int finishPoint;
    Action<int> callback;
    bool haveCharacterSlot;

    public enum Mode
    {
        Quest,
        Build,
        Craft
    }
    public string activityName { get; set; }

    private void OnEnable()
    {
        EventManager.Instance.OnCharacterAssigned += RefreshAssignUI;
    }
    private void OnDisable()
    {
        if (EventManager.Instance)
            EventManager.Instance.OnCharacterAssigned -= RefreshAssignUI; ;
    }
    public void OnClickConfirmTeam(int currentSelectedTeam)
    {
        callback?.Invoke(currentSelectedTeam);
        UnityEngine.Debug.Log($"TryForc");
        GetComponent<ClosePanelHelper>().ForceClosePanel();
    }
    public void CreateTeamSelectorPanel(Mode _mode, Builder _builder, int _finishPoint, Action<int> _callback, bool _haveCharacterSlot)
    {
        this.builder = _builder;
        this.callback = _callback;
        this.haveCharacterSlot = _haveCharacterSlot;
        this.mode = _mode;
        this.finishPoint = _finishPoint;
       
        MainCanvas.FreezeCamera = true;
        gameObject.SetActive(true);
        ClearOldAssignUIData();
        switch (mode)
        {
            case Mode.Quest:
                {
                    CreateAssignBuildingContainer(builder, true);
                    transform.Find("AreaName").GetComponent<Text>().text = activityName;
                    break;
                }
            case Mode.Craft:
                {
                    CreateAssignBuildingContainer(builder, true);
                    break;
                }
            case Mode.Build:
                {

                    this.finishPoint = LoadManager.Instance.allBuildingData[builder.Type].upgradePoint[builder.Level];
                    CreateAssignBuildingContainer(BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter), false);

                    break;
                }
            default:
                {
                    UnityEngine.Debug.LogError($"No definition for {mode}");
                    break;
                }


        }
        if (haveCharacterSlot)
        {
            CreateCharacterSlot();
        }


    }


    void CreateAssignBuildingContainer(Builder builder, bool isInteractable)
    {
        GameObject container = gameObject.transform.Find("TeamPanel/Container").gameObject;
        if (container == null)
        {
            UnityEngine.Debug.LogError("Can't find Scrollview container for BuildingPanel.");
            return;
        }

        Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

        //  Debug.Log("Try Creating " + builder.Type.ToString());

        GameObject assignPanelContainerGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelContainerPrefab") as GameObject, container.transform);
        assignPanelContainerGO.transform.Find("BuildingImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(buildData.spritePath[builder.Level]);
        assignPanelContainerGO.transform.Find("BuildingImage/BuildingName").GetComponent<Text>().text = builder.Type.ToString();
        assignPanelContainerGO.name = builder.ID.ToString();

        for (int i = 0; i < buildData.maxCharacterStored[builder.Level].amount.Count; i++)
        {
            assignPanelContainerGO.GetComponent<AssignSlotCreator>().
            CreateTeamSelectableAssignSlot(assignPanelContainerGO.transform.Find("Container").gameObject, builder, i, finishPoint, isInteractable);

        }

    }
    void CreateCharacterSlot()
    {

        GameObject container = gameObject.transform.Find("CharacterPanel/Container").gameObject;
        if (container == null)
        {
            UnityEngine.Debug.LogError("Can't find Scrollview container for Character.");
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
            else if (character.workStatus == Character.WorkStatus.Quest)
            {
                characterSlot.GetComponent<Image>().color = Color.red;
            }
            else if (character.workStatus == Character.WorkStatus.Idle)
            {
                characterSlot.GetComponent<Image>().color = Color.white;
            }
        }

    }
    public void RefreshAssignUI()
    {
        ClearOldAssignUIData();
        if (haveCharacterSlot)
        {
            CreateCharacterSlot();
        }
        CreateTeamSelectorPanel(mode, builder, finishPoint, callback, haveCharacterSlot);
    }
    void ClearOldAssignUIData()
    {
        GameObject assignContainer = gameObject.transform.Find("TeamPanel/Container").gameObject;
        foreach (Transform transform in assignContainer.transform)
        {
            Destroy(transform.gameObject);
        }
        if (haveCharacterSlot)
        {
            GameObject characterContainer = gameObject.transform.Find("CharacterPanel/Container").gameObject;

            foreach (Transform transform in characterContainer.transform)
            {
                Destroy(transform.gameObject);
            }
        }


    }

}
