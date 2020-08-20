using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Resource : Item
{
    public Resource()
    {
    }
    public Resource(int id, string name, RarityTier rarity, string description, ResourceType type, string spritePath)
    {
        this.id = id;
        this.name = name;
        this.rarity = rarity;
        this.description = description;
        this.type = type;
        this.spritePath = spritePath;
    }

    [System.Serializable]
    public enum ResourceType
    {
        Unknown,
        Consumable,
        Ingredient,
        Material,
        Special,
        Currency
    }

    [SerializeField] public ResourceType type;

    public override string ToString()
    {
        return ($"ID : {id}, Name : {name}, Rarity : {rarity}, Description : {description}, Type : {type}. ");
    }

}
