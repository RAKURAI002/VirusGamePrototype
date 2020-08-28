using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[System.Serializable]
public class Item
{
    [System.Serializable]
    public enum RarityTier{ 
        Unknown,
        Common,
        Uncommon,
        Rare,
        SuperRare,
        UltraRare,
        MythologicalRare

    }

    [SerializeField] protected int id = -1;
    [SerializeField] protected string name = "Default Resource Name";

    [SerializeField] protected RarityTier rarity;
    [SerializeField] public string description = "Please assign something.";
    [SerializeField] public string spritePath = "Please assign something.";

    [SerializeField] public CraftingData craftingData;


    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public RarityTier Rarity { get { return rarity; } }

    [System.Serializable]
    public class CraftingData
    {
        public CraftingData(DictionaryStringToInt craftingMaterials, int point)
        {
            this.craftingMaterials = craftingMaterials;
            this.pointRequired = point;
        }

        [SerializeField] public DictionaryStringToInt craftingMaterials;
        [SerializeField] public int pointRequired;

    }

    public override string ToString()
    {
        return ($"ID : {id}, Name : {name}, Rarity : {rarity}, Description : {description} ");
    }


    

}
