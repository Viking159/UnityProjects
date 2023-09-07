using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemyPrefab;

    private const int OFFSET = 10;

    private int _damage;
    private int _health;
    private int _enemyCount;

    private void Start()
    {
        _enemyCount = 0;
        _damage = 1;
        _health = 5;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (_player.GetComponent<Player>().isActiveAndEnabled == false)
                yield break;
            var enemy = Instantiate(_enemyPrefab).GetComponent<Enemy>();
            enemy.transform.position = new Vector3(_player.transform.position.x + OFFSET + Random.Range(0f, 2f), 0, Random.Range(-2f, 2f));
            enemy.Init(_damage + _enemyCount / 5, _health + _enemyCount / 2);
            _enemyCount++;
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        }
    }

}
