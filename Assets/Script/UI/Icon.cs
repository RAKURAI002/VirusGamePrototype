using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    StringBuilder desciption;
    bool isWink;

    public virtual void StartIcon()
    {
        
    }
    public abstract void AddDescription();
    public  void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {       
    }
}
