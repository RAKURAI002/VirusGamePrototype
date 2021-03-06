﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TextBoxScaler : MonoBehaviour
{
    public Text TargetText;

    [InspectorName("Optional")]
    public Text ExtraText;
    public Vector2 Offset;

    void OnEnable()
    {
        Debug.Log($"Scale");
        StartCoroutine(Scale());

    }
    IEnumerator Scale()
    {
        yield return new WaitForEndOfFrame();

        if (ExtraText == default(Text))
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(TargetText.preferredWidth + Offset.x, TargetText.preferredHeight + Offset.y);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(TargetText.preferredWidth, ExtraText.preferredWidth) + Offset.x, 
                TargetText.preferredHeight + ExtraText.preferredHeight + Offset.y);
           
        }
      //  Debug.Log($"{TargetText.text}");
      //  Debug.Log($"{GetComponent<RectTransform>().sizeDelta}");
    }

}
