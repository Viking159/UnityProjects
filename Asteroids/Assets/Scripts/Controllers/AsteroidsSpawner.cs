using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [SerializeField] private PlayerShuttle _player;
    [SerializeField] private GameObject _asteroidPrefab;

    private void Start()
    {
        StartCoroutine(SpawnAsteroids());
    }

    private IEnumerator SpawnAsteroids()
    {
        while(_player != null)
        {
            InstantiateAsteroid();
            yield return new WaitForSeconds(Random.Range(1f, 3.5f));
        }
    }

    private void InstantiateAsteroid()
    {
        Vector3 startPos;
        Vector2 direction;
        
        int negative = (Random.Range(0, 2) == 0) ? -1 : 1;
        if (Random.Range(0, 2) == 0)
        {
            startPos = new Vector3(negative * -12f, Random.Range(-2.5f, 2.5f), 0);
            direction = new Vector2(negative * 1, Random.Range(-0.3f, 0.3f));
        }
        else
        {
            startPos = new Vector3(Random.Range(-4.5f, 4.5f), negative * 8f, 0);
            direction = new Vector2(Random.Range(-0.3f, 0.3f), negative * -1);
        }
        float scale = Random.Range(0.9f, 2f);
        float speed = Random.Range(1f, 4f);
        Instantiate(_asteroidPrefab).GetComponent<Asteroid>().Init(startPos, scale, direction, speed);
    }

}
