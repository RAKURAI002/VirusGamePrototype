﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventManager : SingletonComponent<EventManager>
{
    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();

        }
        secondCalled = true;

    }

    #endregion


    public event Action OnCharacterAssigned;
    public event Action<string> OnResourceChanged;

    public event Action<int> OnBuildingModified;

    public event Action<ActivityInformation> OnActivityAssigned;
    public event Action<ActivityInformation> OnActivityFinished;
    public event Action<int> OnPlayerLevelUp;

    public event Action OnGameCycleUpdated;
    public event Action OnGameDataLoadFinished;

    public void BuildingModified(int id)
    {
        OnBuildingModified?.Invoke(id);
    }
    public void GameCycleUpdated()
    {
        OnGameCycleUpdated?.Invoke();

    }
    public void GameDataLoadFinished()
    {
        Debug.Log($"Load Game data finished.");
        OnGameDataLoadFinished?.Invoke();

    }
    public void ResourceChanged(string resourceName)
    {
        OnResourceChanged?.Invoke(resourceName);

    }

    public void PlayerLevelUp(int level)
    {
        OnPlayerLevelUp?.Invoke(level);

    }

    public void ActivityAssigned(ActivityInformation activityInformation)
    {  
        OnActivityAssigned?.Invoke(activityInformation);

    }

    public void ActivityFinished(ActivityInformation activityInformation)
    {
        OnActivityFinished?.Invoke(activityInformation);

    }

    public void CharacterAssigned()
    {
        OnCharacterAssigned?.Invoke();

    }

}
