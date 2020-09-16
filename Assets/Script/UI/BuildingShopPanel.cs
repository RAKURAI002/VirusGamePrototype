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
    bool isInitialize;

    private void Awake()
    {
        isInitialize = false;

    }
    private void OnEnable()
    {
        EventManager.Instance.OnResourceChanged += OnResourceChanged;
        InitializeShopData();
        RefreshPanel();
    }
    private void OnDisable()
    {
        MainCanvas.FreezeCamera = false;
        if (EventManager.Instance)
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {

            MainCanvas.FreezeCamera = true;
        }
        else
        {
            MainCanvas.FreezeCamera = false;
        }
    }
    void InitializeShopData()
    {

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

        for (int i = 1; i < Enum.GetNames(typeof(Building.BuildingType)).Length; i++)
        {

            Button button = transform.Find("Container/ShopButton" + (i).ToString()).GetComponent<Button>();
            Builder builder = null;

            if (button != null)
            {
                button.interactable = true;

                builder = BuildingManager.Instance.AllBuildings.FirstOrDefault(b => ((int)b.Type) == int.Parse(button.name.Replace("ShopButton", "")));

                if (builder != null)
                {
                    button.transform.Find("BGPanel/ActiveAmount").GetComponent<Text>().text = builder.CurrentActiveAmount.ToString() + "/" + builder.maxActiveAmount;

                }
                else
                {
                    builder = new Builder((Building.BuildingType)(i));
                    button.transform.Find("BGPanel/ActiveAmount").GetComponent<Text>().text = "0 /" + builder.maxActiveAmount;

                }

            }
            else
            {
                Debug.LogWarning($"Can't find Building button {i} !");

            }

            Building buildingData = LoadManager.Instance.allBuildingData[builder.Type];

            if(!isInitialize)
            {
                button.transform.Find("BGPanel/UpgradePoint").GetComponent<Text>().text = buildingData.upgradePoint[0].ToString();
                button.transform.Find("Cost/Wood/CostText").GetComponent<Text>().text = buildingData.buildingCost[0]["Wood"].ToString();
                button.transform.Find("Cost/Stone/CostText").GetComponent<Text>().text = buildingData.buildingCost[0]["Stone"].ToString();
                
            }

            if (!ItemManager.Instance.IsAffordable(buildingData.buildingCost[0]))
            {
                button.image.color = new Color(1f, 0.5f, 0.5f, 1f);
                button.interactable = false;

            }
            else
            {
                button.image.color = Color.white;

            }

        } /// End of all Button loop.
        isInitialize = true;

        for (int i = 0; i < BuildingManager.Instance.AllBuildings.Count; i++)
        {
            if (BuildingManager.Instance.AllBuildings[i].CurrentActiveAmount == BuildingManager.Instance.AllBuildings[i].maxActiveAmount)
            {
                Button button = GameObject.Find("ShopButton" + ((int)BuildingManager.Instance.AllBuildings[i].Type).ToString()).GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = false;

                }
                else
                {
                    Debug.LogWarning($"Can't find Building button : {BuildingManager.Instance.AllBuildings[i].Type} !");

                }

            }

        }

    }

}
