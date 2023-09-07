using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

[RequireComponent (typeof(Animator), typeof(Collider))]
public class Enemy : Slime
{
    [SerializeField] private GameObject _deathPSPrefab;
    [SerializeField] private Slider _healthBar;
    private Collider _collider;
    private SlimeMovement _movement;
    private Player _player;
    [SerializeField]private int _maxHealth;
    private int _health;
    [SerializeField] private int _damage;
    public int Damage => _damage;

    public void Init(int damage, int health)
    {
        _collider = GetComponent<BoxCollider>();
        _movement = GetComponent<SlimeMovement>();
        _player = FindObjectOfType<Player>();
        _player.OnPlayerDie += OnPlayerDie;
        _player.FindEnemy();
        _maxHealth = health;
        _damage = damage;
        _health = _maxHealth;
        _collider.enabled = false;
        StartCoroutine(CheckMinDistance());
    }

    private void OnPlayerDie()
    {
        _player = null;
        NotifyObservers(new Data(Actions.Jumping, 1));
        _movement.ChangeJumping(false);
        //StopAllCoroutines();
    }

    private IEnumerator CheckMinDistance()
    {
        if (_player == null)
            yield break;
        while ((Vector3.Distance(_player.transform.position, transform.position) > -1))
        {
            if (_player == null)
                yield break;
            if (Vector3.Distance(_player.transform.position, transform.position) <= Player.MIN_ENEMY_DISTANCE*0.7f)
            {
                NotifyObservers(new Data(Actions.Jumping, 0));
                _movement.ChangeJumping(false);
                StartCoroutine(MoveToPlayer());
                yield break;
            }
            yield return new WaitForSeconds(0.8f);
            if (_player == null)
                yield break;
        }  
    }

    private IEnumerator MoveToPlayer()
    {
        //angry anim
        if (_player == null)
            yield break;
        yield return new WaitForSeconds(0.5f);
        if (_player == null)
            yield break;
        float rotTime = 0.3f;
        gameObject.transform.DOLookAt(_player.transform.position, rotTime);
        yield return new WaitForSeconds(rotTime*1.1f);
        if (_player == null)
            yield break;
        rotTime = 0.1f;
        gameObject.transform.DOLookAt(_player.transform.position, rotTime);
        yield return new WaitForSeconds(rotTime*1.1f);
        if (_player == null)
            yield break;
        Vector3 playerPos = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);
        Vector3 selfPos = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(playerPos, selfPos);
        Vector3 moveTo = transform.TransformVector(new Vector3(GetRandom(-0.5f, -0.35f, 0.35f, 0.5f), 0, GetRandom(-0.5f, -0.35f, 0.35f, 0.5f) + distance * ((_player.transform.position.x > transform.position.x) ? -1 : 1)));
        gameObject.transform.DOBlendableMoveBy(moveTo, 1.5f);
        yield return new WaitForSeconds(2f);
        if (_player == null)
            yield break;
        gameObject.transform.DOLookAt(_player.transform.position, 0.1f);
        yield return new WaitForSeconds(0.5f);
        if (_player == null)
            yield break;
        StartCoroutine(AttackPlayer());
    }

    private float GetRandom(float min1, float max1, float min2, float max2)
        => (Random.Range(0, 2) == 0) ? Random.Range(min1, max1) : Random.Range(min2, max2);

    private IEnumerator AttackPlayer()
    {
        while (_health > 0)
        {
            if (_player == null)
                yield break;
            float distance = Vector3.Distance(_player.transform.position, transform.position);
            if (distance > 1)
            {
                Debug.LogWarning(gameObject.name + ": Moved again!");
                StartCoroutine(MoveToPlayer());
                yield break;
            }
            Vector3 punchVector = Vector3.forward * distance;
            _collider.enabled = true;
            //gameObject.transform.DOPunchPosition(transform.TransformVector(new Vector3(punchVector.x, punchVector.y, punchVector.z - 0.6f)), 0.433f, 0);
            NotifyObservers(new Data(Actions.Attacking, 0));
            yield return new WaitForSeconds(0.3f);
            _collider.enabled = false;
            //anim set Attack Trigger
            yield return new WaitForSeconds(0.7f);
        }
    }

    public void GetHitted()
    {
        _health -= _player.Damage;
        _health = (_health < 0) ? 0 : _health;
        _healthBar.value = _health / (float)_maxHealth;
        NotifyObservers(new Data(Actions.Hitted, _health));
        NotifyObservers(new Data(Actions.DamageText, _player.Damage));
        if (_health == 0)
        {
            _healthBar.gameObject.SetActive(false);
            Death();
        }
    }

    private void Death()
    {
        StopAllCoroutines();
        var ps = Instantiate(_deathPSPrefab, GetComponentInChildren<EnemyHitArea>().gameObject.transform);
        ps.transform.parent = null;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.OnEnemyDie();
    }

}
