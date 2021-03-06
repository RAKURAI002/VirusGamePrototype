﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ResidenceBehavior : BuildingBehavior
{
    void Start()
    {
       
    }
    protected override void OnGameCycleUpdated()
    {
        base.OnGameCycleUpdated();
        if (builder.Level == 0)
        {
            return;
        }
        HealAllCharacters();
        BreedNewCharacter();

    }

    void BreedNewCharacter()
    {
        List<Character> characters = builder.CharacterInBuilding[0].Characters;

        if(characters.Count == 2 && characters[0].Gender != characters[1].Gender)
        {
            int random = UnityEngine.Random.Range(0, 100);
            int chance = buildingData.production[builder.Level]["BreedingChance"];
            Debug.Log($"{Constant.TimeCycle.GENERAL_GAME_CYCLE}s passed, Trying to start BreedNewCharacter Event.");
            Debug.Log($"Chance : 0 - {chance - 1}, Randomed : {random}. Start event = {(random < chance)}");
            if (random < chance)
            {
            }
            else
            {
                return;
            }

            Character female = characters.Single(c => c.Gender == Character.GenderType.Female);
            Character male = characters.Single(c => c.Gender == Character.GenderType.Male);

            if (female.workStatus == Character.WorkStatus.Pregnant)
            {
                return;

            }

            female.workStatus = Character.WorkStatus.Pregnant;
            
            NotificationManager.Instance.AddActivity(new ActivityInformation()
            {
                activityName = ($"Pregnancy:{female.Name}"),
                activityType = ActivityType.Pregnancy,
                startPoint = DateTime.Now.Ticks,
                requiredPoint = DateTime.Now.Ticks + (Constant.TimeCycle.PREGNANCY_GIVE_BIRTH_TIME * TimeSpan.TicksPerSecond),
                informationID = female.ID,
                informationID2 = male.ID
               
            });

        }

    }

    protected override void ContinueFromOffline()
    {
        base.ContinueFromOffline();

        long offlineTimePassed = (DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond;
        int healedAmount = (int)offlineTimePassed / Constant.TimeCycle.GENERAL_GAME_CYCLE;
        ForceHealAllCharacters(healedAmount);

    }
    void HealAllCharacters()
    {
        List<Character> characters = builder.CharacterInBuilding[0].Characters;

        int baseHealAmount = buildingData.production[builder.Level]["BaseHealAmount"];
        foreach (Character character in characters)
        {
            character.CurrentHitPoint += baseHealAmount;

        }

    }
    void ForceHealAllCharacters(int value)
    {
        List<Character> characters = builder.CharacterInBuilding[0].Characters;

        foreach (Character character in characters)
        {
            character.CurrentHitPoint += value;

        }
    }

}
