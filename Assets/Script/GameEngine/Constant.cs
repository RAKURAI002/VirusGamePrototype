using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Constant
{
    public static class QuestMechanicConstant
    {
        public static int BASE_DODGE_CHANCE = 20;

        public static int DODGE_CHANCE_AD = 2;
        public static int MAX_ACCURACY_AD = 18;
        public static int MIN_ACCURACY_AD = 0;

        public static int DODGE_CHANCE_DA = 85;
        public static int MAX_ACCURACY_DA = 65;
        public static int MIN_ACCURACY_DA = 0;

        public static int RETREAT_CHANCE = 20;
        public static int RUNAWAY_CHANCE = 30;
        public static int CRITICAL_CHANCE = 10;
        public static int WKDAMAGE_CHANCE = 15;
    }
    public static class ItemRarityChance
    {
        public static float COMMON_DROP_CHANCE = 45;
        public static float UNCOMMON_DROP_CHANCE = 30;
        public static float RARE_DROP_CHANCE = 15;
        public static float SUPERRARE_DROP_CHANCE = 7;
        public static float ULTRARARE_DROP_CHANCE = 2.5f;
        public static float MYTHOLOGICALRARE_DROP_CHANCE = 0.5f;

    }
    public static class EventOccurChance
    {
        public static int BREEDING_CHANCE = 50;
    
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
        public static int RESOURCE_UPDATE_CYCLE = 10;
        public static int CHARACTER_ADDING_EVENT_CYCLE = 60;
        public static int GENERAL_GAME_CYCLE = 10;
        public static int PREGNANCY_GIVE_BIRTH_TIME = 30;
        public static int CHILD_GROWING_TIME = 60;
    }
    public static class CalculatingFunction
    {
        public static Func<int, int> CharacterJoiningChanceCalculation = characterWaitingAmount => { return 40 / ((characterWaitingAmount / 5) + 1); };

    }

    public static class FireBaseData
    {
        public static string WEB_CLIENT_ID = "503284986617-pfqma7n52qicbe78jd44psvpem1me8sk.apps.googleusercontent.com";
    }


    public static class IDMask
    {
        public static int BUILDING_ID_MASK = 100000;
        public static int CHARACTER_ID_MASK = 200000;
        public static int QUEST_ID_MASK = 300000;
        public static int EQUIPMENT_ID_MASK = 400000;
        public static int RESOURCE_ID_MASK = 500000;
        public static int ACHIEVEMENT_ID_MASK = 600000;
        public static int ACTIVITY_ID_MASK = 700000;

    }

}
