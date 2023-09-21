using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Asteroid : MonoBehaviour
{
    [SerializeField] private GameObject _asteroidPrefab;
    private Rigidbody2D _rb;
    [SerializeField] private bool _isSmall;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 startPos, float scale, Vector2 direction, float speed, bool isSmall = false)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        _isSmall = isSmall;
        transform.position = startPos;
        Move(direction * speed);
    }

    private void Move(Vector2 direction)
    {

        _rb.velocity = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() != null || collision.GetComponent<PlayerShuttle>() != null)
        {
            FindObjectOfType<PlayerShuttle>()?.DestroyedObject(false);
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        if (_isSmall == false)
        {
            for(int i = 0; i < 3; i++)
                Instantiate(_asteroidPrefab).GetComponent<Asteroid>().Init(
                    transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0),
                    Random.Range(0.3f, 0.6f), 
                    new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized,
                    Random.Range(1f, 4f),
                    true);
        }
        Destroy(gameObject);
    }


}
