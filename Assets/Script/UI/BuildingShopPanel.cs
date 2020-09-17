using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingShopPanel : MonoBehaviour
{
    public GameObject uiCanvas;
    public Button cancleButton;
    Button currentSelectedButton;
    bool isInitialize;

    private void Awake()
    {
        isInitialize = false;
        CreateShopMenu();
    }
    private void OnEnable()
    {
        EventManager.Instance.OnResourceChanged += OnResourceChanged;
        transform.Find("BackGroundPanel").gameObject.SetActive(true);
        cancleButton.gameObject.SetActive(false);
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
    void CreateShopMenu()
    {
        Transform container = transform.Find("BackGroundPanel/Container");

        List<Building> buildingsShowInShop = LoadManager.Instance.allBuildingData.Select(b => b.Value).ToList();

        buildingsShowInShop.Remove(LoadManager.Instance.allBuildingData[Building.BuildingType.TownBase]);
        buildingsShowInShop.Remove(LoadManager.Instance.allBuildingData[Building.BuildingType.LaborCenter]);

        Builder laborCenter = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter);
        Building laborCenterData = LoadManager.Instance.allBuildingData[Building.BuildingType.LaborCenter];
        int baseConstructingPoint = laborCenterData.production[laborCenter.Level]["Production"];

        foreach (var buildingData in buildingsShowInShop)
        {
            GameObject shopMenu = Instantiate(Resources.Load("Prefabs/UI/ShopButtonPrefab") as GameObject, container);
            shopMenu.name = $"{buildingData.type}:ShopButton";

            shopMenu.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(buildingData.spritePath[1]);
            shopMenu.transform.Find("Name").GetComponent<Text>().text = buildingData.type.ToString();
            shopMenu.transform.Find("Information/UpgradePoint").GetComponent<Text>().text = GetFormattedDuration(buildingData.upgradePoint[0] / baseConstructingPoint);
            shopMenu.transform.Find("Cost/Wood/CostText").GetComponent<Text>().text = buildingData.buildingCost[0]["Wood"].ToString();
            shopMenu.transform.Find("Cost/Stone/CostText").GetComponent<Text>().text = buildingData.buildingCost[0]["Stone"].ToString();

            shopMenu.GetComponent<Button>().onClick.AddListener(TryPurchaseBuilding);

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

        cancleButton.gameObject.SetActive(true);

        GameObject.Find("UICanvas").SetActive(false);
        transform.Find("BackGroundPanel").gameObject.SetActive(false);
        MapManager.Instance.SelectedBuildingName = currentSelectedButton.name.Replace(":ShopButton", "");

    }

    /// Cancle Button
    public void CancelTryPurchaseBuilding()
    {
        cancleButton.gameObject.SetActive(false);
        uiCanvas.SetActive(true);
        transform.Find("BackGroundPanel").gameObject.SetActive(true);
        MapManager.Instance.CancleShowAvailableTiles();

    }
    public void ExitButton()
    {
        CancelTryPurchaseBuilding();
        gameObject.SetActive(false);

    }
    public void RefreshPanel()
    {
        if (gameObject.activeSelf == false)
            return;

        Debug.Log("Refreshing Building Shop Panel . . .");

        Transform container = transform.Find("BackGroundPanel/Container");

        foreach(Transform buttonTranform in container)
        {
            Button button = buttonTranform.GetComponent<Button>();

            button.interactable = true;
            Building.BuildingType type = (Building.BuildingType)Enum.Parse(typeof(Building.BuildingType), button.name.Replace(":ShopButton", ""));
            Builder builder = BuildingManager.Instance.AllBuildings.FirstOrDefault( b => b.Type == type);
            Building buildingData = LoadManager.Instance.allBuildingData[type];

            if (builder != default(Builder))
            {
                button.transform.Find("Information/Amount").GetComponent<Text>().text = $"{builder.CurrentActiveAmount}/{builder.maxActiveAmount}";
                if(builder.CurrentActiveAmount == builder.maxActiveAmount)
                {
                    button.interactable = false;
                }
            }
            else
            {
                button.transform.Find("Information/Amount").GetComponent<Text>().text = $"0/{buildingData.maxActiveAmount}";
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

        }

    }
    string GetFormattedDuration(int duration)
    {
        int hours = Mathf.FloorToInt(duration / 3600);
        int minutes = Mathf.FloorToInt(duration % 3600 / 60);
        int seconds = Mathf.FloorToInt(duration % 3600 % 60f);

        return $"{(hours > 0 ? hours + "H " : "")}{(minutes > 0 ? minutes + "M " : "")}{(seconds > 0 ? seconds + "s " : "")}";

    }

}
