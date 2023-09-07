using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    private Rigidbody _rb;
    private float _force;
    private bool _jumped;

    private readonly WaitForSeconds jumpCheckWait = new WaitForSeconds(0.05f);

    private bool _movingForward;
    public bool IsJumping => _movingForward;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        Init();
        StartCoroutine(MoveForward());
    }

    private void Init()
    {
        _movingForward = true;
        _force = 3.5f;
        _jumped = false;
    }

    public void ChangeJumping(bool value)
    {
        _movingForward = value;
        if (value)
            StartCoroutine(MoveForward());
    }

    public void Jumped()
    {
        _jumped = true;
    }

    private IEnumerator MoveForward()
    {
        while (_movingForward)
        {
            if (_jumped == true)
            {
                _rb.velocity = Vector3.zero;
                _jumped = false;
                _rb.AddForce(_direction * _force, ForceMode.Impulse);
            }
            yield return jumpCheckWait;
        }
    }
}
