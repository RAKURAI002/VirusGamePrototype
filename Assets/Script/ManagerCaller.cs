using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerCaller : MonoBehaviour
{
    private void Awake()
    {
        
        if(!LoadManager.Instance)
        {
            Debug.Log($"Start calling all Manager . . .");
            Instantiate(Resources.Load("Prefabs/AllManager") as GameObject);
        }
    }
}
