using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
public class ClosePanelHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] bool isPointerOverUI;

    Action callback;

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"ENNNNTERRRRRRR");
        isPointerOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"EEEEEEXITTTTTTTTTTTTTTTTTTTTTT");
        isPointerOverUI = false;
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

        void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetMouseButtonUp(0) && !isPointerOverUI)
        {
            StartCoroutine(ClosePanel());
        }

#elif UNITY_ANDROID

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log($"{IsPointerOverUIObject()}");
            if(!IsPointerOverUIObject())
              StartCoroutine(ClosePanel());

        }

#endif
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
        int length = GameObject.FindObjectsOfType<ClosePanelHelper>().Length;
        Debug.Log("ClosePanelHelper : Close Panel by " + gameObject.name);
        GameObject.FindGameObjectsWithTag("ExtraUIPanel").ToList().ForEach((go) => { Debug.Log(go.name); go.SetActive(false); });
        gameObject.SetActive(false);
        callback?.Invoke();
        if (length <= 1)
        {
            MainCanvas.FreezeCamera = false;
        }
    }

    
}
