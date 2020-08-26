using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        RefreshResourcePanel();
    }

    void RefreshResourcePanel()
    {
        GameObject container = transform.Find("InventoryPanel/ItemPanel/Container").gameObject;

        foreach(Transform transform in container.transform)
        {
            Destroy(transform.gameObject);

        }

        foreach(KeyValuePair<string, Resource> resource in ItemManager.Instance.AllResources)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
            itemGO.GetComponent<Image>().sprite = Resources.Load<Sprite>(resource.Value.spritePath);

            Text amount = itemGO.GetComponentInChildren<Text>();
            amount.text = ItemManager.Instance.GetResourceAmount(resource.Value.Name).ToString();    
            
        }
    }
    void RefreshEquipmentPanel()
    {
        GameObject container = transform.Find("InventoryPanel/ItemPanel/Container").gameObject;

        foreach(Transform transform in container.transform)
        {
            Destroy(transform.gameObject);

        }

        foreach(KeyValuePair<string, Equipment> equipment in ItemManager.Instance.AllEquipments)
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
            itemGO.GetComponent<Image>().sprite = Resources.Load<Sprite>(equipment.Value.spritePath);

            Text amount = itemGO.GetComponentInChildren<Text>();
            amount.text = (equipment.Value.AllAmount).ToString();

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
