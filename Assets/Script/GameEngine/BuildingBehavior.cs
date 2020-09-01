using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class BuildingBehavior : MonoBehaviour
{
    [SerializeField] public Builder builder;
    Building buildingData;
    Vector2 origin;

    OnMouseOverHelper onMouseOverHelper;

    public void SetBuilder(Builder _builder)
    {
        builder = _builder;
        buildingData = LoadManager.Instance.allBuildingData[builder.Type];
        Initialize();
        
    }

    private void OnEnable()
    {
        if (buildingData.productionSpritePath != "")
        {
            EventManager.Instance.OnResourceChanged += OnResourceChanged;
        }

    }
    private void OnDisable()
    {
        if (EventManager.Instance && buildingData.productionSpritePath != "")
        {
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
            EventManager.Instance.OnActivityAssigned += OnActivityAssigned;
            EventManager.Instance.OnActivityFinished += OnActivityFinished;

        }

    }

    private void Awake()
    {

    }

    void OnActivityFinished(ActivityInformation activityInformation)
    {
        if(activityInformation.activityType == ActivityType.Build)
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
    void OnClickCollectResource()
    {

        ItemManager.Instance.AddResource(buildingData.production[builder.Level].First().Key , Mathf.FloorToInt(builder.currentProductionAmount));
        builder.currentProductionAmount -= Mathf.FloorToInt(builder.currentProductionAmount);
        RefreshProductionAmount();

    }
    private void OnMouseOver()
    {


    }

    private void OnMouseExit()
    {

    }

    void OnResourceChanged(string name)
    {
        RefreshProductionAmount();

    }

    void Initialize()
    {
        if (builder == null)
        {
            return;

        }
        
        ChangePrefab();

       
        
        

        AdjustNewCollider();

        if (buildingData.productionSpritePath != "")
        {
            onMouseOverHelper = transform.Find("Production").GetComponent<OnMouseOverHelper>();
            onMouseOverHelper.SetOnClickCallBack(OnClickCollectResource);

            Transform production = transform.Find("Production");

            Sprite productionSprite = Resources.Load<Sprite>(buildingData.productionSpritePath);
           // Color color = productionSprite.texture.GetPixel(productionSprite.texture.width / 2, productionSprite.texture.height / 2);

            SpriteRenderer fillSpriteRenderer = transform.Find("Production/FillSprite").GetComponent<SpriteRenderer>();

            Sprite fillSprite = Resources.Load<Sprite>("Sprites/UI/Solid");
            //  fillSprite.texture = ScaleTexture(fillSprite.texture, (int)productionSprite.rect.width, (int)productionSprite.rect.height);

            fillSpriteRenderer.sprite = fillSprite;
            fillSpriteRenderer.color = new Color(1f, 1f, 1f, 0.6f);

            production.GetComponent<SpriteMask>().sprite = productionSprite;

            SpriteRenderer parentSpriteRenderer = production.GetComponent<SpriteRenderer>();
            parentSpriteRenderer.sprite = productionSprite;


            RectTransform parentRect = production.GetComponent<RectTransform>();
            RectTransform fillRect = production.Find("FillSprite").GetComponent<RectTransform>();
            //  Debug.Log(productionSprite.rect.width.ToString()+ " "+ productionSprite.bounds.size.y);
            //  Debug.Log(parentSpriteRenderer.size.x + " : " + parentSpriteRenderer.bounds.size.y);
            parentRect.sizeDelta = new Vector2(productionSprite.bounds.size.x, productionSprite.bounds.size.y);
            //fillRect.sizeDelta = new Vector2(productionSprite.bounds.size.x, productionSprite.bounds.size.y);
            // Debug.Log(fillSpriteRenderer.size);
            //  fillSpriteRenderer.size = new Vector2(fillSpriteRenderer.size.x, productionSprite.bounds.size.y);
            //  fillRect.localPosition = Vector2.zero;
            Debug.Log(parentRect.sizeDelta.y + " : " + fillRect.sizeDelta.y);
            production.GetComponent<SpriteMask>().sprite = productionSprite;
            production.GetComponent<SpriteRenderer>().sprite = productionSprite;

            float newYScale = parentRect.sizeDelta.y / (fillRect.sizeDelta.y);// productionSprite.bounds.size.y / ((productionSprite.bounds.size.y / 2) - (fillSprite.bounds.size.y / 2));
            Debug.Log($"{parentRect.sizeDelta.y} / {fillRect.sizeDelta.y} = {newYScale}");

            fillRect.localScale = new Vector3(2f, newYScale, 0f);
            transform.Find("Production").gameObject.AddComponent<PolygonCollider2D>();
            origin = fillRect.localPosition;
            RefreshProductionAmount();
            

        }
        else
        {

        }


    }

    void ChangePrefab()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(buildingData.spritePath[builder.Level]);


    }
    void AdjustNewCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

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
            Debug.Log($"{fillScale} {origin.y} {parentRect.sizeDelta.y}");
            float newY = origin.y + (parentRect.sizeDelta.y * fillScale);
            // (fillRect.rect.height / 2 - parentRect.rect.height /2) + parentRect.localPosition.y + (parentRect.rect.height * fillScale);// (parentRect.rect.height * fillScale) + ((parentRect.rect.height / 2f) + (fillRect.rect.height / 2f));

            //  Debug.Log((parentRect.localPosition.y - parentRect.rect.height / 2) + fillRect.rect.height / 2);
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
        if(buildingData.productionSpritePath != "")
        {
            transform.Find("Production/Amount").gameObject.SetActive(onMouseOverHelper.isMouseOverObject);

        }

    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;

    }



}
