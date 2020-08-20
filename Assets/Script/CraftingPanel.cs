using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingPanel : MonoBehaviour
{
    private void OnEnable()
    {
        foreach(KeyValuePair<string, Resource> resource in ItemManager.Instance.AllResources.Where(r => r.Key.Contains("Recipe")))
        {
            Debug.Log(resource.Key);
        }
    }
}
