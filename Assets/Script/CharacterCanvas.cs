using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class CharacterCanvas : MonoBehaviour
{
    public GameObject equipmentContainer;

    public GameObject equipmentSlotPanel;
    public GameObject equipmentListPanel;

    List<GameObject> characterGO;
    List<GameObject> equipmentImageGO;

    [SerializeField] Character character;

    private void Awake()
    {
        characterGO = new List<GameObject>();
        equipmentImageGO = new List<GameObject>();
    }
    private void OnEnable()
    {
        ClearCharacterSlot();
        CreateCharacterSlot();
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdateInformation();
    }
    void UpdateInformation()
    {
        GameObject characterImage = transform.Find("MainPanel/InformationPanel/CharacterImage").gameObject;
        characterImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
        GameObject statusPanel = transform.Find("MainPanel/InformationPanel/StatusPanel").gameObject;
        statusPanel.transform.Find("Name").GetComponent<Text>().text = "<color=green>Name</color> : " + character.Name;
        statusPanel.transform.Find("Healthy").GetComponent<Text>().text = $"Healthy : {character.Stats.healthy} + <color=red>{character.equipments.Sum(e => e.Stats.healthy)}</color>";
        statusPanel.transform.Find("Crafting").GetComponent<Text>().text =  $"Crafting : {character.Stats.crafting} + <color=red>{character.equipments.Sum(e => e.Stats.crafting)}</color>";
        statusPanel.transform.Find("Intelligent").GetComponent<Text>().text =  $"Intelligent : {character.Stats.intelligence} + <color=red>{character.equipments.Sum(e => e.Stats.intelligence)}</color>";
        statusPanel.transform.Find("Strength").GetComponent<Text>().text = $"Strength : {character.Stats.strength} + <color=red>{character.equipments.Sum(e => e.Stats.strength)}</color>";
        statusPanel.transform.Find("Observing").GetComponent<Text>().text = $"Observing : {character.Stats.observing} + <color=red>{character.equipments.Sum(e => e.Stats.observing)}</color>";
        statusPanel.transform.Find("Luck").GetComponent<Text>().text = $"Luck : {character.Stats.luck} + <color=red>{character.equipments.Sum(e => e.Stats.luck)}</color>";
        statusPanel.transform.Find("Speed").GetComponent<Text>().text = $"Speed : {character.Stats.speed} + <color=red>{character.equipments.Sum(e => e.Stats.speed)}</color>";

    }
    void CreateCharacterSlot()
    {
        GameObject container = gameObject.transform.Find("CharacterPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for Character.");
            return;
        }

        foreach (Character character in CharacterManager.Instance.AllCharacters)
        {

            GameObject characterSlot = new GameObject();
            characterSlot.name = character.Name;
            
            characterSlot.transform.SetParent(container.transform);
            characterSlot.AddComponent<RectTransform>();
            characterSlot.AddComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
            characterSlot.AddComponent<Button>().onClick.AddListener(OnClickCharacterSlot);

            if (character.workStatus == Character.WorkStatus.Working)
            {
                characterSlot.GetComponent<Image>().color = Color.red;
            }
            else if (character.workStatus == Character.WorkStatus.Idle)
            {
                characterSlot.GetComponent<Image>().color = Color.white;
            }
            characterGO.Add(characterSlot);
        }
    }
    void ClearCharacterSlot()
    {
        if (characterGO != null)
        {
            for (int i = characterGO.Count - 1; i >= 0; i--)
            {
                Destroy(characterGO[i].gameObject);
            }
            characterGO.Clear();
        }
    }

    void OnClickCharacterSlot()
    {
        equipmentSlotPanel.SetActive(false);
        equipmentListPanel.SetActive(false);
        character = CharacterManager.Instance.AllCharacters.SingleOrDefault( c => c.Name == EventSystem.current.currentSelectedGameObject.name);
        
        if (this.character == null)
        {
            Debug.LogError("Can't find Character, Something happened here.");
            return;
        }
        Debug.Log($"Select : {character.Name}");
    }

    public void OnClickCharacterPicture()
    {
        Debug.Log(character.Name);
        if(character.Name == "")
        {
            return;
        }
        equipmentSlotPanel.SetActive(true);
        GameObject equippingPanel = equipmentSlotPanel.transform.Find("EquippingPanel").gameObject;
        for(int i = 1; i <= 6; i++)
        {
           // Debug.Log(((Equipment.EquipmentPosition)i).ToString());
            equippingPanel.transform.Find("Container/" + ((Equipment.EquipmentPosition)i).ToString()).GetComponent<Image>().sprite = null ;
        }
        foreach (Equipment equipment in character.equipments)
        {
            equippingPanel.transform.Find("Container/" + equipment.Position.ToString()).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(equipment.spritePath);
        }
        
    }
    public void OnClickEquipmentSlot()
    {
        equipmentListPanel.SetActive(true);
        ShowEquipmentsList();
    }
    public void ShowEquipmentsList()
    {
        if (equipmentImageGO != null)
        {
            foreach (GameObject go in equipmentImageGO)
            {
                Destroy(go);
            }
        }

        equipmentImageGO.Clear();
        foreach (var equipment in ItemManager.Instance.AllEquipments)
        {
            equipmentImageGO.Add(new GameObject());
            equipmentImageGO[equipmentImageGO.Count - 1].name = equipment.Value.Name;
            Image image = equipmentImageGO[equipmentImageGO.Count - 1].AddComponent<Image>();
            image.sprite = (Resources.Load<Sprite>(equipment.Value.spritePath));
            equipmentImageGO[equipmentImageGO.Count - 1].transform.SetParent(equipmentContainer.transform);
            Button button = equipmentImageGO[equipmentImageGO.Count - 1].AddComponent<Button>();
            button.onClick.AddListener(AssignEquipmentToCharacter);
            if (equipment.Value.AllAmount - equipment.Value.UsingAmount == 0)
            {
                image.color = Color.red;
                button.interactable = false;

            }

            GameObject textGO = new GameObject();
            textGO.name = "Amount";
            textGO.transform.SetParent(equipmentImageGO[equipmentImageGO.Count - 1].transform);
            
            Text text = textGO.AddComponent<Text>();
            text.text = (equipment.Value.AllAmount - equipment.Value.UsingAmount).ToString() + "/" + equipment.Value.AllAmount.ToString();
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().fontSize = 20;
            text.transform.position = text.transform.position + new Vector3(30, -110, 0);
        }
    }

    void AssignEquipmentToCharacter()
    {
        string selectedEquipmentName = EventSystem.current.currentSelectedGameObject.name;
        Equipment equipment = ItemManager.Instance.AllEquipments[selectedEquipmentName];

        Equipment oldEquipment = character.equipments.SingleOrDefault(e => e.Position == equipment.Position);
        if (oldEquipment != null)
        {
            Debug.LogWarning($"This Character has already worn {oldEquipment.Name} at position : {oldEquipment.Position}");
            return;
        }

        character.equipments.Add(equipment);
        equipment.UsingAmount++;
        Debug.Log($"Add {equipment.Name} to {character.Name}. Now {equipment.Name} has {equipment.UsingAmount} out of {equipment.AllAmount}. ");
        equipmentListPanel.SetActive(false);
        OnClickCharacterPicture();
    }
}
