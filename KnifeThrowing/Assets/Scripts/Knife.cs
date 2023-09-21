using System;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private readonly Vector3 _knifePrefabPos = new Vector3(0, -3.38f, 105);
    
    private GameObject _desk;
    private Rigidbody2D _rb;
    [SerializeField]
    private Vector2 _move;
    private ParticleSystem _particleSystem;

    private bool _inDesk;

    public event Action<bool> OnGameEndAction;
    public event Action OnKnifeCreating;

    private void Start()
    {
        GetComponentInChildren<BoxCollider2D>().enabled = true;
        _inDesk = false;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _desk = FindObjectOfType<Desk>().gameObject;
        _rb = GetComponent<Rigidbody2D>();
        _move = new Vector2(_rb.velocity.x, LevelDesign.KnifeSpeed);
        _desk.GetComponent<Desk>().OnKnifeEnterEvent += OnDeskEnter;
    }

    private void OnDeskEnter()
    {
        GetComponentInChildren<BoxCollider2D>().enabled = false;
        _inDesk = true;
        _particleSystem.Play();
        _rb.velocity = new Vector2(0, 0);
        
        if (LevelDesign.KnifeCount > 0)
        {
            Instantiate(gameObject, _knifePrefabPos, Quaternion.identity);
            GetComponent<Knife>().enabled = false;
            OnKnifeCreating?.Invoke();
        }
        else
        {
            OnGameEndAction?.Invoke(true);
        }
        transform.parent = _desk.transform;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        _desk.GetComponent<Desk>().OnKnifeEnterEvent -= OnDeskEnter;
    }

    public void ThrowKnife()
    {
        _rb.velocity = _move;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "knife" && !_inDesk)
        {
            OnGameEndAction?.Invoke(false);
        }
    }

}
