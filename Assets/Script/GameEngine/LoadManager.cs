using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using UnityEngine.Assertions;
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
    [SerializeField] public List<Character.CharacterData> allCharacterData;
    [SerializeField] public BirthMarkDataDictionary allBirthMarkDatas;
    [SerializeField] public List<AchievementData> allAchievementDatas;

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnInitialize()
    {

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
    public IEnumerator InitializeGameData()
    {
        Debug.Log("Loading Player Data . . .");
        LoadPlayerDataFromJson(Application.persistentDataPath + "/PlayerData.json");

        Debug.Log("Loading Game Data . . .");
        Coroutine LoadGameDataCoroutine = StartCoroutine(LoadInGameData());
        

        yield return LoadGameDataCoroutine;

        LoadPlayerDataToScene();

        CheckFirstLogin();


    }

    public void SavePlayerDataToJson()
    {
        playerData.characterInPossession = CharacterManager.Instance.AllCharacters;
        playerData.resourceInPossession = ItemManager.Instance.AllResources;
        playerData.buildingInPossession = BuildManager.Instance.AllBuildings;
        playerData.equipmentInPossession = ItemManager.Instance.AllEquipments;
        playerData.currentActivities = NotificationManager.Instance.ProcessingActivies;
        playerData.characterWaitingInLine = CharacterManager.Instance.characterWaitingInLine;

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
            playerData.completeTutorial = false;

        }

        return;
    }

    public IEnumerator LoadInGameData()
    {
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
                foreach (Building building in buildings)
                {
                    allBuildingData.Add(building.type, building);
                }

                Debug.Log("Fetching Building Data completed.\n");

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
                Debug.Log("Fetching Quest Data completed.\n");
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
                Debug.Log("Fetching Resource Data completed.\n");
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
                foreach (Equipment equipment in equipments)
                {
                    allEquipmentData.Add(equipment.Name, equipment);
                }
                Debug.Log("Fetching Equipment Data completed.\n");

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
                Debug.Log("Fetching Enemy Data completed.\n");
                allEnemyData.AddRange(JsonHelper.FromJson<Enemy>(req.downloadHandler.text));
            }
        }));
        Coroutine c6 = StartCoroutine(LoadManager.Instance.GetRequest("/CharacterData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allCharacterData = new List<Character.CharacterData>();
                Debug.Log("Fetching Character Data completed.\n");
                allCharacterData.AddRange(JsonHelper.FromJson<Character.CharacterData>(req.downloadHandler.text));

            }
        }));
        Coroutine c7 = StartCoroutine(LoadManager.Instance.GetRequest("/BirthMarkData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allBirthMarkDatas = new BirthMarkDataDictionary();
                Debug.Log("Fetching Character BirthMark Data completed.\n");

                BirthMarkSerializer birthMarkSerializer = new BirthMarkSerializer();

                birthMarkSerializer = JsonUtility.FromJson<BirthMarkSerializer>(req.downloadHandler.text);

                foreach (var birthMarkData in birthMarkSerializer.birthMarkDatas)
                {
                    allBirthMarkDatas.Add(birthMarkData.name, birthMarkData);
                }

            }
        }));
        Coroutine c8 = StartCoroutine(LoadManager.Instance.GetRequest("/AchievementData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allAchievementDatas = new List<AchievementData>();
                Debug.Log("Fetching Character Achievement Datas completed.\n");
                allAchievementDatas = JsonHelper.FromJson<AchievementData>(req.downloadHandler.text).ToList();
            }
        }));

        Debug.Log("Now Waiting . . .");

        yield return c1;

        yield return c2;

        yield return c3;

        yield return c4;

        yield return c5;

        yield return c6;

        yield return c7;

        yield return c8;

        Debug.Log("Load GameData Complete.");

    }

    void LoadPlayerDataToScene()
    {
        Debug.Log("Initializing Player Data to Scene . . .");
        CharacterManager.Instance.AllCharacters = playerData.characterInPossession;
        ItemManager.Instance.AllResources = playerData.resourceInPossession;
        ItemManager.Instance.AllEquipments = playerData.equipmentInPossession;
        NotificationManager.Instance.ProcessingActivies = playerData.currentActivities;

        CharacterManager.Instance.characterWaitingInLine = playerData.characterWaitingInLine;

        /// Load Player's progress to Map.
        MapManager.Instance.SetExpandedArea();
        MapManager.Instance.LoadBuildingToScene();
        RemoveDuplicateCharacterData();

        EventManager.Instance.GameDataLoadFinished();
        
    }
    void RemoveDuplicateCharacterData()
    {
        foreach (Character character in CharacterManager.Instance.AllCharacters)
        {
            Character.CharacterData cData = LoadManager.Instance.allCharacterData.SingleOrDefault(c => c.name == character.Name);
            if (cData != null)
            {
                LoadManager.Instance.allCharacterData.Remove(cData);

            }

        }
        foreach (Character character in CharacterManager.Instance.characterWaitingInLine)
        {

            Character.CharacterData[] cData = LoadManager.Instance.allCharacterData.Where(c => c.name == character.Name).ToArray();
   
   
            if (cData.Length != 0)
            {
                LoadManager.Instance.allCharacterData.Remove(cData[0]);

            }

        }
    }

    void CheckFirstLogin()
    {
        if (!playerData.completeTutorial)
        {
            Debug.Log("First login detected, Starting Tutorial . . . ");
            GameManager.Instance.StartTutorial();

        }
    }
   
    public IEnumerator GetRequest(string path, Action<UnityWebRequest> callback)
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



