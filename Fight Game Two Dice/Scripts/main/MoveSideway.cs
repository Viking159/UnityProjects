using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSideway : MonoBehaviour
{

    private float _speed;
    private float _duration;

    private int _direction;
    private bool _horizontalMove = false;
    private bool _verticalMove = false;
    private float _startTime;

    public int Direction { get { return _direction; } }

    public void Initialize(int charNum)
    {
        _speed = 1f;
        _duration = 2f;
        _direction = (charNum == 0) ? -1 : 1;
        _horizontalMove = true;
        _startTime = Time.time;
    }

    public void Initialize(bool comb)
    {
        if (comb)
        {
            _speed = 0.2f;
            _duration = 2.5f;
        }
        else
        {
            _speed = 1f;
            _duration = 2f;
        }
        _startTime = Time.time;
        _verticalMove = true;
    }

    private void Update()
    {
        if (_horizontalMove)
            transform.localPosition += Vector3.right * _direction * _speed;
        if (_verticalMove)
            transform.localPosition += Vector3.up * _speed;
        if (Time.time - _startTime > _duration)
            Destroy(gameObject);
    }

}
