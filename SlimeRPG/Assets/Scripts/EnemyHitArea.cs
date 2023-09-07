using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitArea : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            var self = GetComponentInParent<Enemy>();
            if (self != null)
                self.GetHitted();
        }
    }
}
