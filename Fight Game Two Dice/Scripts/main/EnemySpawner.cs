using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    private Enemy _enemy;


    public static int enemyCount = 0;

    public event Action OnEnemyInstantientAction;

    private void Start()
    {
        enemyCount = 0;
        _enemy = FindObjectOfType<Enemy>();
        _enemy.OnEnemyDieActionLate += CreateEnemy;
    }

    private void CreateEnemy()
    {
        _enemy.OnEnemyDieActionLate -= CreateEnemy;
        if (enemyCount == ArenaFightSettings.EnemyCount) return;
        //if (enemyCount == 15) return;
        Destroy(_enemy.gameObject);
        _enemy = Instantiate(_enemyPrefab).GetComponent<Enemy>();
        _enemy.OnEnemyDieActionLate += CreateEnemy;
        OnEnemyInstantientAction?.Invoke();
    }

    private void OnDestroy()
    {
        _enemy.OnEnemyDieActionLate -= CreateEnemy;
        enemyCount = 0;
    }
}
