using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandConfirmationPanel : MonoBehaviour
{
    public int expandingAreaID { get; set; }
    // Start is called before the first frame update
    private void OnEnable()
    {
        Text statusText = gameObject.GetComponentInChildren<Text>();
        GameObject treeGO = GameManager.FindDeepChild(GameObject.Find("Map/Trees").transform, expandingAreaID.ToString()).gameObject;
        int fogAreaID = int.Parse(treeGO.transform.parent.name.Replace("Group", ""));

        int expandGoldPrice = 5 * fogAreaID;

        int expandDiamondPrice = fogAreaID;

        statusText.text = $"Expanding this area for {expandGoldPrice} Gold or {expandDiamondPrice} Diamonds.";
    }
    void Start()
    {
        transform.Find("GoldButton").GetComponent<Button>().onClick.AddListener(()=> { OnClickExpandAreaButton(0); });
        transform.Find("DiamondButton").GetComponent<Button>().onClick.AddListener(() => { OnClickExpandAreaButton(1); });

    }
    void OnClickExpandAreaButton(int purchaseOption)
    {
            if (MapManager.Instance.PurchaseNewArea(expandingAreaID, purchaseOption))
            {
                Debug.Log("Complete");
                GetComponent<ClosePanelHelper>().ForceClosePanel();
            }
            else
            {
                 string resourceName = purchaseOption == 0 ? "Gold" : "Diamond";
                 gameObject.GetComponentInChildren<Text>().text = $"Not enough {resourceName}. ";
            }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
