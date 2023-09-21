using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotifyData;

[RequireComponent (typeof(Collider2D))]
public sealed class PlayerShuttle : SpaceShuttle
{
    #region Observer Patter
    private List<IObserver> _observers = new List<IObserver>();
    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }
    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    private void NotifyObservers(Data data)
    {
        _observers.ForEach((obs) => obs.OnNotify(data));
    }
    #endregion
    [SerializeField] private GameObject _standartBulletPrefab;
    [SerializeField] private GameObject _superBulletPrefab;
    [SerializeField] private float _shootTimeDelta; // 0.5f
    [SerializeField] private float _superShootTimeDelta; // 5f

    private int _score;

    private float _fuel;
    private float _lostedFuel;
    private float _lastShootTime;
    private float _lastSuperShootTime;

    private const int MAX_LOSTED_FUEL = 1;
    private const int SCORE_PER_ENEMY = 5;
    private const int SCORE_PER_ASTEROID = 1;
    private const int FUEL_PER_ENEMY = 5;
    private const float FUEL_PER_MOVE = 0.07f; // 0.07f


    private void Start()
    {
        _lastShootTime = -_shootTimeDelta;
        _lastSuperShootTime = -_superShootTimeDelta;
        _score = 0;
        rb = GetComponent<Rigidbody2D>();
        _fuel = 100f;
        _lostedFuel = 0f;
        NotifyObservers(new Data(Action.Fuel, _fuel));
        NotifyObservers(new Data(Action.Score, _score));
    }

    public override void Move(Vector2 direction)
    {
        if (_fuel == 0)
        {
            Stop();
            return;
        }
        base.Move(direction);
        if (direction.sqrMagnitude <= POWER_OFFSET)
            return;
        _lostedFuel += FUEL_PER_MOVE;
        if (_lostedFuel >= MAX_LOSTED_FUEL)
        {
            _fuel -= MAX_LOSTED_FUEL;
            _fuel = (_fuel <= 0) ? 0 : _fuel;
            NotifyObservers(new Data(Action.Fuel, _fuel));
            _lostedFuel = 0;
        }

    }

    public void Rotate(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public void Shoot(bool standartBuller)
    {
        if (Time.time - _lastShootTime >= _shootTimeDelta && standartBuller)
        {
            Instantiate(_standartBulletPrefab, transform.position, transform.rotation).GetComponent<Bullet>().Init(transform.TransformDirection(Vector2.up));
            _lastShootTime = Time.time;
        }
        if (Time.time - _lastSuperShootTime >= _superShootTimeDelta && standartBuller == false)
        {
            Instantiate(_superBulletPrefab, transform.position, transform.rotation).GetComponent<Bullet>().Init(transform.TransformDirection(Vector2.up));
            _lastSuperShootTime = Time.time;
        }
    }

    

    public void DestroyedObject(bool enemy)
    {
        if (enemy)
        {
            _fuel += FUEL_PER_ENEMY;
            _fuel = (_fuel >= 100) ? 100 : _fuel;
            NotifyObservers(new Data(Action.Fuel, _fuel));
        }
        _score += (enemy)? SCORE_PER_ENEMY : SCORE_PER_ASTEROID;
        NotifyObservers(new Data(Action.Score, _score));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyShuttle>() != null || collision.GetComponent<Asteroid>() != null)
        {
            Death();
        }
    }

    protected override void Death()
    {
        NotifyObservers(new Data(Action.EndGame, 0));
        base.Death();
    }
}
