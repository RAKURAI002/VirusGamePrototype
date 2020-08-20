using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningText : MonoBehaviour
{


    public void SetWarningMessage(string message, int seconds)
    {
        gameObject.GetComponentInChildren<Text>().color = Color.red;
        gameObject.GetComponentInChildren<Text>().text = message;
        StartCoroutine(FadingCoroutine(seconds));
    }
    IEnumerator FadingCoroutine(int seconds)
    {

        Text text = gameObject.GetComponentInChildren<Text>();
        for (float i = seconds; i >= 0; i -= Time.deltaTime)
        {
            text.color = new Color(1, 0, 0, (i / seconds));
            yield return null;
        }
        gameObject.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
