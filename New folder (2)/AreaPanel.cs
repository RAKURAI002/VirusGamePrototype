using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class AreaPanel : MonoBehaviour
{
    public GameObject informationPanel;
    public GameObject bgPanel;
    public GameObject teamUpCanvas;

    [SerializeField] public string areaName;

    private void Start()
    {
        QuestManager.Instance.isAvoidBattle = false;
    }
    private void OnEnable()
    {
        informationPanel.SetActive(false);
    }

    public void SelectLevel()
    {
        areaName = EventSystem.current.currentSelectedGameObject.name.Replace("Button", "");

        OnClickNormal();
     
    }
    public void OnClickNormal()
    {
        bgPanel.GetComponent<Image>().color = Color.yellow;
        QuestManager.Instance.selectingLevel = areaName + "Normal";
        informationPanel.SetActive(true);

    }

    public void OnClickHard()
    {
        bgPanel.GetComponent<Image>().color = Color.red;
        QuestManager.Instance.selectingLevel = areaName + "Hard";

    }

    public void OnClickReady()
    {
         TeamSelectorPanel teamSelectorPanel = Resources.FindObjectsOfTypeAll<TeamSelectorPanel>()[0];
         teamSelectorPanel.gameObject.SetActive(true);
         teamSelectorPanel.activityName = QuestManager.Instance.selectingLevel;
         teamSelectorPanel.CreateTeamSelectorPanel(TeamSelectorPanel.Mode.Quest, 
                BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.TownBase),
                LoadManager.Instance.allQuestData.SingleOrDefault(q => q.Value.questName == QuestManager.Instance.selectingLevel).Value.duration, QuestManager.Instance.StartQuest , true);
    
    }
    public void OnClickAvoidBattle()
    {
        QuestManager.Instance.isAvoidBattle = true;
    }
}
