using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Equipment : Item
{
    [System.Serializable]
    public enum EquipmentPosition
    {
        Unknown,
        Head,
        Face,
        Hand,
        Body,
        Leg,
        Foot
    }

    public Equipment(int id, string name, RarityTier rarity, string description, EquipmentPosition position, string spritePath, Character.AllStats stats)
    {
        this.id = id;
        this.name = name;
        this.rarity = rarity;
        this.description = description;
        this.position = position;
        this.spritePath = spritePath;
        this.stats = stats;
    }
    public Equipment(Equipment equipment)
    {
        this.id = equipment.id;
        this.name = equipment.name;
        this.rarity = equipment.rarity;
        this.description = equipment.description;
        this.position = equipment.position;
        this.spritePath = equipment.spritePath;
        this.stats = equipment.stats;
    }

    [SerializeField] private EquipmentPosition position;
    [SerializeField] private Character.AllStats stats;

    [SerializeField] private int allAmount;
    [SerializeField] private int usingAmount;

    public EquipmentPosition Position { get { return position; } }
    public Character.AllStats Stats { get { return stats; } }
    public int AllAmount { get { return allAmount; } set { allAmount = value; } }
    public int UsingAmount { get { return usingAmount; } set { usingAmount = (value <= allAmount) ? value : throw new Exception("AvailableAmount reaching exceed amount."); } }
    
}
