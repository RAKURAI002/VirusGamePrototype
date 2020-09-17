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
    protected Building buildingData;
    protected virtual void OnEnable()
    {
        EventManager.Instance.OnGameCycleUpdated += OnGameCycleUpdated;
    }
    protected virtual void OnDisable()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.OnGameCycleUpdated -= OnGameCycleUpdated;
        }
    }
    protected virtual void OnGameCycleUpdated()
    {

    }

    public void SetBuilder(Builder _builder)
    {
        builder = _builder;
        buildingData = LoadManager.Instance.allBuildingData[builder.Type];
        Initialize();
        ContinueFromOffline();
    }
    protected virtual void ContinueFromOffline()
    {
    }

    protected virtual void Initialize()
    {
        if (builder == null)
        {
            return;

        }     
        UpdatePrefab();

    }

    public void UpdatePrefab()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(buildingData.spritePath[builder.Level]);
        Destroy(GetComponent<BoxCollider2D>() ?? null);
        gameObject.AddComponent<BoxCollider2D>();

    }
}
