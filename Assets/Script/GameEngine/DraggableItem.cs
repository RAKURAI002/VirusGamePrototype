using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool isSelected;
    public LayerMask ignoreLayer;
    
    GameObject draggingGO = null;
    [SerializeField] Character character;

    void Start()
    {
        character = CharacterManager.Instance.AllCharacters.Single(c => c.ID.ToString() == gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggingGO == null)
        {
            return;
        }
        draggingGO.transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");

        isSelected = true;
        draggingGO = new GameObject();
        
        draggingGO.name = gameObject.name;
        
        if (character.workStatus == Character.WorkStatus.Quest)
        {
            Debug.LogWarning($"This character({character.Name} : ID {character.ID}) is already doing some Quest !");
            Destroy(draggingGO);
            return;
        }
        
        draggingGO.transform.SetParent(gameObject.transform.parent.transform.parent.transform.parent.transform);
        draggingGO.transform.position = Input.mousePosition;
        draggingGO.AddComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
        draggingGO.AddComponent<CanvasGroup>().blocksRaycasts = false;  
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (draggingGO == null || eventData.pointerEnter ==null)
        {
        }
        
        if (eventData.pointerEnter.tag == "DropSlot")
        {
            Slot slot = eventData.pointerEnter.GetComponent<Slot>();
           // Debug.Log($"Finding {slot.character.Name} :  {slot.character.ID}");
            if (CharacterManager.Instance.AssignWork(character, slot.builder, slot.teamNumber))
            {
                slot.character = CharacterManager.Instance.AllCharacters.Single(c => c.ID.ToString() == draggingGO.name);
                eventData.pointerEnter.GetComponent<Slot>().character = character;
            }
        }

        Destroy(draggingGO);
        return;
    }
}
