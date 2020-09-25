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

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }

    IEnumerator ReloadSceneCoroutine()
    {
        GameObject dontDestroyManager = GameObject.FindGameObjectWithTag("DontDestroyManager");
        Debug.Log($"Destroying {dontDestroyManager.name}");
        Destroy(dontDestroyManager);

        yield return new WaitForEndOfFrame();
        
        Debug.Log($"Reloading scene . . .");
        SceneManager.LoadScene("MainScene");
    }
}
