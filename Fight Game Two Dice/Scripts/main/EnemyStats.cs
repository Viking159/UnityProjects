using System;
using UnityEngine;
using Structs;
using Random = UnityEngine.Random;

public class EnemyStats : MonoBehaviour
{
    private EnemySpawner _enemySpawner;

    public Action<int[]> OnStatsSet;

    private void Start()
    {
        SetStats();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _enemySpawner.OnEnemyInstantientAction += SetStats;
    }

    private void SetStats()
    {
        if (ArenaFightSettings.FightType == -2)
        {
            SetTrainStats();
            return;
        }
        if (ArenaFightSettings.FightType != 0) 
        {
            string json;
            if (ArenaFightSettings.League == 1) json = FightSettings.SpecialEnemie1Json;
            else if (ArenaFightSettings.League == 2) json = FightSettings.SpecialEnemie2Json;
            else json = FightSettings.SpecialEnemie3Json;
            int[] specStats = JsonUtility.FromJson<LeagueEnemyGlads>(json).glads[ArenaFightSettings.SpecialEnemyNum].stats;
            OnStatsSet?.Invoke(specStats);
            return; 
        }
        int minRange = 0, maxRange = 0;
        int[] range = new int[2];
        switch (ArenaFightSettings.League)
        {
            case 1:
                minRange = 1;
                maxRange = 10;
                break;
            case 2:
                minRange = 10;
                maxRange = 20;
                break;
            case 3:
                minRange = 20;
                maxRange = 30;
                break;
        }
        int[] stats = new int[4];
        stats[0] = Random.Range(minRange, maxRange + 1);
        int temp = Random.Range(minRange, maxRange + 1);
        stats[1] = temp == 1 ? 2 : temp;
        stats[2] = Random.Range(minRange, maxRange + 1);
        stats[3] = Random.Range(minRange, maxRange + 1);
        OnStatsSet?.Invoke(stats);
    }

    private void SetTrainStats()
    {
        int[] stats = new int[4];
        switch (EnemySpawner.enemyCount)
        {
            case 0:
                stats[0] = 3;
                stats[1] = 4;
                stats[2] = 3;
                stats[3] = 3;
                break;
            case 1:
                stats[0] = 5;
                stats[1] = 7;
                stats[2] = 5;
                stats[3] = 5;
                break;
            case 2:
                stats[0] = 25;
                stats[1] = 30;
                stats[2] = 50;
                stats[3] = 10;
                break;
            default:
                break;
        }
        OnStatsSet?.Invoke(stats);
    }

    private void OnDestroy()
    {
        if (_enemySpawner != null)
            _enemySpawner.OnEnemyInstantientAction -= SetStats;
    }

}
