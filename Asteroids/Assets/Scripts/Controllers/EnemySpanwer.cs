using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpanwer : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private PlayerShuttle _player;

    private const int ENEMY_SPAWN_R = 12;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (_player != null)
        {
            yield return new WaitForSeconds(5 * Random.Range(1, 5));
            InstantiateEnemy();
        }
    }

    private void InstantiateEnemy()
    {
        Vector3 startPos = Vector3.zero;
        startPos.y = Random.Range(-ENEMY_SPAWN_R, (float)ENEMY_SPAWN_R);
        startPos.x = ((Random.Range(0, 2) == 0)? -1: 1) * Mathf.Sqrt(ENEMY_SPAWN_R * ENEMY_SPAWN_R - startPos.y * startPos.y);
        float maxSpeed = Random.Range(80, 130);
        Instantiate(_enemyPrefab).GetComponent<EnemyShuttle>().Init(_player, startPos, maxSpeed);
    }

}
