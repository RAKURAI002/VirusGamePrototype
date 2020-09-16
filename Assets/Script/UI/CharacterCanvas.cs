using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Reflection;

public class CharacterCanvas : MonoBehaviour
{

    public GameObject equipmentContainer;

    public GameObject itemSlotPanel;
    public GameObject equipmentListPanel;

    [SerializeField] Character character;

    private void Awake()
    {
    }
    private void OnEnable()
    {
        CreateCharacterSlot();
        EventManager.Instance.OnCharacterAssigned += UpdateInformation;
        EventManager.Instance.OnResourceChanged += OnResourceChanged;

    }
    private void OnDisable()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
            EventManager.Instance.OnCharacterAssigned -= UpdateInformation;
        }
    }

    void Start()
    {
        character = CharacterManager.Instance.AllCharacters[0];
        UpdateInformation();

    }

    void Update()
    {
    }

    void OnResourceChanged(string name)
    {
        GameObject consumableItemPanelGO = transform.Find("MainPanel/ConsumableItemPanel").gameObject;
        if (consumableItemPanelGO.activeSelf)
        {
            RefreshConsumableItemList();

        }
        UpdateInformation();
    }
    void UpdateInformation()
    {
        GameObject characterImage = transform.Find("MainPanel/InformationPanel/CharacterImage").gameObject;
        characterImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
        GameObject statsPanel = transform.Find("MainPanel/InformationPanel/StatusPanel/STATS").gameObject;
        statsPanel.transform.parent.Find("Name").GetComponent<Text>().text = "<color=green>Name</color> : " + character.Name;
        statsPanel.transform.parent.Find("Gender").GetComponent<Text>().text = "<color=green>Gender</color> : " + character.Gender.ToString();
        statsPanel.transform.parent.Find("Level").GetComponent<Text>().text = "<color=green>Level</color> : " + character.level;
        statsPanel.transform.Find("Healthy").GetComponent<Text>().text = $"Healthy :\t{character.Stats.immunity} + <color=red>{character.equipments.Sum(e => e.Stats.immunity)}</color>";
        statsPanel.transform.Find("Crafting").GetComponent<Text>().text = $"Crafting :\t{character.Stats.craftsmanship} + <color=red>{character.equipments.Sum(e => e.Stats.craftsmanship)}</color>";
        statsPanel.transform.Find("Intelligence").GetComponent<Text>().text = $"Intelligence :\t{character.Stats.intelligence} + <color=red>{character.equipments.Sum(e => e.Stats.intelligence)}</color>";
        statsPanel.transform.Find("Strength").GetComponent<Text>().text = $"Strength :\t{character.Stats.strength} + <color=red>{character.equipments.Sum(e => e.Stats.strength)}</color>";
        statsPanel.transform.Find("Observing").GetComponent<Text>().text = $"Observing :\t{character.Stats.perception} + <color=red>{character.equipments.Sum(e => e.Stats.perception)}</color>";
        statsPanel.transform.Find("Luck").GetComponent<Text>().text = $"Luck :\t{character.Stats.luck} + <color=red>{character.equipments.Sum(e => e.Stats.luck)}</color>";
        statsPanel.transform.Find("Speed").GetComponent<Text>().text = $"Speed :\t{character.Stats.speed} + <color=red>{character.equipments.Sum(e => e.Stats.speed)}</color>";

        Text statsPointText = statsPanel.transform.Find("StatsPoint").GetComponent<Text>();

        Transform birthMarkContainer = transform.Find("MainPanel/InformationPanel/StatusPanel/BrithMarkPanel/Container");
        foreach(Transform transform in birthMarkContainer)
        {
            Destroy(transform.gameObject);

        }

        foreach(Character.BirthMark birthMark in character.BirthMarks)
        {
            BirthMarkData birthMarkData = ObjectCopier.Clone(LoadManager.Instance.allBirthMarkDatas[birthMark.name]);
            birthMarkData.level = birthMark.level;
            BirthMarkIcon birthMarkIcon = Instantiate(Resources.Load("Prefabs/UI/IconPrefab") as GameObject, birthMarkContainer).AddComponent<BirthMarkIcon>();
            birthMarkIcon.Initialize(birthMarkData, false);

        }


        List<Transform> plusButtons = new List<Transform>();


        GameObject buffIconContainer = transform.Find("MainPanel/InformationPanel/BuffIconContainer").gameObject;
        foreach(Transform transform in buffIconContainer.transform)
        {
            Destroy(transform.gameObject);

        }

        foreach(Resource.Effect effect in character.effects)
        {
            GameObject buffIconGO = Instantiate(Resources.Load("Prefabs/UI/IconPrefab") as GameObject, buffIconContainer.transform);
            buffIconGO.AddComponent<BuffIcon>().Initialize(effect, true);

        }


        foreach (Transform transform in statsPanel.transform.Cast<Transform>().Select(t => t.Find("PlusButton")).OfType<Transform>())
        {
            plusButtons.Add(transform);

        }

        if (character.statsUpPoint > 0)
        {
            statsPointText.gameObject.SetActive(true);
            statsPointText.text = $"Stats Point Left : {character.statsUpPoint}";

            plusButtons.ForEach((t) => { t.gameObject.SetActive(true); });

        }
        else
        {
            statsPointText.gameObject.SetActive(false);
            plusButtons.ForEach((t) => { t.gameObject.SetActive(false); });

        }


        Slider expSlider = transform.Find("MainPanel/InformationPanel/EXPSlider").GetComponent<Slider>();

        expSlider.maxValue = character.level * 10;
        expSlider.value = character.level * 10 - ((character.level * 5 * (character.level + 1)) - character.Experience);
        
        expSlider.GetComponentInChildren<Text>().text = ((character.level * 5 * (character.level + 1)) - character.Experience).ToString() + " EXP left";

        Slider hpSlider = transform.Find("MainPanel/InformationPanel/HPSlider").GetComponent<Slider>();
        hpSlider.maxValue = character.MaxHitPoint;
        hpSlider.value = character.CurrentHitPoint;
       
        hpSlider.GetComponentInChildren<Text>().text = $"{character.CurrentHitPoint} / {character.MaxHitPoint}";

    }

    void CreateCharacterSlot()
    {
        GameObject container = gameObject.transform.Find("CharacterPanel/Container").gameObject;
        if (container == null)
        {
            Debug.LogError("Can't find Scrollview container for Character.");
            return;

        }

        foreach (Transform transform in container.transform)
        {
            Destroy(transform.gameObject);

        }

        foreach (Character character in CharacterManager.Instance.AllCharacters)
        {
            GameObject characterSlot = new GameObject();
            characterSlot.name = character.ID.ToString();

            characterSlot.transform.SetParent(container.transform);
            characterSlot.AddComponent<RectTransform>();
            characterSlot.AddComponent<Image>().sprite = Resources.Load<Sprite>(character.spritePath);
            characterSlot.AddComponent<Button>().onClick.AddListener(OnClickCharacterSlot);

            if (character.workStatus == Character.WorkStatus.Working)
            {
                characterSlot.GetComponent<Image>().color = Color.blue;

            }
            else if (character.workStatus == Character.WorkStatus.Quest)
            {
                characterSlot.GetComponent<Image>().color = Color.red;

            }
            else if (character.workStatus == Character.WorkStatus.Idle)
            {
                characterSlot.GetComponent<Image>().color = Color.white;

            }

        }

    }
    public void OnClickPlusStatsButton()
    {
        string statsName = EventSystem.current.currentSelectedGameObject.transform.parent.name;

        Debug.Log(statsName.ToLower());
        FieldInfo fInfo = typeof(Character.AllStats).GetField(statsName.ToLower());
        //  Debug.Log(typeof(Character.AllStats).GetField(statsName.ToLower()).ToString());

        fInfo.SetValue(character.Stats, (int)fInfo.GetValue(character.Stats) + 1);
        character.statsUpPoint--;
        //character.IncreaseStats(new Character.AllStats() { fInfo.ReflectedType = 1 });
        UpdateInformation();

    }

    void OnClickCharacterSlot()
    {
        character = CharacterManager.Instance.AllCharacters.SingleOrDefault(c => c.ID.ToString() == EventSystem.current.currentSelectedGameObject.name);

        if (this.character == null)
        {
            Debug.LogError("Can't find Character, Something happened here.");
            return;
        }

        itemSlotPanel.SetActive(false);
        equipmentListPanel.SetActive(false);
        transform.Find("MainPanel/ConsumableItemPanel").gameObject.SetActive(false);

        Debug.Log($"Select : {character.Name}");
        UpdateInformation();
    }

    public void OnClickCharacterPicture()
    {
        Debug.Log(character.Name);
        if (character.Name == "")
        {
            return;
        }
        if (character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning("You cannot modify Character that currently on Quest.");
            return;
        }

        itemSlotPanel.SetActive(true);
        GameObject equippingPanel = itemSlotPanel.transform.Find("EquippingPanel").gameObject;
        for (int i = 1; i <= 6; i++)
        {
            equippingPanel.transform.Find("Container/" + ((Equipment.EquipmentPosition)i).ToString()).GetComponent<Image>().sprite = null;
        }
        foreach (Equipment equipment in character.equipments)
        {
            equippingPanel.transform.Find("Container/" + equipment.Position.ToString()).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(equipment.spritePath);
        }

    }
    public void OnClickEquipmentSlot()
    {
        equipmentListPanel.SetActive(true);
        RefreshEquipmentsList(EventSystem.current.currentSelectedGameObject.name);
    }
    public void OnClickUseItemButton()
    {
        if (character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning("You cannot modify Character that currently on Quest.");
            return;
        }

        GameObject consumableItemPanelGO = transform.Find("MainPanel/ConsumableItemPanel").gameObject;
        consumableItemPanelGO.SetActive(true);
        RefreshConsumableItemList();

    }
    void RefreshConsumableItemList()
    {
        GameObject consumableItemPanelGO = transform.Find("MainPanel/ConsumableItemPanel").gameObject;
        GameObject container = consumableItemPanelGO.transform.Find("Mask/Container").gameObject;

        foreach (Transform transform in container.transform)
        {
            Destroy(transform.gameObject);

        }

        foreach (var item in ItemManager.Instance.AllResources.Where(r => r.Value.type == Resource.ResourceType.Consumable))
        {
            if (ItemManager.Instance.GetResourceAmount(item.Value.Name) == 0)
            {
                return;
            }
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
            itemGO.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.Value.spritePath);

            Button button = itemGO.AddComponent<Button>();
            button.onClick.AddListener(() => { CharacterManager.Instance.ConsumeItem(character, item.Value); transform.Find("MainPanel/ConsumableItemPanel").gameObject.SetActive(false); });

            Text amount = itemGO.GetComponentInChildren<Text>();
            amount.text = ItemManager.Instance.GetResourceAmount(item.Value.Name).ToString();

        }

    }
    public void RefreshEquipmentsList(string _position)
    {
        Equipment.EquipmentPosition position = (Equipment.EquipmentPosition)Enum.Parse(typeof(Equipment.EquipmentPosition), _position);
        GameObject container = transform.Find("MainPanel/WearingItemSlotPanel/EquipmentPanel/EquipmentListPanel/Scrollview/Container").gameObject;

        foreach (Transform transform in container.transform)
        {
            Destroy(transform.gameObject);

        }

        GameObject cancleButtonGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
        cancleButtonGO.name = "CancleEquippingButton";
        cancleButtonGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/Button/BtnCancel");
        cancleButtonGO.GetComponentInChildren<Text>().text = "";
        cancleButtonGO.AddComponent<Button>().onClick.AddListener(() => { UnAssignEquipmentToCharacter(position); UpdateInformation(); });

        foreach (var equipment in ItemManager.Instance.AllEquipments.Where(e => e.Value.Position == position))
        {
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/UI/ImageWithAmountPrefab") as GameObject, container.transform);
            itemGO.name = equipment.Value.Name;
            Image image = itemGO.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(equipment.Value.spritePath);

            Text amount = itemGO.GetComponentInChildren<Text>();
            amount.text = $"{equipment.Value.UsingAmount} / {equipment.Value.AllAmount}";

            Button button = itemGO.AddComponent<Button>();
            button.onClick.AddListener(() => { AssignEquipmentToCharacter(equipment.Value); UpdateInformation(); });

            if (equipment.Value.AllAmount - equipment.Value.UsingAmount == 0)
            {
                image.color = Color.red;
                button.interactable = false;

            }

        }

    }
    void UnAssignEquipmentToCharacter(Equipment.EquipmentPosition position)
    {
        Equipment equipment = character.equipments.SingleOrDefault(e => e.Position == position);
        if (equipment == null)
        {
            return;
        }

        character.equipments.Remove(equipment);
        equipment.UsingAmount--;
        Debug.Log($"Remove {equipment.Name} from {character.Name}. Now {equipment.Name} has {equipment.UsingAmount} out of {equipment.AllAmount}. ");
        equipmentListPanel.SetActive(false);

        OnClickCharacterPicture();
    }

    void AssignEquipmentToCharacter(Equipment equipment)
    {
        Equipment oldEquipment = character.equipments.SingleOrDefault(e => e.Position == equipment.Position);
        if (oldEquipment != null)
        {
            UnAssignEquipmentToCharacter(oldEquipment.Position);

        }

        character.equipments.Add(equipment);
        equipment.UsingAmount++;
        Debug.Log($"Add {equipment.Name} to {character.Name}. Now {equipment.Name} has {equipment.UsingAmount} out of {equipment.AllAmount}. ");
        equipmentListPanel.SetActive(false);

        OnClickCharacterPicture();

    }

    public void AddCharacterLevelTest()
    {
        character.AddEXP(5);
        UpdateInformation();

    }

}
