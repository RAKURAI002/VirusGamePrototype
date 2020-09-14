using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// All Collectable Buildings : Farm, WaterTreatmentCenter, Mine, LumberYard.
/// </summary>
public class ResourceBuildingBehavior : BuildingBehavior
{
    Vector2 origin;
    [SerializeField] bool isCollectable;
    OnMouseOverHelper onMouseOverHelper;

    private void OnEnable()
    {
        if (isCollectable)
        {
            EventManager.Instance.OnResourceChanged += OnResourceChanged;
            EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
            EventManager.Instance.OnActivityFinished += OnActivityFinished;

        }

    }
    private void OnDisable()
    {
        if (EventManager.Instance && isCollectable)
        {
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
            EventManager.Instance.OnActivityAssigned -= OnActivityAssigned;
            EventManager.Instance.OnActivityFinished -= OnActivityFinished;

        }

    }
    protected override void Initialize()
    {
        base.Initialize();

        isCollectable = !string.IsNullOrEmpty(buildingData.productionSpritePath);

        if (isCollectable)
        {
            CreateSpriteFillMode();
            RefreshProductionAmount();

        }

    }
    void OnActivityFinished(ActivityInformation activityInformation)
    {
        if (activityInformation.activityType == ActivityType.Build)
        {
            RefreshProductionAmount();
        }

    }

    void OnActivityAssigned(ActivityInformation activityInformation)
    {
        if (activityInformation.activityType == ActivityType.Build)
        {
            RefreshProductionAmount();

        }

    }
    protected override void ContinueFromOffline()
    {
        base.ContinueFromOffline();
        long offlineTimePassed = (DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond;


    }
    void OnClickCollectResource()
    {
        ItemManager.Instance.AddResource(buildingData.production[builder.Level].First().Key, Mathf.FloorToInt(builder.currentProductionAmount));
        builder.currentProductionAmount -= Mathf.FloorToInt(builder.currentProductionAmount);
        RefreshProductionAmount();

    }

    void OnResourceChanged(string name)
    {
        RefreshProductionAmount();

    }

    void CreateSpriteFillMode()
    {
        Transform production = transform.Find("Production");

        onMouseOverHelper = production.GetComponent<OnMouseOverHelper>();
        onMouseOverHelper.SetOnClickCallBack(OnClickCollectResource);

        Sprite productionSprite = Resources.Load<Sprite>(buildingData.productionSpritePath);

        SpriteRenderer fillSpriteRenderer = transform.Find("Production/FillSprite").GetComponent<SpriteRenderer>();

        Sprite fillSprite = Resources.Load<Sprite>("Sprites/UI/Solid");

        fillSpriteRenderer.sprite = fillSprite;
        fillSpriteRenderer.color = new Color(1f, 1f, 1f, 0.6f);

        production.GetComponent<SpriteMask>().sprite = productionSprite;

        SpriteRenderer parentSpriteRenderer = production.GetComponent<SpriteRenderer>();
        parentSpriteRenderer.sprite = productionSprite;

        RectTransform parentRect = production.GetComponent<RectTransform>();
        RectTransform fillRect = production.Find("FillSprite").GetComponent<RectTransform>();

        parentRect.sizeDelta = new Vector2(productionSprite.bounds.size.x, productionSprite.bounds.size.y);


        production.GetComponent<SpriteRenderer>().sprite = productionSprite;

        float newYScale = parentRect.sizeDelta.y / (fillRect.sizeDelta.y);
        fillRect.localScale = new Vector3(2f, newYScale, 0f);
        production.gameObject.AddComponent<PolygonCollider2D>();
        origin = fillRect.localPosition;
    }


    void RefreshProductionAmount()
    {
        Transform production = transform.Find("Production");
        Transform amount = transform.Find("Production/Amount");
        if (builder.Level != 0 && !builder.constructionStatus.isConstructing)
        {

            production.gameObject.SetActive(true);
            amount.GetComponent<TextMeshPro>().text = $"{Mathf.FloorToInt(builder.currentProductionAmount)} / {buildingData.maxProductionStored[builder.Level]}";

            float fillScale = (builder.currentProductionAmount) / buildingData.maxProductionStored[builder.Level];

            RectTransform fillRect = production.Find("FillSprite").GetComponent<RectTransform>();
            RectTransform parentRect = production.GetComponent<RectTransform>();

            float newY = origin.y + (parentRect.sizeDelta.y * fillScale);

            fillRect.localPosition = new Vector2(fillRect.localPosition.x, newY);


        }
        else
        {
            production.gameObject.SetActive(false);

        }

        return;
    }
    void Update()
    {
        if (isCollectable)
        {
            transform.Find("Production/Amount").gameObject.SetActive(onMouseOverHelper.isMouseOverObject);

        }

    }
}
