﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class CharacterManager : SingletonComponent<CharacterManager>
{
    [SerializeField] private List<Character> allCharacters;
    [SerializeField] public List<Character> characterWaitingInLine;

    [SerializeField] public DeadCharacterDictionary allDeadcharacter;
    public int MaxCharacterInGame { get; set; } = 1;
    public List<Character> AllCharacters { get { return allCharacters; } set { allCharacters = value; } }

    #region Unity Functions
    protected override void OnInitialize()
    {
        allCharacters = new List<Character>();
        characterWaitingInLine = new List<Character>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

        EventManager.Instance.OnGameDataLoadFinished += OnGameDataLoadFinished;
        EventManager.Instance.OnActivityFinished += OnActivityFinished;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;

        if (EventManager.Instance)
        {
            EventManager.Instance.OnActivityFinished += OnActivityFinished;
            EventManager.Instance.OnGameDataLoadFinished -= OnGameDataLoadFinished;
        }
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
        InvokeRepeating(nameof(CharacterJoiningEvent), Constant.TimeCycle.CHARACTER_ADDING_EVENT_CYCLE, Constant.TimeCycle.CHARACTER_ADDING_EVENT_CYCLE);
    }

    #endregion
    void OnActivityFinished(ActivityInformation activityInformation)
    {
        if(activityInformation.activityType == ActivityType.Build)
        {
            if(BuildingManager.Instance.AllBuildings.SingleOrDefault(b => 
            b.ID == activityInformation.informationID).Type != Building.BuildingType.Residence)
            {
                return;
            }

            List<Builder> allResidences = BuildingManager.Instance.AllBuildings.Where(b 
                => b.Type == Building.BuildingType.Residence).ToList();
            Building residenceData = LoadManager.Instance.allBuildingData[Building.BuildingType.Residence];

            int maxCharacter = 0;
            foreach (Builder residence in allResidences)
            {
                maxCharacter += residenceData.production[residence.Level]["MaxCharacterStored"];
            }
            MaxCharacterInGame = maxCharacter;
        }
    }

    void CharacterJoiningEvent()
    {
        Debug.Log($"{Constant.TimeCycle.CHARACTER_ADDING_EVENT_CYCLE}s passed, Trying to start CharacterJoiningEvent.");
        if (LoadManager.Instance.allCharacterData.Count <= 0)
        {
            return;
        }

        int random = UnityEngine.Random.Range(0, 100);
        int joiningChance = Constant.CalculatingFunction.CharacterJoiningChanceCalculation(characterWaitingInLine.Count);
        Debug.Log($"Chance : 0 - {joiningChance}, Randomed : {random}. Start event = {!(random > joiningChance)}");
        if (random > joiningChance)
        {
            return;
        }
        CreateWaitingCharacter();
    }

    void CreateWaitingCharacter()
    {
        int index = UnityEngine.Random.Range(0, LoadManager.Instance.allCharacterData.Count);

        Character.CharacterData characterData = LoadManager.Instance.allCharacterData.ElementAt(index).Value;
        Character character = new Character(characterData.name, characterData.gender, characterData.spritePath); ;
        characterWaitingInLine.Add(character);
        LoadManager.Instance.allCharacterData.Remove(characterData.name);
        EventManager.Instance.CharacterAssigned();

    }

    public void CreateNewCharacter()
    {
        Character character = new Character("John " + UnityEngine.Random.Range(0, 10000));
        Debug.Log("Creating New Character : " + character.ToString());
        AddCharacterToList(character);
        LoadManager.Instance.SavePlayerDataToFireBase();
        return;

    }
    public void CreateChildCharacter(Character father, Character mother)
    {
        int index = UnityEngine.Random.Range(0, LoadManager.Instance.allCharacterData.Count);

        Character.CharacterData characterData = LoadManager.Instance.allCharacterData.ElementAt(index).Value;
        Character character = new Character(characterData.name, characterData.gender, characterData.spritePath); ;
        character.workStatus = Character.WorkStatus.Minor;

        character.InheritStars(father, mother);

        AddCharacterToList(character);
        LoadManager.Instance.allCharacterData.Remove(characterData.name);
        EventManager.Instance.CharacterAssigned();
        NotificationManager.Instance.AddActivity(new ActivityInformation() 
        {
            activityName = ($"Growing:{character.Name}"),
            activityType = ActivityType.CharacterGrowing,
            startPoint = DateTime.Now.Ticks,
            requiredPoint = DateTime.Now.Ticks + (Constant.TimeCycle.CHILD_GROWING_TIME * TimeSpan.TicksPerSecond),
            informationID = character.ID

        });
    }

    public void AddCharacterToList(Character character)
    {
        allCharacters.Add(character);
        return;
    }

    void OnGameDataLoadFinished()
    {
        ApplyItemEffectsToCharacters();

    }
    void ApplyItemEffectsToCharacters()
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
        if (character.workStatus != Character.WorkStatus.Idle && character.workStatus != Character.WorkStatus.Working)
        {
            Debug.LogWarning($"{character.workStatus} status is unable to assign work.");
            return false;
        }
        if (builder.CharacterInBuilding[team].Characters.Count >= LoadManager.Instance.allBuildingData[builder.Type].maxCharacterStored[builder.Level].amount[team])
        {
            Debug.LogWarning("This building is Full");
            return false;
        }

        if (character.workStatus == Character.WorkStatus.Working && character.WorkingPlaceID != -1)
        {
            Builder oldBuilder = BuildingManager.Instance.AllBuildings.Single(b => b.ID == character.WorkingPlaceID);
            Debug.Log($"This Character is already doing work at {oldBuilder.Type}. Try canceling old work . . .");
            CancelAssignWork(character);

        }

        builder.CharacterInBuilding[team].Characters.Add(character);
        character.workStatus = Character.WorkStatus.Working;

        character.WorkingPlaceID = builder.ID;
        Debug.Log($"{character.WorkingPlaceID} : {builder.ID}");
        LoadManager.Instance.SavePlayerDataToFireBase();
        EventManager.Instance.CharacterAssigned();

        return true;
    }
    public bool CancelAssignWork(Character character)
    {
        Builder builder = BuildingManager.Instance.AllBuildings.SingleOrDefault(b => b.ID == character.WorkingPlaceID);
        if (character.workStatus == Character.WorkStatus.Pregnant || character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning($"{character.workStatus} Character can't be Unassign.");
            return false;
        }

        if (!builder.CharacterInBuilding.Find(cw => cw.Characters.Contains(character)).Characters.Remove(character)) 
        {
            Debug.LogError("Remove Character in Building FAILED.");
            return false;
        }

        character.workStatus = Character.WorkStatus.Idle;
        character.WorkingPlaceID = 0;
        Debug.Log($"Stop {character.Name} from working at {builder.Type}. Now {character.Name} is {character.workStatus}");
        EventManager.Instance.CharacterAssigned();
        LoadManager.Instance.SavePlayerDataToFireBase();
        return true;
    }

    public void ExpelCharacter(Character character)
    {
        if(character.workStatus != Character.WorkStatus.Idle)
        {
            Debug.Log($"You can't expel {character.workStatus} Character !");
            return;
        }

        if(!allCharacters.Remove(character))
        {
            Debug.Log($"Can't remove this Character for some reasons !");   
        }
        Debug.Log($"Expel {character.Name} successfully.");
        EventManager.Instance.CharacterAssigned();
    }

    public void ConsumeItem(Character character, Resource item)
    {
        if (ItemManager.Instance.TryConsumeResources(item.Name, 1))
        {
            if (character.effects.Exists(e => e.name == item.effect.name))
            {
                ApplyItemEffect(character, character.effects.Single(e => e.name == item.effect.name), true);

            }
            else
            {
                character.effects.Add(ObjectCopier.Clone(item.effect));
                ApplyItemEffect(character, character.effects[character.effects.Count - 1], false);

            }
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
