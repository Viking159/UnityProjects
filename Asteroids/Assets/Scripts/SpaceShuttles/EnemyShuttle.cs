using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyShuttle : SpaceShuttle
{

    private float _health;
    
    private PlayerShuttle _player;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        _health = 3;
    }

    public void Init(PlayerShuttle player, Vector3 startPos, float maxSpeed)
    {
        base.maxSpeed = maxSpeed;
        transform.position = startPos;
        if (player == null)
        {
            Debug.LogWarning("PLAYER IS NULL!");
            Destroy(gameObject);
        }
        _player = player;
        StartCoroutine(FollowPlayer());
    }


    private void OnPlayerDeath()
    {
        StopAllCoroutines();
        Stop();
        this.enabled = false;
    }

    private IEnumerator FollowPlayer()
    {
        while (_player != null)
        {
            Vector2 direction = (_player.transform.position - transform.position).normalized * maxSpeed * Time.fixedDeltaTime;
            Move(direction);
            Rotate(direction);
            yield return new WaitForSeconds(0.5f);
        }
        OnPlayerDeath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            _health -= (bullet.IsSuperBullet) ? 3 : 1;
            if (_health <= 0)
                Death();
        }
        if (collision.GetComponent<PlayerShuttle>() != null)
        {
            _health = 0;
            Death();
        }
    }

    protected override void Death()
    {
        _player?.DestroyedObject(true);
        base.Death();
    }



}
