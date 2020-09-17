using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignSlotCreator : MonoBehaviour
{
    public void CreateAssignSlot(GameObject container, Builder builder, int teamNumber)
    {
        Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

        GameObject assignPanelGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelPrefab") as GameObject, container.transform);

        assignPanelGO.GetComponentInChildren<Text>().text = $"Team {teamNumber + 1}";


        List<GameObject> slotListGO = new List<GameObject>();
        for (int i = 1; i <= buildData.maxCharacterStored[builder.Level].amount[teamNumber]; i++)
        {
            GameObject slotGO = new GameObject();
            slotGO.transform.SetParent(assignPanelGO.transform.Find("Container"));
            slotGO.AddComponent<Image>().color = (teamNumber % 2 == 0 ? Color.yellow : Color.green);
            Slot slot = slotGO.AddComponent<Slot>();
            slot.builder = builder;
            slot.teamNumber = teamNumber;
            slotGO.name = "Character" + (i).ToString() + "Team" + (teamNumber);
            slotGO.tag = "DropSlot";
            slotGO.transform.localScale = Vector3.one * 0.8f;

            slotListGO.Add(slotGO);
        }

        for (int i = 0; i < builder.CharacterInBuilding[teamNumber].Characters.Count; i++)
        {
            slotListGO[i].GetComponent<Slot>().character = builder.CharacterInBuilding[teamNumber].Characters[i];
        }

    }

    public void CreateTeamSelectableAssignSlot(GameObject container, Builder builder, int teamNumber, int point, bool isInteractable)
    {
        Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

        GameObject assignPanelGO = Instantiate(Resources.Load("Prefabs/UI/AssignPanelPrefab") as GameObject, container.transform);

        assignPanelGO.GetComponentInChildren<Text>().text = $"Team {teamNumber + 1}";
        if(builder.TeamLockState.Contains(teamNumber))
        {
            assignPanelGO.GetComponent<Image>().color = Color.red;
        }
        else
        {
            assignPanelGO.AddComponent<Button>().onClick.AddListener(() => {
                TeamSelectorPanel teamSelectorPanel = GameObject.FindObjectOfType<TeamSelectorPanel>();
                teamSelectorPanel.OnClickConfirmTeam(teamNumber);
            });
        }

        if (builder.Type == Building.BuildingType.LaborCenter)
        {
            float productionPoint = (float)(LoadManager.Instance.allBuildingData[Building.BuildingType.LaborCenter].production[builder.Level]["Production"]);
            if (builder.CharacterInBuilding != null)
            {
                productionPoint += builder.CharacterInBuilding[teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));
           
            }
            

            assignPanelGO.transform.Find("Duration").GetComponent<Text>().text = GetFormattedDuration((int)(point / productionPoint));
        }
        else if (builder.Type == Building.BuildingType.TownBase)
        {
            assignPanelGO.transform.Find("Duration").GetComponent<Text>().text = GetFormattedDuration((int)(point));

        }
        else if (builder.Type == Building.BuildingType.MedicalCenter || builder.Type == Building.BuildingType.Armory || builder.Type == Building.BuildingType.Kitchen)
        {
            float productionPoint = (float)(LoadManager.Instance.allBuildingData[builder.Type].production[builder.Level]["Production"]);
            if (builder.CharacterInBuilding != null)
            {
                productionPoint += builder.CharacterInBuilding[teamNumber].Characters.Sum(c => ((c.Stats.strength * 0.2f / 8) + (c.Stats.speed * 0.2f / 8) + (c.Stats.craftsmanship * 0.8f / 3)));

            }

            assignPanelGO.transform.Find("Duration").GetComponent<Text>().text = GetFormattedDuration((int)(point / productionPoint));

        }



        // Debug.Log($"{point} / {productionPoint} ");
        List<GameObject> slotListGO = new List<GameObject>();
        for (int i = 1; i <= buildData.maxCharacterStored[builder.Level].amount[teamNumber]; i++)
        {
            GameObject slotGO = new GameObject();
            slotGO.transform.SetParent(assignPanelGO.transform.Find("Container"));
            slotGO.AddComponent<Image>().color = (teamNumber % 2 == 0 ? Color.yellow : Color.green);
            Slot slot = slotGO.AddComponent<Slot>();
            slot.builder = builder;
            slot.isInteractable = isInteractable;
            slot.teamNumber = teamNumber;
            slotGO.name = "Character" + (i).ToString() + "Team" + (teamNumber);
            slotGO.tag = "DropSlot";

            slotListGO.Add(slotGO);
        }

        for (int i = 0; i < builder.CharacterInBuilding[teamNumber].Characters.Count; i++)
        {
            slotListGO[i].GetComponent<Slot>().character = builder.CharacterInBuilding[teamNumber].Characters[i];
        }


    }

    string GetFormattedDuration(int duration)
    {
        int hours = Mathf.FloorToInt(duration / 3600);
        int minutes = Mathf.FloorToInt(duration % 3600 / 60);
        int seconds = Mathf.FloorToInt(duration % 3600 % 60f);

        return $"{(hours > 0 ?  hours + "H " : "" )}{(minutes > 0 ? minutes + "M " : "")}{(seconds > 0 ? seconds + "s " : "")}";

    }

}
