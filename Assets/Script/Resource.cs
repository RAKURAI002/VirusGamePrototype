using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Resource : Item
{
    public Resource(string name, int amount)
    {
        Resource resourceData = LoadManager.Instance.allResourceData[name];

        this.id = resourceData.id;
        this.name = resourceData.name;
        this.amount = amount;
        this.rarity = resourceData.rarity;
        this.description = resourceData.description;
        this.type = resourceData.type;
        this.spritePath = resourceData.spritePath;
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
        Currency,
        Recipe
    }

    [SerializeField] public ResourceType type;

    [SerializeField] private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    public override string ToString()
    {
        return ($"ID : {id}, Name : {name}, Rarity : {rarity}, Description : {description}, Type : {type}. ");
    }

}
