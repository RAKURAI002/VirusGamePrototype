using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System.Threading.Tasks;

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
    [SerializeField] public CharacterDataDictionary allCharacterData;
    [SerializeField] public BirthMarkDataDictionary allBirthMarkData;
    [SerializeField] public List<AchievementData> allAchievementData;

    #region Unity Functions

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        Application.quitting += OnApplicationTryToQuit;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        Application.quitting -= OnApplicationTryToQuit;
    }

    private void OnApplicationTryToQuit()
    {
        SavePlayerDataToFireBase();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
            GameObject.Find("MainCanvas/Fog").GetComponent<Animation>().Play();
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
        Debug.Log("Loading Game Data . . .");
        Coroutine LoadGameDataCoroutine = StartCoroutine(LoadInGameData());

        Debug.Log("Loading Player Data . . .");
        Coroutine LoadPlayerDataCoroutine = StartCoroutine(LoadPlayerData());
       
        yield return LoadGameDataCoroutine;
        yield return LoadPlayerDataCoroutine;

        LoadPlayerDataToScene();

        CheckFirstLogin();

    }

    public void SavePlayerDataToFireBase()
    {
        playerData.characterInPossession = CharacterManager.Instance.AllCharacters;
        playerData.resourceInPossession = ItemManager.Instance.AllResources;
        playerData.buildingInPossession = BuildingManager.Instance.AllBuildings;
        playerData.equipmentInPossession = ItemManager.Instance.AllEquipments;
        playerData.currentActivities = NotificationManager.Instance.ProcessingActivies;
        playerData.characterWaitingInLine = CharacterManager.Instance.characterWaitingInLine;

        string playerDataJson = JsonUtility.ToJson(playerData, true);
        // Debug.Log("Saving Data to JSON to " + Application.persistentDataPath + playerDatas);

        FireBaseManager.Instance.SendData(playerDataJson);
       // System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerDataJson);

    }

    IEnumerator LoadPlayerData()
    {
        Debug.Log("Fetching Player data from FireBase . . .");

        playerData = new PlayerData();
        
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.Log("No FireBaseUser detected, trying sign-in as guest.");
            var task = FireBaseManager.Instance.SignInAsGuest().ContinueWith(user =>
            {
                Debug.Log($"Sign-in successfully");
            });

            yield return new WaitUntil(() => task.IsCompleted);

            playerData.completeTutorial = false;
            playerData.UID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            string playerDatas = JsonUtility.ToJson(playerData, true);

            FireBaseManager.Instance.SendData(playerDatas);
        }
        else
        {
            string playerDatas = JsonUtility.ToJson(playerData, true);

            var ds = FireBaseManager.Instance.ReceivePlayerData();

            yield return new WaitUntil(() => ds.IsCompleted);
            Debug.Log($"{ds.Status}");
            Debug.Log($"Datasnapshot : {ds.Result.GetRawJsonValue()}");

            playerData = JsonUtility.FromJson<PlayerData>(ds.Result.GetRawJsonValue());
        }
        
        Debug.Log($"Currently working on Firebase User : {FirebaseAuth.DefaultInstance.CurrentUser?.UserId}");
    }

    public IEnumerator LoadInGameData()
    {
        Debug.Log("Fetching Building data . . .");
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

        Debug.Log("Fetching Quests' data  . . .");
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
        
        Debug.Log("Fetching Resources' data . . .");
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
        
        Debug.Log("Fetching Equipments' data . . .");
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
        
        Debug.Log("Fetching Enemies' data  . . .");
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

        Debug.Log("Fetching Characters' data  . . .");
        Coroutine c6 = StartCoroutine(LoadManager.Instance.GetRequest("/CharacterData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allCharacterData = new CharacterDataDictionary();
                Debug.Log("Fetching Character Data completed.\n");
                var tempData = (JsonHelper.FromJson<Character.CharacterData>(req.downloadHandler.text));
                foreach (var item in tempData)
                {
                    allCharacterData.Add(item.name, item);
                }     
            }
        }));

        Debug.Log("Fetching BirthMarks' data  . . .");
        Coroutine c7 = StartCoroutine(LoadManager.Instance.GetRequest("/BirthMarkData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allBirthMarkData = new BirthMarkDataDictionary();
                Debug.Log("Fetching Character BirthMark Data completed.\n");

                BirthMarkSerializer birthMarkSerializer = new BirthMarkSerializer();

                birthMarkSerializer = JsonUtility.FromJson<BirthMarkSerializer>(req.downloadHandler.text);

                foreach (var birthMarkData in birthMarkSerializer.birthMarkDatas)
                {
                    allBirthMarkData.Add(birthMarkData.name, birthMarkData);
                }

            }
        }));

        Debug.Log("Fetching Achievements' data  . . .");
        Coroutine c8 = StartCoroutine(LoadManager.Instance.GetRequest("/AchievementData.json", (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                allAchievementData = new List<AchievementData>();
                Debug.Log("Fetching Character Achievement Datas completed.\n");
                allAchievementData = JsonHelper.FromJson<AchievementData>(req.downloadHandler.text).ToList();
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
            if (LoadManager.Instance.allCharacterData.ContainsKey(character.Name))
            {
                LoadManager.Instance.allCharacterData.Remove(character.Name);
            }
        }

        foreach (Character character in CharacterManager.Instance.characterWaitingInLine)
        {
            if (LoadManager.Instance.allCharacterData.ContainsKey(character.Name))
            {
                LoadManager.Instance.allCharacterData.Remove(character.Name);
            }
        }
    }

    void CheckFirstLogin()
    {
        if (!playerData.completeTutorial)
        {
            Debug.Log("First login detected, Starting Tutorial . . . ");
            StartCoroutine(GameManager.Instance.StartTutorial());
        }
        else
        {
            GameObject.Find("MainCanvas/Fog").GetComponent<Animation>().Play();
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



