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

    [Header("Close Setting")]
    public bool isCloseAllPanel;

    Action callback;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverUI = false;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
      
        /*foreach (var item in results)
        {
            Debug.Log($"{item.gameObject.name}");
        }
        Debug.Log($"{results.Count} : {gameObject.name}");*/
        return results.Where(r => r.gameObject.name == gameObject.name).ToList().Count > 0;
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
            if (!IsPointerOverUIObject())
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

        Debug.Log("ClosePanelHelper : Close Panel by " + gameObject.name);
        GameObject.FindGameObjectsWithTag("ExtraUIPanel").ToList().ForEach(
            (go) => {go.SetActive(false); });
        if(isCloseAllPanel)
        {
            GameObject.FindGameObjectsWithTag("Panel").ToList().ForEach(
           (go) => { go.SetActive(false); });

        }

        MainCanvas.FreezeCamera = false;
        gameObject.SetActive(false);
        callback?.Invoke();
        

    }


}
