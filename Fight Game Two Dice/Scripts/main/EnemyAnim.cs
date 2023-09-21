using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (transform.position.x < 0.77f)
        {
            _anim.Play("Idle");
            _anim.applyRootMotion = false;
            transform.position = gameObject.GetComponent<Enemy>().NormalPosition;
            enabled = false;
        }
    }
}
