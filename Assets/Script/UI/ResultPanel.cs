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
    List<GameObject> itemImageGO;
    bool isPointerOverUI;
    public string QuestLog { get; set; }
    private void Awake()
    {
        itemImageGO = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Update()
    {
    }
    public void ShowResultPanel(Dictionary<string, int> rewardList)
    {
        if (itemImageGO != null)
        {
            foreach (GameObject go in itemImageGO)
            {
                Destroy(go);
            }
        }


        itemImageGO.Clear();

       

        GameObject container = transform.Find("ItemPanel/Container").gameObject;

        foreach (KeyValuePair<string, int> resource in rewardList)
        {
            
            GameObject itemGO = new GameObject();
            itemGO.transform.SetParent(container.transform);
            itemGO.AddComponent<Image>().sprite = Resources.Load<Sprite>(LoadManager.Instance.allResourceData[resource.Key].spritePath);

            GameObject textGO = new GameObject();
            textGO.name = "Amount";
            textGO.transform.SetParent(itemGO.transform);
            Text text = textGO.AddComponent<Text>();
            text.text = resource.Value.ToString();
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().fontSize = 20;
            RectTransformExtensions.SetAnchor(text.GetComponent<RectTransform>(), AnchorPresets.BottonCenter);
            text.transform.position = text.transform.position + new Vector3(0, -10, 0);
            itemImageGO.Add(itemGO);
        }


    }

    public void ShowQuestLog()
    {
        GameObject logGO = Instantiate(Resources.Load("Prefabs/UI/QuestLogPanel") as GameObject, gameObject.transform);
        logGO.transform.Find("ScrollPanel/Container").GetComponent<Text>().text = QuestLog;
        logGO.GetComponentInChildren<Button>().onClick.AddListener(()=> { EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false); });
    }

    public void OnClickShowQuestLog()
    {
        ShowQuestLog();
    }
    IEnumerator ClosePanel()
    {
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
        MainCanvas.FreezeCamera = false;
    }
}
