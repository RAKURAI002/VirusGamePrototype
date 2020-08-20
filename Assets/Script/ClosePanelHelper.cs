using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
public class ClosePanelHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool isPointerOverUI;

    Action callback;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOverUI = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        callback?.Invoke();
        isPointerOverUI = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !isPointerOverUI)
        {
            Debug.Log("ClosePanelHelper : Close Panel by " + gameObject.name);
            StartCoroutine(ClosePanel());
        }

    }
    public void ForceClosePanel()
    {
        StartCoroutine(ClosePanel());
    }
    public void SetOnExitCallback(Action callback)
    {
        this.callback = callback;
    }
    IEnumerator ClosePanel()
    {
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
        GameObject.FindGameObjectsWithTag("ExtraUIPanel").ToList().ForEach((go) => { Debug.Log(go.name); go.SetActive(false); });

        MainCanvas.canvasActive = false;

    }
}
