using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AddCharacterPanel : MonoBehaviour
{
    [SerializeField] Character character;

    private void OnEnable()
    {
        character = CharacterManager.Instance.characterWaitingInLine.First();
        RefreshWaitingCharacterPanel();

        EventManager.Instance.OnCharacterAssigned += OnCharacterAddEvent;
    }

    private void OnDisable()
    {
        if(EventManager.Instance)
        {
            EventManager.Instance.OnCharacterAssigned -= OnCharacterAddEvent;
        }
       
    }
    void OnCharacterAddEvent()
    {
        RefreshWaitingCharacterPanel();
    }
    void RefreshWaitingCharacterPanel()
    {
        if (CharacterManager.Instance.characterWaitingInLine.Count == 0)
        {
            GetComponent<ClosePanelHelper>().ForceClosePanel();
            return;
        }

        character = CharacterManager.Instance.characterWaitingInLine.First();


        ClearWaitingCharacterList();
        CreateWaitingCharacterList();
        RefreshInformationPanel();


    }
    void ClearWaitingCharacterList()
    {

        Transform container = transform.Find("ScrollView/Container");
        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);

        }
    }

    void CreateWaitingCharacterList()
    {

        Transform container = transform.Find("ScrollView/Container");


        foreach (Character character in CharacterManager.Instance.characterWaitingInLine)
        {
            GameObject characterPanelGO = Instantiate(Resources.Load("Prefabs/UI/CharactePanelPrefab") as GameObject, container);
            characterPanelGO.transform.Find("CharacterImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
            characterPanelGO.transform.Find("Name").GetComponent<Text>().text = character.Name;
            characterPanelGO.transform.Find("Level").GetComponent<Text>().text = $"Level {character.level}";
            characterPanelGO.transform.Find("GenderImage").GetComponent<Image>().sprite = 
                Resources.Load<Sprite>(character.Gender == Character.GenderType.Male ? "Sprites/UI/MaleIcon" : "Sprites/UI/FemaleIcon");

            characterPanelGO.GetComponent<Button>().onClick.AddListener(() => { this.character = character; RefreshInformationPanel(); });
            
        }

    }
    public void OnClickAcceptCharacter()
    {
        CharacterManager.Instance.AddCharacterToList(character);
        CharacterManager.Instance.characterWaitingInLine.Remove(character);
        RefreshWaitingCharacterPanel();
        EventManager.Instance.CharacterAssigned();

    }
    public void OnClickRefuseCharacter()
    {
        CharacterManager.Instance.characterWaitingInLine.Remove(character);
        RefreshWaitingCharacterPanel();

        EventManager.Instance.CharacterAssigned();

    }

    void RefreshInformationPanel()
    {
        GameObject characterImage = transform.Find("InformationPanel/StatusPanel/CharacterImage").gameObject;
        characterImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
        GameObject statsPanel = transform.Find("InformationPanel/StatusPanel/STATS").gameObject;
        statsPanel.transform.parent.Find("Name").GetComponent<Text>().text = $"<color=green>{character.Name}</color>";
        statsPanel.transform.parent.Find("Level").GetComponent<Text>().text = "<color=green>Level</color> : " + character.level;
        statsPanel.transform.Find("Healthy").GetComponent<Text>().text = $"Healthy :\t{character.Stats.immunity}";
        statsPanel.transform.Find("Crafting").GetComponent<Text>().text = $"Crafting :\t{character.Stats.craftsmanship}";
        statsPanel.transform.Find("Intelligence").GetComponent<Text>().text = $"Intelligence :\t{character.Stats.intelligence}";
        statsPanel.transform.Find("Strength").GetComponent<Text>().text = $"Strength :\t{character.Stats.strength}";
        statsPanel.transform.Find("Observing").GetComponent<Text>().text = $"Observing :\t{character.Stats.perception}";
        statsPanel.transform.Find("Luck").GetComponent<Text>().text = $"Luck :\t{character.Stats.luck}";
        statsPanel.transform.Find("Speed").GetComponent<Text>().text = $"Speed :\t{character.Stats.speed}";

        Transform birthMarkContainer = transform.Find("InformationPanel/StatusPanel/BirthMarkPanel/Container");
        foreach (Transform transform in birthMarkContainer)
        {
            Destroy(transform.gameObject);

        }

        foreach (Character.BirthMark birthMark in character.BirthMarks)
        {
            Image bmImage = new GameObject().AddComponent<Image>();
            bmImage.transform.SetParent(birthMarkContainer);
            bmImage.sprite = Resources.Load<Sprite>(birthMark.spritePath);

        }

    }


}
