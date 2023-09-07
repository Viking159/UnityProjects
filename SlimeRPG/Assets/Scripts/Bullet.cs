using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{

    private Sequence _flySequence;

    private const float SHOOT_POWER = 0.3f;
    private const float SHOOT_OFFSET = 0.1f;
    private void Start()
    {
        Invoke(nameof(SelfDestroy), 0.33f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHitArea"))
        {
            SelfDestroy();
        }
    }

    public void SetGoal(Vector3 pos)
    {
        Vector3 predictPos = new Vector3(pos.x - SHOOT_OFFSET, pos.y + 0.1f, pos.z);
        float distance = Vector3.Distance(transform.position, predictPos);
        _flySequence = transform.DOJump(predictPos, SHOOT_POWER * distance, 1, 0.3f);
    }

    private void SelfDestroy()
    {
        _flySequence.Kill();
        Destroy(gameObject);
    }

}
