using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TextBoxScaler : MonoBehaviour
{
    public Text TargetText;
    public Vector2 Offset;

    void OnEnable()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(TargetText.preferredWidth + Offset.x, TargetText.preferredHeight + Offset.y);

    }


}
