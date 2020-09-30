using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Linq;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif


public class GameManager : SingletonComponent<GameManager>
{

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnInitialize()
    {
        dontDestroyManager = GameObject.FindGameObjectWithTag("DontDestroyManager");
        if (dontDestroyManager == null)
        {
            Debug.LogError("Can't find dontDestroyManager");
        }
        DontDestroyOnLoad(dontDestroyManager);

        StartCoroutine(LoadManager.Instance.InitializeGameData());

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        EventManager.Instance.OnGameDataLoadFinished += OnGameDataLoadFinished;     
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if (EventManager.Instance)
            EventManager.Instance.OnGameDataLoadFinished -= OnGameDataLoadFinished;
    }

    void OnGameDataLoadFinished()
    {
        isGameDataLoaded = true;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loading {scene.name} . . .");
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
            timerCanvas.transform.position += new Vector3(0, 500, 0);
            allBuildings.transform.position += new Vector3(0, 500, 0);

        }
        if (scene.name == "WorldMap")
        {
            timerCanvas.transform.position += new Vector3(0, -500, 0);
            allBuildings.transform.position += new Vector3(0, -500, 0);
        }
    }

    GameObject timerCanvas;
    GameObject allBuildings;
    GameObject dontDestroyManager;

    public static bool isGameDataLoaded;

    void Start()
    {
        if (!secondCalled)
        {
            timerCanvas = GameObject.Find("TimerCanvas");
            allBuildings = GameObject.Find("AllBuildings");
            if (timerCanvas == null)
            {
                Debug.LogError("Can't find timerCanvas");
            }
            if (allBuildings == null)
            {
                Debug.LogError("Can't find allBuildings");
            }
            secondCalled = true;
        }

        InvokeRepeating(nameof(BroadCastGeneralGameCycle), Constant.TimeCycle.GENERAL_GAME_CYCLE, Constant.TimeCycle.GENERAL_GAME_CYCLE);

    }


    private void OnApplicationQuit()
    {
        LoadManager.Instance.playerData.lastLoginTime = DateTime.Now.Ticks;
        LoadManager.Instance.SavePlayerDataToFireBase();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            LoadManager.Instance.playerData.lastLoginTime = DateTime.Now.Ticks;
            LoadManager.Instance.SavePlayerDataToFireBase();
            Debug.Log("Application paused after " + Time.time + " seconds");
        }
    }
    #endregion

    public void ReloadGame()
    {
        Debug.Log($"Reload Game, destroying {transform.parent.gameObject.name} . . .");
        Destroy(transform.parent.gameObject);

        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// General purpose event.
    /// </summary>
    void BroadCastGeneralGameCycle()
    {
        Debug.Log($"BroadCastGeneralGameCycle Updated.");
        EventManager.Instance.GameCycleUpdated();
    }

    public IEnumerator StartTutorial()
    {
        GameTutorialManager gameTutorialManager = Resources.FindObjectsOfTypeAll<GameTutorialManager>()[0];

        Coroutine getPlayerName = StartCoroutine(gameTutorialManager.GetPlayerName());

        yield return getPlayerName;

        BuildingManager.Instance.ForceCreateBuilding(Building.BuildingType.TownBase, 0);
        BuildingManager.Instance.ForceCreateBuilding(Building.BuildingType.LaborCenter, 0);
        BuildingManager.Instance.ForceCreateBuilding(Building.BuildingType.Residence, 1);

        CharacterManager.Instance.CreateNewCharacter();
        
        EventManager.Instance.PlayerNameChanged();
        LoadManager.Instance.playerData.completeTutorial = true;
        LoadManager.Instance.SavePlayerDataToFireBase();
        GameObject.Find("MainCanvas/Fog").GetComponent<Animation>().Play("Fog_On_Start_Game");

    }

    public void ShowAccountSelector(PlayerData playerData)
    {
        Debug.Log($"ShowAccountSelector");
        AccountSelectorPanel accountSelectorPanel = Resources.FindObjectsOfTypeAll<AccountSelectorPanel>()[0];
        accountSelectorPanel.gameObject.SetActive(true);

        StartCoroutine(accountSelectorPanel.SetAccountInformation(playerData));

    }

    public void ChangeAccount(PlayerData playerData)
    {
        string currentUID = LoadManager.Instance.playerData.UID;

        if(playerData.UID != currentUID)
        {
            Debug.Log($"Changing account progress . . .");
            LoadManager.Instance.playerData = playerData;
            LoadManager.Instance.SavePlayerDataToFireBase();
            FireBaseManager.Instance.DeleteOldAccount(playerData.UID);
            ReloadGame();
        }
        else
        {
            LoadManager.Instance.playerData.UID = playerData.UID;
            LoadManager.Instance.SavePlayerDataToFireBase();
            FireBaseManager.Instance.DeleteOldAccount(playerData.UID);
        }
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public static Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

}
