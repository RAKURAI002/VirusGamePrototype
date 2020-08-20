using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssignWorkCanvas : MonoBehaviour
{


    private void Awake()
    {
    }

    private void OnEnable()
    {
        RefreshAssignUI();
         EventManager.Instance.OnCharacterAssigned += RefreshAssignUI;
    }
    private void OnDisable()
    {
        if (EventManager.Instance)
            EventManager.Instance.OnCharacterAssigned -= RefreshAssignUI; ;
    }
    void CreateAssignBuildingContainer()
    {
        GameObject container = gameObject.transform.Find("BuildingPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for BuildingPanel.");
            return;
        }

        foreach (Builder builder in BuildManager.Instance.AllBuildings)
        {
            if(builder.Level == 0)
            {
                continue;
            }

            Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

            //Debug.Log("Try Creating " + builder.Type.ToString());
            
            GameObject assignPanelContainerGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelContainerPrefab") as GameObject, container.transform);
            assignPanelContainerGO.transform.Find("BuildingImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(buildData.spritePath[builder.Level]);
            assignPanelContainerGO.transform.Find("BuildingImage/BuildingName").GetComponent<Text>().text = builder.Type.ToString();
            assignPanelContainerGO.name = builder.ID.ToString();

            for(int i = 0; i < buildData.maxCharacterStored[builder.Level].amount.Count; i++)
            {
                assignPanelContainerGO.GetComponent<AssignSlotCreator>().CreateAssignSlot(assignPanelContainerGO.transform.Find("Container").gameObject, builder, i);
            }
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

        foreach (Character character in CharacterManager.Instance.AllCharacters)
        {
            
            GameObject characterSlot = new GameObject();
            characterSlot.name = character.Name;

            characterSlot.transform.SetParent(container.transform);
            characterSlot.AddComponent<RectTransform>();
            characterSlot.AddComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
            characterSlot.AddComponent<DraggableItem>();

            GameObject buildingWorked = new GameObject();
            buildingWorked.transform.localScale = new Vector3(0.3f, 0.3f);
            buildingWorked.transform.SetParent(characterSlot.transform);
            /*
            Builder builder = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == character.WorkingPlaceID);
            if (builder != null)
            {
                buildingWorked.AddComponent<Image>().sprite = Resources.Load<Sprite>(LoadManager.Instance.allBuildingData.SingleOrDefault(b => b.type == builder.Type).spritePath[builder.Level]);
            }*/

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


    public void RefreshAssignUI()
    {
        //Debug.Log("Refreshing AssignWork Canvas . . .");

        ClearOldAssignUIData();
        CreateCharacterSlot();
        CreateAssignBuildingContainer();
    }

    void ClearOldAssignUIData()
    {
        GameObject assignContainer = gameObject.transform.Find("BuildingPanel/Container").gameObject;
        foreach(Transform transform in assignContainer.transform)
        {
            Destroy(transform.gameObject);
        }

        GameObject characterContainer = gameObject.transform.Find("CharacterPanel/Container").gameObject;
        foreach (Transform transform in characterContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }

}
