using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed; // 15
    [SerializeField] private bool _isSuperBullet;
    protected Rigidbody2D rb;

    public bool IsSuperBullet => _isSuperBullet;

    public void Init(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        rb.velocity = direction * _speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isSuperBullet == false && (collision.GetComponent<EnemyShuttle>() != null || collision.GetComponent<Asteroid>() != null)
            || collision.gameObject.layer == 3)
            Destroy(gameObject);
    }

}
