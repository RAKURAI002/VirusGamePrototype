using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Slot : MonoBehaviour, IPointerDownHandler
{
    public Character character { get; set; }
    public Builder builder { get; set; }
    public int teamNumber { get; set; }

    public bool isInteractable { get; set; }

    Image thisImage;

    private void Awake()
    {
        isInteractable = true;
        character = null;
        builder = null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(isInteractable && character != null)
        {
            if(CharacterManager.Instance.CancleAssignWork(character, builder))
            {
                character = null;
            }
               
           
        }
    }
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(character != null)
        {
            thisImage.sprite = Resources.Load<Sprite>(character.spritePath);
        }
        else
        {
            thisImage.sprite = null;
        }
        
    }


}
