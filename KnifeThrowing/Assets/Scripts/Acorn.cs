using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Acorn : MonoBehaviour
{
    private Rigidbody2D _rb;
    private ParticleSystem _particleSystem;

    private Vector2 _move;

    private float _startX;
    [SerializeField]
    private float _speed;
    private float _scale;
    [SerializeField]
    private int _direction;

    public event Action<float> OnKnifeEnterEvent;

    private void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _rb = GetComponent<Rigidbody2D>();

        _scale = Random.Range(0.5f, 1.5f);
        _speed = Random.Range(3f, 10f);
        _startX = Random.Range(-10f, -4f);
        _direction = (_startX > 0) ? -1 : 1;
        _move = new Vector2(_speed, _rb.velocity.y);

        
        transform.position = new Vector3(_startX, -0.55f, 0);
        transform.localScale *= _scale;
    }

    private void Update()
    {
        _rb.velocity = _move * _direction;
        if ((_direction > 0 && transform.position.x >= 4f) || 
            (_direction < 0 && transform.position.x <= -4f)) 
            _direction = -_direction;
    }

    [Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "normalCollider")
        {
            _particleSystem.Play();
            OnKnifeEnterEvent?.Invoke(_scale);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, _particleSystem.startLifetime);
        }
    }

}
