using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : MonoBehaviour
{

    void Start()
    {
        
    }

    public void OnClickLinkAccountButton()
    {
        transform.Find("LinkAccountPanel").gameObject.SetActive(true);
    }
}
