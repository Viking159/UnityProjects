using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPS : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject, 0.3f);
    }

}
