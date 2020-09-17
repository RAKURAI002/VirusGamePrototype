using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseOverHelper : MonoBehaviour
{
    [SerializeField] public bool isMouseOverObject;

    Action onClickCallback;
    Action onMouseOverCallback;
    Action onMouseExitCallback;

    public void SetOnClickCallBack(Action _callback)
    {
        onClickCallback = _callback;

    }
    public void SetOnMouseCallBack(Action _onMouseOverCallback, Action _onMouseExitCallback)
    {
        onMouseOverCallback  = _onMouseOverCallback;
        onMouseExitCallback = _onMouseExitCallback;
    }
    private void OnMouseOver()
    {
        isMouseOverObject = true;
        onMouseOverCallback?.Invoke();
    }

    private void OnMouseExit()
    {
        isMouseOverObject = false;
        onMouseExitCallback?.Invoke();
    }

    private void OnMouseDown()
    {
        onClickCallback?.Invoke();
    }

}
