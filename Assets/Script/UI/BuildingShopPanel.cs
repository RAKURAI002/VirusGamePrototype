using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingShopPanel : MonoBehaviour
{
    public Button cancelButton;
    Button currentSelectedButton;

    private void OnEnable()
    {
        EventManager.Instance.OnResourceChanged += OnResourceChanged;
        RefreshPanel();
    }
    private void OnDisable()
    {
        MainCanvas.canvasActive = false;
        if (EventManager.Instance)
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {

            MainCanvas.canvasActive = true;
        }
        else
        {
            MainCanvas.canvasActive = false;
        }
    }
    void OnResourceChanged(string name)
    {
        RefreshPanel();

    }
    public void TryPurchaseBuilding()
    {
        RefreshPanel();
        MapManager.Instance.ShowAvailableTiles();
        currentSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currentSelectedButton.interactable = false;
        cancelButton.gameObject.SetActive(true);

        MapManager.Instance.SelectedBuildingName = currentSelectedButton.name.Replace("ShopButton", "");
        MapManager.Instance.ShowAvailableTiles();
    }
    public void CancelTryPurchaseBuilding()
    {
        if (currentSelectedButton != null)
        {
            currentSelectedButton.interactable = true;
        }

        MapManager.Instance.CancleShowAvailableTiles();

    }
    public void RefreshPanel()
    {
        if (gameObject.activeSelf == false)
            return;

        Debug.Log("Refreshing Building Shop Panel . . .");

        for (int i = 0; i < 11; i++)
        {
            Button button = GameObject.Find("ShopButton" + (i + 1).ToString()).GetComponent<Button>();

            Builder bu = null;

            Building buildingData = null;



            if (button != null)
            {
                button.interactable = true;


                bu = BuildManager.Instance.AllBuildings.FirstOrDefault(b => ((int)b.Type) == int.Parse(button.name.Replace("ShopButton", "")));




                if (bu != null)
                {
                    button.transform.Find("BGPanel/ActiveAmount").GetComponent<Text>().text = bu.CurrentActiveAmount.ToString() + "/" + bu.maxActiveAmount;

                    buildingData = LoadManager.Instance.allBuildingData[bu.Type];

                    button.transform.Find("BGPanel/UpgradePoint").GetComponent<Text>().text = buildingData.upgradePoint[0].ToString();
                    button.transform.Find("Cost/Wood/CostText").GetComponent<Text>().text = ItemManager.Instance.GetbuildingCost("Wood", buildingData).ToString();
                    button.transform.Find("Cost/Stone/CostText").GetComponent<Text>().text = ItemManager.Instance.GetbuildingCost("Stone", buildingData).ToString();
                }
                else
                {
                    bu = new Builder((Building.BuildingType)(i + 1));
                    // Debug.Log(bu.maxActiveAmount);
                    button.transform.Find("BGPanel/ActiveAmount").GetComponent<Text>().text = "0 /" + bu.maxActiveAmount;

                    buildingData = LoadManager.Instance.allBuildingData[bu.Type];

                    button.transform.Find("BGPanel/UpgradePoint").GetComponent<Text>().text = buildingData.upgradePoint[0].ToString();

                    button.transform.Find("Cost/Wood/CostText").GetComponent<Text>().text = ItemManager.Instance.GetbuildingCost("Wood", buildingData).ToString();
                    button.transform.Find("Cost/Stone/CostText").GetComponent<Text>().text = ItemManager.Instance.GetbuildingCost("Stone", buildingData).ToString();
                }
            }
            else
            {
                Debug.LogWarning("Can't find Building button !");
            }

            buildingData = LoadManager.Instance.allBuildingData[bu.Type];

            if (!ItemManager.Instance.IsAffordable(buildingData.buildingCost[0]))
            {
                button.image.color = new Color(1f, 0.5f, 0.5f, 1f);
                button.interactable = false;
            }
            else
            {
                button.image.color = Color.white;
            }


        }
        for (int i = 0; i < BuildManager.Instance.AllBuildings.Count; i++)
        {
            if (BuildManager.Instance.AllBuildings[i].CurrentActiveAmount == BuildManager.Instance.AllBuildings[i].maxActiveAmount)
            {
                Button button = GameObject.Find("ShopButton" + ((int)BuildManager.Instance.AllBuildings[i].Type).ToString()).GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = false;
                }
                else
                {
                    Debug.LogWarning($"Can't find Building button : {BuildManager.Instance.AllBuildings[i].Type} !");
                }

            }


        }

    }

}
