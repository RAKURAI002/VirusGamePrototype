using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetWarningPanel : MonoBehaviour
{
    private void OnEnable()
    {
        if(GameManager.isGameDataLoaded)
        {
            GameManager.FindInActiveObjectByName("Fog").GetComponent<Animation>().Play("Fog_On_Stop_Game");

        }

    }


    public void OnClickTryReconnectInternet()
    {
        GameManager.Instance.ReloadGame();
    }
}
