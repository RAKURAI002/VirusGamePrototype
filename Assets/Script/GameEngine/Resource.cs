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

        if(type == ResourceType.Consumable)
        {
            this.effect = resourceData.effect;

        }
        else if(type == ResourceType.ConsumableRecipe || type == ResourceType.GadgetRecipe || type == ResourceType.MedicineRecipe)
        {
            this.craftingData = resourceData.craftingData;
        }
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
    public Resource(int id, string name, RarityTier rarity, string description, ResourceType type, string spritePath, Effect effect)
    {
        if (!(type == ResourceType.Consumable || type == ResourceType.Gadget || type == ResourceType.Medicine))
        {
            Debug.LogError("Only Consumable type can contain Effect. Otherwise may caused errors.");

        }

        this.id = id;
        this.name = name;
        this.rarity = rarity;
        this.description = description;
        this.type = type;
        this.spritePath = spritePath;
        this.effect = effect;
    }

    public Resource(int id, string name, RarityTier rarity, string description, ResourceType type, string spritePath, CraftingData craftingMaterials)
    {
        if (!IsRecipeType(type))
        {
            Debug.LogError("Only Recipe type can contain CraftingMaterials. Otherwise may caused errors.");
        }

        this.id = id;
        this.name = name;
        this.rarity = rarity;
        this.description = description;
        this.type = type;
        this.spritePath = spritePath;
        this.craftingData = craftingMaterials;
    }

    [System.Serializable]
    public enum ResourceType
    {
        Unknown,
        Consumable, /// Crafting Item from Kitchen
        Medicine,
        Gadget,
        Ingredient,
        Material,
        Special, /// Building Production Point
        Currency,


        ConsumableRecipe,
        GadgetRecipe,
        MedicineRecipe

    }
    public bool IsRecipe()
    {
        return (type == ResourceType.MedicineRecipe || type == ResourceType.GadgetRecipe || type == ResourceType.ConsumableRecipe);
    }
    public static bool IsRecipeType(ResourceType type)
    {
        return (type == ResourceType.MedicineRecipe || type == ResourceType.GadgetRecipe || type == ResourceType.ConsumableRecipe);
    }

    [System.Serializable]
    public class Effect
    {
        public Effect()
        {
        }

        public Effect(Effect effect)
        {
            this.duration = effect.duration;
        } 

        [SerializeField] public string name;
        [SerializeField] public string spritePath;
        [SerializeField] public Character.AllStats stats;
        [SerializeField] public int duration;

        [SerializeField] public string instanceID;
        [SerializeField] public long startTime;
        [SerializeField] public long finishTime;

        public Effect SetInstanceID(string id)
        {
            instanceID = id;
            return this;
        }

        public static string GenerateInstanceID(Character character, Effect effect)
        {
            return character.ID + ":" + effect.name + DateTime.Now.Ticks;

        }

    }

    [SerializeField] public ResourceType type;
    [SerializeField] public Effect effect;
    [SerializeField] private float amount;

    public float Amount { get { return amount; } set { 
            if (IsRecipeType(type))
            { 
                if(value > 1)
                {
                    amount = 1;
                    return;

                }

            }
            else
            {
                amount = value;

            }
            } }
    public override string ToString()
    {
        return ($"ID : {id}, Name : {name}, Rarity : {rarity}, Description : {description}, Type : {type}. ");
    }

}
