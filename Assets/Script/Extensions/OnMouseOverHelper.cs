using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseOverHelper : MonoBehaviour
{
    [SerializeField] public bool isMouseOverObject;

    Action callback;

    public void SetOnClickCallBack(Action _callback)
    {
        callback = _callback;

    }
    private void OnMouseOver()
    {
        isMouseOverObject = true;

    }

    private void OnMouseExit()
    {
        isMouseOverObject = false;

    }

    private void OnMouseDown()
    {
        callback?.Invoke();
    }

}
