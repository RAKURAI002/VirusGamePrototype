using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCanvas : MonoBehaviour
{
    public GameObject registerCanvas;
    public GameObject loginCanvas;

    public void OnRegisterButtonClicked()
    {
        loginCanvas.SetActive(false);
        registerCanvas.SetActive(true);
    }
    public void OnLoginButtonClicked()
    {
        loginCanvas.SetActive(true);
        registerCanvas.SetActive(false);
    }

}
