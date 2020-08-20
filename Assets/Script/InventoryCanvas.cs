using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    public GameObject gridLayout;

    List<GameObject> itemImageGO;


    // Start is called before the first frame update
    private void Awake()
    {
        itemImageGO = new List<GameObject>();
    }
    void Start()
    {

    }

    private void OnEnable()
    {
        RefreshResourcePanel();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void RefreshResourcePanel()
    {
        if(itemImageGO != null)
        {
            foreach (GameObject go in itemImageGO)
            {
                Destroy(go);
            }
        }


        itemImageGO.Clear();
        foreach (KeyValuePair<string, Resource> resource in ItemManager.Instance.AllResources)
        {
            Resource rData = LoadManager.Instance.allResourceData[resource.Key];
            itemImageGO.Add(new GameObject());
            itemImageGO[itemImageGO.Count -1].name = rData.Name;
            itemImageGO[itemImageGO.Count - 1].AddComponent<Image>().sprite = (Resources.Load<Sprite>(rData.spritePath));
            itemImageGO[itemImageGO.Count - 1].transform.SetParent(gridLayout.transform);
            // Debug.Log($"{resourceImage[resourceImage.Count - 1].GetComponent<Image>().sprite.name}");

            GameObject textGO = new GameObject();
            textGO.name = "Amount";
            textGO.transform.SetParent(itemImageGO[itemImageGO.Count - 1].transform);
            Text text = textGO.AddComponent<Text>();
            text.text = resource.Value.Amount.ToString();
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().fontSize = 20;
            text.transform.position = text.transform.position + new Vector3(30,-110,0);
        }
    }
    void RefreshEquipmentPanel()
    {
        if (itemImageGO != null)
        {
            foreach (GameObject go in itemImageGO)
            {
                Destroy(go);
            }
        }


        itemImageGO.Clear();
        foreach (var equipment in ItemManager.Instance.AllEquipments)
        {
            itemImageGO.Add(new GameObject());
            itemImageGO[itemImageGO.Count - 1].name = equipment.Value.Name;
            itemImageGO[itemImageGO.Count - 1].AddComponent<Image>().sprite = (Resources.Load<Sprite>(equipment.Value.spritePath));
            itemImageGO[itemImageGO.Count - 1].transform.SetParent(gridLayout.transform);
            // Debug.Log($"{resourceImage[resourceImage.Count - 1].GetComponent<Image>().sprite.name}");

            GameObject textGO = new GameObject();
            textGO.name = "Amount";
            textGO.transform.SetParent(itemImageGO[itemImageGO.Count - 1].transform);
            Text text = textGO.AddComponent<Text>();
            text.text = (equipment.Value.AllAmount - equipment.Value.UsingAmount).ToString() + "/" + equipment.Value.AllAmount.ToString();
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().fontSize = 20;
            text.transform.position = text.transform.position + new Vector3(30, -110, 0);
        }
    }

    public void ShowResource()
    {
        Debug.Log("Refreshing Canvas To Resources.");
        RefreshResourcePanel();
    }
    public void ShowEquipment()
    {
        Debug.Log("Refreshing Canvas To Equipments.");
        RefreshEquipmentPanel();
    }
}
