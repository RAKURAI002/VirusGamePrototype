using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamUpCanvas : MonoBehaviour
{
    private void Awake()
    {


    }
    void Start()
    {
        Button readyButton = transform.Find("ReadyButton").GetComponent<Button>();
        readyButton.onClick.AddListener(OnClickReady);

    }
    private void OnEnable()
    {
        RefreshAssignUI();

        EventManager.Instance.OnCharacterAssigned += RefreshAssignUI;
    }
    private void OnDisable()
    {
        if (EventManager.Instance)
            EventManager.Instance.OnCharacterAssigned -= RefreshAssignUI;
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
        GameObject container = gameObject.transform.Find("TeamPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for BuildingPanel.");
            return;
        }
        Builder builder = BuildingManager.Instance.AllBuildings.Single( b => b.Type == Building.BuildingType.TownBase);



        Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

        Debug.Log("Try Creating " + builder.Type.ToString());

        GameObject assignPanelContainerGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelContainerPrefab") as GameObject, container.transform);
        assignPanelContainerGO.transform.Find("BuildingImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(buildData.spritePath[builder.Level]);
        assignPanelContainerGO.transform.Find("BuildingImage/BuildingName").GetComponent<Text>().text = builder.Type.ToString();
        assignPanelContainerGO.name = builder.ID.ToString();

        int questDuration = LoadManager.Instance.allQuestData.Single(q => q.Value.questName == QuestManager.Instance.selectingLevel).Value.duration;
        for (int i = 0; i < buildData.maxCharacterStored[builder.Level].amount.Count; i++)
        {
            assignPanelContainerGO.GetComponent<AssignSlotCreator>().
                CreateTeamSelectableAssignSlot(assignPanelContainerGO.transform.Find("Container").gameObject, builder, i, questDuration, true);
        }

    }


    void CreateCharacterSlot()
    {

        GameObject container = gameObject.transform.Find("CharacterPanel/Container").gameObject;
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
        GameObject assignContainer = gameObject.transform.Find("TeamPanel/Container").gameObject;
        foreach (Transform transform in assignContainer.transform)
        {
            Destroy(transform.gameObject);
        }

        GameObject characterContainer = gameObject.transform.Find("CharacterPanel/Container").gameObject;
        foreach (Transform transform in characterContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }
    void OnClickReady()
    {
        GameObject.FindGameObjectsWithTag("ExtraUIPanel").ToList().ForEach((go) => { go.SetActive(false); });
        
        QuestManager.Instance.StartQuest(1);
    }
}
