using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Text;
/// <summary>
/// All Collectable Buildings : Farm, WaterTreatmentCenter, Mine, LumberYard.
/// </summary>
public class ResourceBuildingBehavior : BuildingBehavior
{
    Vector2 origin;
    [SerializeField] bool isCollectable;
    OnMouseOverHelper onMouseOverHelper;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (isCollectable)
        {
            EventManager.Instance.OnResourceChanged += OnResourceChanged;
            EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
            EventManager.Instance.OnActivityFinished += OnActivityFinished;

        }

    }
    protected override void OnDisable()
    {
        base.OnDisable();
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
    protected override void OnGameCycleUpdated()
    {
        base.OnGameCycleUpdated();
        AddResource();

    }
    void AddResource()
    {
        Building buildingData = LoadManager.Instance.allBuildingData[builder.Type];     
        List<Character> characters = builder.CharacterInBuilding[0].Characters;

        foreach (KeyValuePair<string, int> baseProduction in buildingData.production[builder.Level])
        {     
            Resource resource = LoadManager.Instance.allResourceData[baseProduction.Key];
            if (resource.type != Resource.ResourceType.Material)
            {
                return;

            }

            float productionAmount = GetTotalProduction(baseProduction, characters);

            AddProductionToBuilding(productionAmount);

            EventManager.Instance.ResourceChanged(baseProduction.Key);
        }
    }

    void AddResourceAfterOffine(int multipler)
    {
        Building buildingData = LoadManager.Instance.allBuildingData[builder.Type];
        List<Character> characters = builder.CharacterInBuilding[0].Characters;
        int offlineTimePassed = (int)((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond);

        foreach (KeyValuePair<string, int> baseProduction in buildingData.production[builder.Level])
        {
            Resource resource = LoadManager.Instance.allResourceData[baseProduction.Key];
            if (resource.type != Resource.ResourceType.Material)
            {
                return;

            }

            float productionAmount = GetTotalProduction(baseProduction, characters) * multipler;

            Debug.Log($"{offlineTimePassed}s passed since lst login, resulting in add {baseProduction.Key} : {baseProduction.Value}(base) * {multipler}(multipler) = {productionAmount}");

            AddProductionToBuilding(productionAmount);

             EventManager.Instance.ResourceChanged(baseProduction.Key);
        }
    }

    float GetTotalProduction(KeyValuePair<string, int> production, List<Character> characters)
    {
        StringBuilder log = new StringBuilder();
        log.AppendLine($"{Constant.TimeCycle.RESOURCE_UPDATE_CYCLE} seconds passed. Base Production of {builder.Type}[ID : {builder.ID}] is {production.Value}");

        float finalUpdatedAmount = production.Value;
        foreach (Character character in characters)
        {
            float productionAmount = character.Stats.speed * 0.2f;

            List<Character.BirthMark> birthMarks = character.BirthMarks.Where(bm => bm.type == typeof(IncreaseProductionOnBuildingBirthMark).ToString()).ToList();
            Debug.Log(string.Concat(birthMarks.Select(b => b.type.ToString())));
            if (birthMarks.Count == 0)
            {
                continue;

            }

            Debug.Log($"{birthMarks.Count}");
            List<BirthMarkData> birthMarkDatas = new List<BirthMarkData>();
            birthMarks.ForEach((bm) =>
            {
                BirthMarkData birthMarkData = LoadManager.Instance.allBirthMarkData[bm.name];
                Debug.Log($"{birthMarkData.name}");
                if (birthMarkData != null)
                {
                    birthMarkDatas.Add(ObjectCopier.Clone<BirthMarkData>(birthMarkData));
                    birthMarkDatas[birthMarkDatas.Count - 1].level = bm.level;

                }


            });

            log.AppendLine($"{character.Name} : speed = {character.Stats.speed} increases {character.Stats.speed * 0.2f}");
            Debug.Log($"{string.Concat(birthMarkDatas.Select(b => ((IncreaseProductionOnBuildingBirthMark)b).buildingType))}");
            birthMarkDatas.Where(bData => ((IncreaseProductionOnBuildingBirthMark)bData).buildingType.ToArray().Contains(builder.Type)).ToList().ForEach((bData) =>
            {
                log.AppendLine($"Affected BirthMarks are {bData.name}(Level{bData.level}) increase {productionAmount * bData.effectValues[bData.level]}");
                productionAmount += productionAmount * bData.effectValues[bData.level];

            });

            finalUpdatedAmount += productionAmount;
            log.AppendLine($"Total production : {finalUpdatedAmount}");
            Debug.Log($"{log}");


        }

        return finalUpdatedAmount;
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
        int offlineTimePassed = (int)((DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond);
        int resourceMultipler = offlineTimePassed / Constant.TimeCycle.RESOURCE_UPDATE_CYCLE;
        AddResourceAfterOffine(resourceMultipler);

    }
    void AddProductionToBuilding(float amount)
    {
        Building buildData = LoadManager.Instance.allBuildingData[builder.Type];

        builder.currentProductionAmount += amount;
        if (builder.currentProductionAmount >= buildData.maxProductionStored[builder.Level])
        {
            builder.currentProductionAmount = buildData.maxProductionStored[builder.Level];

        }
        RefreshProductionAmount();

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
