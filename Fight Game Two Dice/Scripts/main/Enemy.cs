using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;

public class Enemy : Character
{
    private Gold _gold;
    private EnemyStats _stats;

    public event Action OnEnemyDieActionLate;

    private void OnEnable()
    {
        IsGoodMove = -2;
        _stats = FindObjectOfType<EnemyStats>();
        _stats.OnStatsSet += SetStats;
        NormalPosition = new Vector3(0.7560104f, 0f, -10.99866f);
        EnemyCharacter = FindObjectOfType<Player>();
        Anim = GetComponent<Animator>();
        _gold = FindObjectOfType<Gold>();
        StartCoroutine(SetElements());
    }

    private IEnumerator SetElements()
    {
        yield return null;
        if (ArenaFightSettings.FightType == 1)
        {
            string json;
            if (ArenaFightSettings.League == 1) json = FightSettings.SpecialEnemie1Json;
            else if (ArenaFightSettings.League == 2) json = FightSettings.SpecialEnemie2Json;
            else json = FightSettings.SpecialEnemie2Json;
            GetComponent<ModulesShaker>().LoadElemets(JsonUtility.FromJson<LeagueEnemyGlads>(json).glads[ArenaFightSettings.SpecialEnemyNum].elements);
        }
        else
        {
            GetComponent<ModulesShaker>().RandomizeAll();
        }
        HasShield = (GetComponent<ModulesShaker>().GetShieldNum() == 200) ? false : true;
        HasSpear = GetComponent<ModulesShaker>().IsSpeadOn();
    }

    private void SetStats(int[] stats)
    {

        Health = stats[0] * 5;
        Energy = stats[1] * 5;
        Damage = stats[2];
        BlockedDamage = stats[3];
        MaxEnergy = Energy;
        Initialize();
        _stats.OnStatsSet -= SetStats;
    }

    public override void Dodge(int x)
    {
        AddEnergy(-10);
        int ranValue = UnityEngine.Random.Range(1, 7);
        if (ranValue > x)
        {
            CurrentActionName = Helper.EAction.NONE;
            CurrentActionValue = 0;
            return;
        }
        CurrentActionName = Helper.EAction.DODGE;
        CurrentActionValue = x;
        base.Dodge(x);
    }

    protected override void Death()
    {
        _gold.AddGold(50 * (++EnemySpawner.enemyCount));
        DiceRoller.ResetRerollCount();
        Invoke("DeathDeleyed", 2f);
    }

    private void DeathDeleyed()
    {
        OnEnemyDieActionLate?.Invoke();
    }
}
