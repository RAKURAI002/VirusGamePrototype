using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected StringBuilder permanentTextBuilder;
    protected StringBuilder updateTextBuilder;
    public bool isWink { get; set; }
    protected Transform textPanel;
    protected Text permanentText;
    protected Text updateText;

    private void Awake()
    {
        permanentTextBuilder = new StringBuilder();
        updateTextBuilder = new StringBuilder();

        textPanel = transform.Find("TextPanel");
        permanentText = textPanel.GetComponentInChildren<Text>(true);
        updateText = textPanel.Find("UpdateText").GetComponent<Text>();

    }
    private void Start()
    {
        GetComponent<Animation>().enabled = isWink;


    }
    public abstract void Initialize<T>(T data, bool isWink);

    protected virtual void SetText()
    {
        if (permanentTextBuilder.Equals(default(StringBuilder)))
        {
            return;

        }
        permanentText.text = permanentTextBuilder.ToString();

        if (!updateTextBuilder.Equals(default(StringBuilder)))
        {
            updateText.text = updateTextBuilder.ToString();
            updateTextBuilder.Clear();
        }

    }
    protected virtual void UpdateText()
    {
        if (!updateTextBuilder.Equals(default(StringBuilder)))
        {
            Debug.Log(updateTextBuilder.ToString());
            updateText.text = updateTextBuilder.ToString();
            updateTextBuilder.Clear();

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textPanel.gameObject.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.gameObject.SetActive(false);

    }
}
