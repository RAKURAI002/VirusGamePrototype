using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Linq;

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
        if(EventManager.Instance)
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
            timerCanvas.SetActive(true);
            allBuildings.SetActive(true);

        }
        if (scene.name == "WorldMap")
        {
            timerCanvas.SetActive(false);
            allBuildings.SetActive(false);
        }

    }

    GameObject timerCanvas;
    GameObject allBuildings;
    GameObject editBuildingPanel;
    GameObject dontDestroyManager;

    public bool isGameDataLoaded;



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

        editBuildingPanel = Resources.FindObjectsOfTypeAll<BuildingInformationCanvas>()[0].gameObject;
        if (editBuildingPanel == null)
        {
            Debug.LogError("Can't find editBuildingPanel");
        }

    }
    void Update()
    {

    }
    public void AddLevelTest()
    {
        LoadManager.Instance.playerData.level++;
        Debug.Log($"Now Player Level is {LoadManager.Instance.playerData.level}.");
        EventManager.Instance.PlayerLevelUp(LoadManager.Instance.playerData.level);
        LoadManager.Instance.SavePlayerDataToJson();
    }
    private void OnApplicationQuit()
    {
        LoadManager.Instance.playerData.lastLoginTime = DateTime.Now.Ticks;
        LoadManager.Instance.SavePlayerDataToJson();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    #endregion

    public StringBuilder gameLog;
    
    public void StartTutorial()
    {
        /// Add First default Character
        CharacterManager.Instance.CreateNewCharacter();

        /// Add First default Buildings : TownBase, LaborCenter, Ressidence 
        BuildManager.Instance.ForceCreateBuilding(Building.BuildingType.TownBase, 0);
        BuildManager.Instance.ForceCreateBuilding(Building.BuildingType.LaborCenter, 0);

        BuildManager.Instance.ForceCreateBuilding(Building.BuildingType.Residence, 1);

        LoadManager.Instance.playerData.completeTutorial = true;

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
