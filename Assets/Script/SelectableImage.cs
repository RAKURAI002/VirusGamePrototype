using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.Find("NamePanel").gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.Find("NamePanel").gameObject.SetActive(false);
    }
}
