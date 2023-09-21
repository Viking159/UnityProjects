using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private Joystick _joystick;
    [SerializeField] private PlayerShuttle _player;

    private Vector2 _velocity;

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (_player == null)
        {
            enabled = false;
            return;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            _velocity.y = 0;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _velocity.y = -1;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            _velocity.y = 1;
        }
        else
        {
            _velocity.y = 0;
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            _velocity.x = 0;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _velocity.x = -1;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            _velocity.x = 1;
        }
        else
        {
            _velocity.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _player.Shoot(true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _player.Shoot(false);
        }
    }

    private void FixedUpdate()
    {
        if (_player == null)
        {
            Debug.LogWarning($"FU: Player is null!");
            enabled = false;
            return;
        }
        _player.Move(_velocity.normalized);
        _player.Rotate(GetAngle());
    }

    private float GetAngle()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltaY = mousePos.y - _player.transform.position.y;
        float deltaX = mousePos.x - _player.transform.position.x;
        return Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX);
    }
}
