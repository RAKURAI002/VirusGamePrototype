using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;


//ทำกับ panel ตอนกดปุ่มดูว่ามีใคร  
public class ReviveCharacterPanel : MonoBehaviour
{
    [SerializeField] Character character;

    private void OnEnable()
    {
        EventManager.Instance.OnCharacterAssigned += RefreshDeadCharacterPanel;
    }
    private void OnDisable()
    {
        if(EventManager.Instance)
            EventManager.Instance.OnCharacterAssigned -= RefreshDeadCharacterPanel;
    }
    // Start is called before the first frame update


    private void Start()
    {
        RefreshDeadCharacterPanel();
    }

    void RefreshDeadCharacterPanel()
    {
        Debug.Log($"RefreshDeadCharacterPanel ");
        //คือถ้า 0 ตัวตอนเปิดหน้า panel อยู่ มันจะบังคับปิด
        if (CharacterManager.Instance.allDeadcharacter.Count == 0)
        {
            GetComponent<ClosePanelHelper>().ForceClosePanel();
            return;
        }
        //ถ้ามีตัวอยู่ เข้าไปทำ
        else
        {
            //ตัวแรกให้มันขึ้นเลยตอนกดเปิด panel อ่ะ
            character = CharacterManager.Instance.allDeadcharacter.Keys.First();

            //เคลียร์ทุกครั้งที่เปิดใหม่กันตัวซ้ำ
            ClearDeadCharacterList();
            //ทำหน้าเปิด
            CreateDeadCharacterList();

            RefreshInformationPanel();

        }



    }

    void ClearDeadCharacterList()
    {

        Transform container = transform.Find("ScrollView/Container");
        foreach (Transform transform in container)
        {
            Destroy(transform.gameObject);

        }
    }

    void CreateDeadCharacterList()
    {

        //หา container 
        Transform container = transform.Find("ScrollView/Container");

        //แบบสั้นๆ ไม่ต้อง CharacterManager.Instance.allDeadcharacter มาตลอด
        Dictionary<Character, long> timeoutCharacter = CharacterManager.Instance.allDeadcharacter;

        //วนลูปเหมือน foreach ใช้อันนี้ จะ remove ได้
        for (int i = 0; i < CharacterManager.Instance.allDeadcharacter.Count; i++)
        {
            Debug.Log($"{i}");
            //เช็คว่าหมดเวลายัง   
            if (timeoutCharacter.ElementAt(i).Value > DateTime.Now.Ticks)
            {
                //ถ้ายังก็ทำพรีแฟฟอันนั้น ที่อยู่ข้างซ้าย
                Debug.Log("3");
                GameObject characterPanelGO = Instantiate(Resources.Load("Prefabs/UI/CharactePanelPrefab") as GameObject, container);
                characterPanelGO.transform.Find("CharacterImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(timeoutCharacter.ElementAt(i).Key.spritePath);
                characterPanelGO.transform.Find("Name").GetComponent<Text>().text = timeoutCharacter.ElementAt(i).Key.Name;
                characterPanelGO.transform.Find("Level").GetComponent<Text>().text = $"Level {timeoutCharacter.ElementAt(i).Key.level}";
                characterPanelGO.transform.Find("GenderImage").GetComponent<Image>().sprite =
                Resources.Load<Sprite>(timeoutCharacter.ElementAt(i).Key.Gender == Character.GenderType.Male ? "Sprites/UI/MaleIcon" : "Sprites/UI/FemaleIcon");



                //ไม่รู้เหตุผลว่าทำไมใช้ i ด้วยไม่ได้ แต่แก้แบบนี้แล้วผ่าน จบปิ๊ง 
                Character currentCharacter = timeoutCharacter.ElementAt(i).Key;
                //แบบใน {} มันกลายเป็น i = 3 อย่างเดียว งง
                //เอาไว้ตอนกดแล้วขึ้นข้างขวา
                characterPanelGO.GetComponent<Button>().onClick.AddListener(() => { this.character = currentCharacter; RefreshInformationPanel(); });

            }
            else //ถ้าเกิวเวลาแล้วเอาออกเลย
            {
                CharacterManager.Instance.allDeadcharacter.Remove(timeoutCharacter.ElementAt(i).Key);
                LoadManager.Instance.SavePlayerDataToFireBase();
            }
        }

    }


    //ที่ขึ้นข้างขวา
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
        GameObject.Find("ReviveCharacterPanel/GoldText").GetComponent<Text>().text = $"Use {character.level * 1000} Gold to resurrect.";
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

    //กดชุปเสียตังเท่านี้ๆ
    public void OnClickAcceptCharacter()
    {
        if (ItemManager.Instance.TryConsumeResources("Gold", character.level * 1000))
        {
            character.CurrentHitPoint = character.MaxHitPoint;
            CharacterManager.Instance.allDeadcharacter.Remove(character);
            CharacterManager.Instance.AllCharacters.Add(character);
            LoadManager.Instance.SavePlayerDataToFireBase();
            RefreshDeadCharacterPanel();

            EventManager.Instance.CharacterAssigned();
        }


    }

    //ไม่เอาตัวนี้กลับจ้า
    public void OnClickRefuseCharacter()
    {
        CharacterManager.Instance.allDeadcharacter.Remove(character);
        LoadManager.Instance.SavePlayerDataToFireBase();
        RefreshDeadCharacterPanel();


        EventManager.Instance.CharacterAssigned();
    }
}