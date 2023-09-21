using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent (typeof(Rigidbody2D))]
public class SpaceShuttle : MonoBehaviour
{
    [SerializeField] protected float maxSpeed; // 150
    [SerializeField] protected float acceleration; // 10
    [SerializeField] private float _breakPower; // 5

    protected Rigidbody2D rb;

    protected const float POWER_OFFSET = 0.33f;

    private float _currSpeed;
    private Vector2 _lastDir = Vector2.up;

    public virtual void Move(Vector2 direction)
    {
        float power = direction.sqrMagnitude;
        if (power <= POWER_OFFSET)
        {
            _currSpeed -= _breakPower;
            _currSpeed = (_currSpeed <= 0) ? 0 : _currSpeed;
            rb.velocity = _lastDir * _currSpeed * Time.fixedDeltaTime;
        }
        else
        {
            _lastDir = direction;
            _currSpeed += acceleration * Mathf.Abs(power);
            _currSpeed = (_currSpeed >= maxSpeed) ? maxSpeed : _currSpeed;
            rb.velocity = _currSpeed  * direction * Time.fixedDeltaTime;
        }
    }

    protected void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    protected void Rotate(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;
        if (direction.y >= 0)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * -direction.x));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 + 90 * direction.x));
    }

    protected virtual void Death()
    {
        Stop();
        Destroy(gameObject);
    }

}
