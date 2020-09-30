using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
public class ResultPanel : MonoBehaviour
{
    public string QuestLog { get; set; }

    public string DetailQuestLog { get; set; }

    public void ShowResultPanel(Dictionary<string, int> rewardList)
    {
        GameObject container = transform.Find("ItemPanel/Container").gameObject;

        foreach (Transform item in container.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (KeyValuePair<string, int> resource in rewardList)
        {
            GameObject itemGO = new GameObject();
            itemGO.transform.SetParent(container.transform);

            if (LoadManager.Instance.allResourceData.ContainsKey(resource.Key))
            {

                itemGO.AddComponent<Image>().sprite = Resources.Load<Sprite>(LoadManager.Instance.allResourceData[resource.Key].spritePath);
            }
            else if (LoadManager.Instance.allEquipmentData.ContainsKey(resource.Key))
            {

                itemGO.AddComponent<Image>().sprite = Resources.Load<Sprite>(LoadManager.Instance.allEquipmentData[resource.Key].spritePath);
            }

            GameObject textGO = new GameObject();
            textGO.name = "Amount";
            textGO.transform.SetParent(itemGO.transform);
            Text text = textGO.AddComponent<Text>();
            text.text = resource.Value.ToString();
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().fontSize = 20;
            RectTransformExtensions.SetAnchor(text.GetComponent<RectTransform>(), AnchorPresets.BottonCenter);
            text.transform.position = text.transform.position + new Vector3(0, -10, 0);
        }
    }

    public void ShowQuestLog()
    {
        GameObject logGO = Instantiate(Resources.Load("Prefabs/UI/QuestLogPanel") as GameObject, gameObject.transform);
        logGO.transform.Find("ScrollPanel/Container").GetComponent<Text>().text = QuestLog;
        logGO.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => { EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false); });

        logGO.transform.Find("QuestLog").gameObject.SetActive(false);

        logGO.transform.Find("QuestLog").GetComponent<Button>().onClick.AddListener(() => {

            logGO.transform.Find("ScrollPanel/Container").GetComponent<Text>().text = QuestLog;
            logGO.transform.Find("QuestLog").gameObject.SetActive(false);
            logGO.transform.Find("DetailLog").gameObject.SetActive(true);

        });

        logGO.transform.Find("DetailLog").GetComponent<Button>().onClick.AddListener(() => {

            logGO.transform.Find("ScrollPanel/Container").GetComponent<Text>().text = DetailQuestLog;
            logGO.transform.Find("QuestLog").gameObject.SetActive(true);
            logGO.transform.Find("DetailLog").gameObject.SetActive(false);
        });
    }

    public void OnClickShowQuestLog()
    {
        ShowQuestLog();
    }

}
