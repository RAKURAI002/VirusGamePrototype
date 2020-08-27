using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.IO;
using System;

using UnityEngine.SceneManagement;

using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Load all In-Game data and Player data to Runtime.
/// </summary>
public class LoadManager : SingletonComponent<LoadManager>
{

    [SerializeField] public PlayerData playerData { get; set; }

    [SerializeField] public BuildingDictionary allBuildingData;
    
    [SerializeField] public ResourceDictionary allResourceData;
    [SerializeField] public EquipmentDictionary allEquipmentData;
    [SerializeField] public List<Enemy> allEnemyData; /// ********************
    [SerializeField] public QuestDataDictionary allQuestData; /// ****************
    
    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnInitialize()
    {
        
        LoadPlayerDataFromJson(Application.persistentDataPath + "/PlayerData.json");
        Debug.Log("Loading Game Data . . .");
        StartCoroutine(LoadInGameData());
        
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
            Start();
        }
        if (!secondCalled)
        {
        }
        secondCalled = true;
    }
  
    void Start()
    {
    }

    void Update()
    {
    }
    #endregion

    public void SavePlayerDataToJson()
    {
        playerData.characterInPossession = CharacterManager.Instance.AllCharacters;
        playerData.resourceInPossession = ItemManager.Instance.AllResources;
        playerData.buildingInPossession = BuildManager.Instance.AllBuildings;
        playerData.equipmentInPossession = ItemManager.Instance.AllEquipments;
        playerData.currentActivities = NotificationManager.Instance.ProcessingActivies;
 //       playerData.expandedArea = GameManager.Instance.expandedArea;

        string playerDatas = JsonUtility.ToJson(playerData, true);
       // Debug.Log("Saving Data to JSON to " + Application.persistentDataPath + playerDatas);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerDatas);

    }

    void LoadPlayerDataFromJson(string file_path)
    {
        Debug.Log("Fetching Player data form JSON . . .");

        playerData = new PlayerData();

        string file = null;
        
        if (File.Exists(file_path))
        {
            file = File.ReadAllText(file_path);
            if (file != null)
            {
                playerData = JsonUtility.FromJson<PlayerData>(file);
                Debug.Log(file);
            }  
        }
        else
        {
            Debug.Log("There no file in directory. Creating new PlayerData.json on " + file_path);
            File.Create(file_path);
            playerData.completeTutorial = true;

        }

        return;
    }

    IEnumerator LoadInGameData()
    {
        List<string> gameData = new List<string>(){ "/BuildingData.json", "/QuestData.json", "/ResourceData.json", "/EquipmentData.json" };



        Debug.Log("Fetching Building Data . . .");
        Coroutine c1 = StartCoroutine(GetRequest("/BuildingData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allBuildingData = new BuildingDictionary();
               
                Building[] buildings = JsonHelper.FromJson<Building>(req.downloadHandler.text);
                foreach(Building building in buildings)
                {
                    allBuildingData.Add(building.type, building);
                }

                Debug.Log("Fetching Building Data completed.\n" + req.downloadHandler.text);
            }
        }));
        Debug.Log("Fetching Quest Data  . . .");
        Coroutine c2 = StartCoroutine(GetRequest("/QuestData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allQuestData = new QuestDataDictionary();
                QuestData[] questDatas = JsonHelper.FromJson<QuestData>(req.downloadHandler.text);
                foreach (QuestData questData in questDatas)
                {
                    allQuestData.Add(questData.questID, questData);
                }
                Debug.Log("Fetching Quest Data completed.\n" + req.downloadHandler.text);
            }
        }));
        Debug.Log("Fetching Resource Data . . .");
        Coroutine c3 = StartCoroutine(GetRequest("/ResourceData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allResourceData = new ResourceDictionary();
                Resource[] resources = JsonHelper.FromJson<Resource>(req.downloadHandler.text);
                foreach (Resource resource in resources)
                {
                    allResourceData.Add(resource.Name, resource);
                }
                Debug.Log("Fetching Resource Data completed.\n" + req.downloadHandler.text);
            }
        }));
        Debug.Log("Fetching Equipment Data . . .");
        Coroutine c4 = StartCoroutine(GetRequest("/EquipmentData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allEquipmentData = new EquipmentDictionary();
                Equipment[] equipments = JsonHelper.FromJson<Equipment>(req.downloadHandler.text);
                foreach(Equipment equipment in equipments)
                {
                    allEquipmentData.Add(equipment.Name, equipment);
                }
                Debug.Log("Fetching Equipment Data completed.\n" + req.downloadHandler.text);

            }
        }));
        Debug.Log("Fetching Enemy Data  . . .");
        Coroutine c5 = StartCoroutine(GetRequest("/EnemyData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allEnemyData = new List<Enemy>();
                Debug.Log("Fetching Enemy Data completed.\n" + req.downloadHandler.text);
                allEnemyData.AddRange(JsonHelper.FromJson<Enemy>(req.downloadHandler.text));
            }
        }));

        Debug.Log("Now Waiting . . .");

        yield return c1;

        yield return c2;

        yield return c3;

        yield return c4;

        yield return c5;

        Debug.Log("Initializing Player Data to Scene . . .");
        CharacterManager.Instance.AllCharacters = playerData.characterInPossession;
        ItemManager.Instance.AllResources = playerData.resourceInPossession;
        ItemManager.Instance.AllEquipments = playerData.equipmentInPossession;
        NotificationManager.Instance.ProcessingActivies = playerData.currentActivities;

        /// Load Player's progress to Map.
        MapManager.Instance.SetExpandedArea();
        MapManager.Instance.LoadBuildingToScene();

        Debug.Log("Load GameData Complete.");

        EventManager.Instance.GameDataLoadFinished();

        if (playerData.completeTutorial)
        {
            Debug.Log("First login detected, Starting Tutorial . . . ");
            GameManager.Instance.StartTutorial();

        }

    }
    
    IEnumerator GetRequest(string path, Action<UnityWebRequest> callback)
    {
        string finalPath = System.IO.Path.Combine(Application.streamingAssetsPath + path);
        using (UnityWebRequest request = UnityWebRequest.Get(finalPath))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();
            callback(request);
        }
    }
    IEnumerator WaitAllCoroutine(List<IEnumerator> coroutineList, Action OnComplete)
    {
        foreach (IEnumerator coroutine in coroutineList)
            yield return StartCoroutine(coroutine);
        OnComplete.Invoke();
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

}



