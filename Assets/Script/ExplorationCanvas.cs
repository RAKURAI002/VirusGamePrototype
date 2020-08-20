using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExplorationCanvas : MonoBehaviour
{
    public GameObject areaPanel;

    private void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        //foreach (KeyValuePair<int, int> quest in questInProgress)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnMouseOver()
    {
       
    }




    public void onClickBase()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void onClickArea()
    {
        areaPanel.SetActive(true);
    }
    public void ExitButton()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
}
