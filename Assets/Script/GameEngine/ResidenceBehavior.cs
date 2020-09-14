using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ResidenceBehavior : BuildingBehavior
{

    void Start()
    {
        InvokeRepeating("HealAllCharacters", Constant.TimeCycle.RESIDENCE_HEALING_CYCLE, Constant.TimeCycle.RESIDENCE_HEALING_CYCLE);

    }

    void BreedNewCharacter()
    {

    }

    protected override void ContinueFromOffline()
    {
        base.ContinueFromOffline();

        long offlineTimePassed = (DateTime.Now.Ticks - LoadManager.Instance.playerData.lastLoginTime) / TimeSpan.TicksPerSecond;
        int healedAmount = (int)offlineTimePassed / Constant.TimeCycle.RESIDENCE_HEALING_CYCLE;
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
