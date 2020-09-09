using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected StringBuilder desciption;
    protected bool isWink;
    protected Transform textPanel;
    protected Text text;

    private void Awake()
    {
        textPanel = transform.Find("TextPanel");
        text = textPanel.GetComponentInChildren<Text>();
        desciption = new StringBuilder();

    }

    protected abstract void SetDescription();
    protected virtual void UpdateText()
    {
        text.text = desciption.ToString();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDescription();
        textPanel.gameObject.SetActive(true);
        UpdateText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.gameObject.SetActive(false);

    }
}
