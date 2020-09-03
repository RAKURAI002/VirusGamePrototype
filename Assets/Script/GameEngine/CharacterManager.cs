using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CharacterManager : SingletonComponent<CharacterManager>
{
    [SerializeField] private List<Character> allCharacters;
    [SerializeField] public List<Character> characterWaitingInLine;
    public List<Character> AllCharacters { get { return allCharacters; } set { allCharacters = value; } }

    float CHARACTER_ADDING_EVENT_CYCLE = 60f;

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();

    }
    protected override void OnInitialize()
    {
        allCharacters = new List<Character>();
        characterWaitingInLine = new List<Character>();

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

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
        }
        secondCalled = true;

    }

    void Start()
    {
        InvokeRepeating("CharacterJoiningEvent", CHARACTER_ADDING_EVENT_CYCLE, CHARACTER_ADDING_EVENT_CYCLE);

    }

    #endregion

    void CharacterJoiningEvent()
    {
        if (LoadManager.Instance.allCharacterData.Count <= 0)
        {
            return;

        }

        /// Event occur chance
        if (UnityEngine.Random.Range(0, 101) >= 30 / ((characterWaitingInLine.Count / 5) + 1))
        {
            return;

        }

        int index = UnityEngine.Random.Range(0, LoadManager.Instance.allCharacterData.Count);

        Debug.Log(index);
        Character.CharacterData characterData = LoadManager.Instance.allCharacterData[index];
        Character character = new Character(characterData.name, characterData.gender, characterData.spritePath); ;
        characterWaitingInLine.Add(character);
        LoadManager.Instance.allCharacterData.Remove(characterData);
        EventManager.Instance.CharacterAssigned();



    }


    public void CreateNewCharacter()
    {
        Character character = new Character("John " + Random.Range(0, 10000));
        Debug.Log("Creating New Character : " + character.ToString());
        AddCharacterToList(character);
        LoadManager.Instance.SavePlayerDataToJson();
        return;

    }


    public void AddCharacterToList(Character character)
    {
        allCharacters.Add(character);
        return;
    }

    void OnGameDataLoadFinished()
    {
        allCharacters.Where((c) => c.effects.Count > 0).ToList().ForEach(c =>
        {
            c.effects.ForEach(e => { Debug.Log($"Applying {e.name} to {c.Name}"); ApplyItemEffect(c, e, false); });
        });

    }


    public bool AssignWork(Character character, Builder builder, int team)
    {
        if (builder.Type == Building.BuildingType.TownBase && builder.CharacterInBuilding[team].Characters.Count > 0 && builder.CharacterInBuilding[team].Characters[0].workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning("Can't assign character to TownBase while doing Quest.");
            return false;
        }
        if (character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning("Questing Character can't be Assign.");
            return false;
        }
        if (builder.CharacterInBuilding[team].Characters.Count >= LoadManager.Instance.allBuildingData[builder.Type].maxCharacterStored[builder.Level].amount[team])
        {
            Debug.LogWarning("This building is Full");
            return false;
        }

        if (character.workStatus == Character.WorkStatus.Working && character.WorkingPlaceID != -1)
        {
            Builder oldBuilder = BuildManager.Instance.AllBuildings.Single(b => b.ID == character.WorkingPlaceID);
            Debug.Log($"This Character is already doing work at {oldBuilder.Type}. Try canceling old work . . .");
            CancleAssignWork(character, oldBuilder);
        }
        builder.CharacterInBuilding[team].Characters.Add(character);
        character.workStatus = Character.WorkStatus.Working;
        character.WorkingPlaceID = builder.ID;
        // Debug.Log(Object.ReferenceEquals(builder, BuildManager.Instance.AllBuildings.Single(b => b.ID == builder.ID)));
        // Debug.Log($"Now {builder.Type} has {builder.CharacterInBuilding.Count} Character(s) assigned.");
        LoadManager.Instance.SavePlayerDataToJson();
        EventManager.Instance.CharacterAssigned();

        return true;
    }
    public bool CancleAssignWork(Character character, Builder builder)
    {
        if (character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning("Questing Character can't be Unassign.");
            return false;
        }
        //  Debug.Log(builder.CharacterInBuilding.Find(cw => cw.Characters.Contains(character)).Characters.Count);
        if (!builder.CharacterInBuilding.Find(cw => cw.Characters.Contains(character)).Characters.Remove(character))  //(ec => ec.Characters.Single(c=> c.WorkingPlaceID == builder.ID)).Remove(character))
        {
            Debug.LogError("Remove Character in Building FAILED.");
            return false;
        }
        character.workStatus = Character.WorkStatus.Idle;
        character.WorkingPlaceID = -1;
        // Debug.Log(Object.ReferenceEquals(character, CharacterManager.Instance.AllCharacters.Single(c => c.Name == character.Name)));
        Debug.Log($"Stop {character.Name} from working at {builder.Type}. Now {character.Name} is {character.workStatus}");
        EventManager.Instance.CharacterAssigned();
        LoadManager.Instance.SavePlayerDataToJson();
        return true;
    }

    public void ConsumeItem(Character character, Resource item)
    {
        if (ItemManager.Instance.TryConsumeResources(item.Name, 1))
        {
            if (character.effects.Exists(e => e.name == item.effect.name))
            {
                Debug.Log("Same");
                ApplyItemEffect(character, character.effects.Single(e => e.name == item.effect.name), true);

            }
            else
            {
                character.effects.Add(ObjectCopier.Clone(item.effect));
                ApplyItemEffect(character, character.effects[character.effects.Count - 1], false);

            }



            //     Debug.Log(character.Name + $"  {item.effect.name}");


        }

    }

    void ApplyItemEffect(Character character, Resource.Effect effect, bool isAlreadySameEffectActive)
    {
        if (!isAlreadySameEffectActive)
        {
            //  Debug.Log("Applying Effect" + effect.name);
            GameObject effectTimerGO = new GameObject(character.Name + ":" + effect.name);
            effectTimerGO.transform.SetParent(CharacterManager.Instance.gameObject.transform.Find("ItemEffectTimer"));
            effectTimerGO.AddComponent<ItemEffectTimer>().StartEffect(character, effect);

        }
        else
        {

            GameObject effectTimerGO = CharacterManager.Instance.gameObject.transform.Find("ItemEffectTimer/" + effect.instanceID).gameObject;
            effectTimerGO.GetComponent<ItemEffectTimer>().IncreaseDuration(effect.duration);
            //  Debug.Log(effectTimerGO.name);

        }

    }

}
