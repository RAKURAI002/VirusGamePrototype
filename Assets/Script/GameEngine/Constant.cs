using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Constant
{
    public static class ItemRarityChance
    {
        public static float COMMON_DROP_CHANCE = 45;
        public static float UNCOMMON_DROP_CHANCE = 30;
        public static float RARE_DROP_CHANCE = 15;
        public static float SUPERRARE_DROP_CHANCE = 7;
        public static float ULTRARARE_DROP_CHANCE = 2.5f;
        public static float MYTHOLOGICALRARE_DROP_CHANCE = 0.5f;

    }
    public static class BirthMarkAmountChance
    {
        public static int BIRTHMARK_GET_ZERO_CHANCE = 20;
        public static int BIRTHMARK_GET_ONE_CHANCE = 40;
        public static int BIRTHMARK_GET_TWO_CHANCE = 30;
        public static int BIRTHMARK_GET_THREE_CHANCE = 10;

    }
    public static class BirthMarkTierChance
    {
        public static int BIRTHMARK_TIER_ONE_CHANCE = 35;
        public static int BIRTHMARK_TIER_TWO_CHANCE = 28;
        public static int BIRTHMARK_TIER_THREE_CHANCE = 20;
        public static int BIRTHMARK_TIER_FOUR_CHANCE = 12;
        public static int BIRTHMARK_TIER_FIVE_CHANCE = 5;

    }
    public static class BirthMarkLevelChance
    {
        public static int BIRTHMARK_LEVEL_ONE_CHANCE = 20;
        public static int BIRTHMARK_LEVEL_TWO_CHANCE = 20;
        public static int BIRTHMARK_LEVEL_THREE_CHANCE = 20;
        public static int BIRTHMARK_LEVEL_FOUR_CHANCE = 20;
        public static int BIRTHMARK_LEVEL_FIVE_CHANCE = 20;

    }

    /// <summary>
    /// In s(second) unit.
    /// </summary>
    public static class TimeCycle
    {
        public static int RESOURCE_UPDATE_CYCLE = 60;
        public static int CHARACTER_ADDING_EVENT_CYCLE = 60;

    }
    public static class CalculatingFunction
    {
        public static Func<int, int> CharacterJoiningChanceCalculation = characterWaitingAmount => { return 40 / ((characterWaitingAmount / 5) + 1); };



    }


    public static class IDMask
    {
        public static int BUILDING_ID_MASK = 1000;
        public static int CHARACTER_ID_MASK = 2000;
        public static int QUEST_ID_MASK = 3000;
        public static int EQUIPMENT_ID_MASK = 4000;
        public static int RESOURCE_ID_MASK = 5000;

    }

}
