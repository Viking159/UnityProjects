using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : Slime
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _handTranform;
    [SerializeField] private float _bulletForce;
    [SerializeField] private GameObject[] _cams;

    private SlimeMovement _movement;
    private Enemy _nearestEnemy;

    private float _startPosX;
    private int _maxHealth;
    private int _health;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackSpeed;
    private float _lastShootTime;

    public const int MIN_ENEMY_DISTANCE = 3;
    public const int MAX_ATTACK_DISTANCE = 5;

    public int Damage => _damage;

    public event System.Action OnPlayerDie;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _lastShootTime = Time.time;
        _startPosX = transform.position.x;
        _maxHealth = 3;
        _health = _maxHealth;
        _damage = 2;
        _attackSpeed = 1.5f;
        _movement = GetComponent<SlimeMovement>();
        NotifyObservers(new Data(Actions.Damage, new UpgradeValues(1, _damage, 5, 5)));
        NotifyObservers(new Data(Actions.AttackSpeed, new UpgradeValues(1, _attackSpeed, 0.5f, 15)));
        NotifyObservers(new Data(Actions.Distance, 0));
        NotifyObservers(new Data(Actions.Coins, Coins.Instance.Count));
        NotifyObservers(new Data(Actions.Health, _health/(float)_maxHealth));
        StartCoroutine(CheckEnemyDistance());
        StartCoroutine(CalculateDistance());
    }

    public void Attack()
    {
        if (Time.time - _lastShootTime < 1f / _attackSpeed)
            return;
        Debug.Log(Time.time - _lastShootTime);
        _lastShootTime = Time.time;
        var bulet = Instantiate(_bulletPrefab, _handTranform);
        bulet.transform.parent = null;
        bulet.GetComponent<Bullet>().SetGoal(GetBulletEndPosition());
    }

    private Vector3 GetBulletEndPosition()
    {
        if (_nearestEnemy == null)
            return new Vector3(transform.position.x + Random.Range(5, 25), 0, transform.position.z + Random.Range(-10, 10));
        var dist = Vector3.Distance(transform.position, _nearestEnemy.transform.position);
        if  (dist > MAX_ATTACK_DISTANCE)
            return new Vector3(transform.position.x + Random.Range(5, 25), 0, transform.position.z + Random.Range(-10, 10));
        return _nearestEnemy.transform.position;
    }

    private void GetNearestEnemy()
    {
        if (FindObjectOfType<Enemy>() == null)
        {
            _nearestEnemy = null;
            return;
        }
        var enemies = FindObjectsOfType<Enemy>().ToList<Enemy>();
        enemies.Sort(
            (e1, e2) => Vector3.Distance(transform.position, e1.transform.position).CompareTo(
                Vector3.Distance(transform.position, e2.transform.position))
            );
        _nearestEnemy = enemies[0];
    }

    private IEnumerator CalculateDistance()
    {
        while (_movement.IsJumping)
        {
            NotifyObservers(new Data(Actions.Distance,(int)(2 * (transform.position.x - _startPosX))));
            yield return new WaitForSeconds(0.35f);
        }
    }

    private IEnumerator CheckEnemyDistance()
    {
        while (_health > 0)
        {
            yield return new WaitForSeconds(0.8f);
            if (_nearestEnemy == null)
            {
                if (_movement.IsJumping == false)
                {
                    NotifyObservers(new Data(Actions.Jumping, 1));
                    _movement.ChangeJumping(true);
                    StartCoroutine(CalculateDistance());
                }
                continue;
            }
            float distance = Vector3.Distance(transform.position, _nearestEnemy.transform.position);
            if (distance <= MIN_ENEMY_DISTANCE)
            {
                NotifyObservers(new Data(Actions.Jumping, 0));
                _movement.ChangeJumping(false);
            }
            else
            {
                NotifyObservers(new Data(Actions.Jumping, 1));
                _movement.ChangeJumping(true);
                StartCoroutine(CalculateDistance());
            }

        }
    }

    private void GetDamage(int damage)
    {
        _health -= damage;
        _health = (_health < 0) ? 0 : _health;
        NotifyObservers(new Data(Actions.Hitted, _health));
        NotifyObservers(new Data(Actions.DamageText, damage));
        NotifyObservers(new Data(Actions.Health, _health/(float)_maxHealth));
        if (_health == 0)
            Death();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_health == 0)
            return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("EnemyAttacked!");
            if (enemy != null)
                GetDamage(enemy.Damage);
        }
    }

    public void FindEnemy()
    {
        if (_nearestEnemy == null)
            GetNearestEnemy();
    }

    public void OnEnemyDie()
    {
        Coins.Instance.AddMoney(10);
        NotifyObservers(new Data(Actions.Coins, Coins.Instance.Count));
        _nearestEnemy = null;
        FindEnemy();
    }

    public void UpgradeStat(Actions action, float value)
    {
        NotifyObservers(new Data(Actions.Coins, Coins.Instance.Count));
        switch (action)
        {
            case Actions.Damage:
                _damage = (int)value;
                break;
            case Actions.AttackSpeed:
                _attackSpeed = value;
                break;
        }
    }

    public void Death()
    {
        _cams[1].transform.parent = null;
        _cams[0].SetActive(false);
        _cams[1].SetActive(true);
        NotifyObservers(new Data(Actions.GameOver, 0));
        OnPlayerDie?.Invoke();
        enabled = false;
    }

}
