using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slyak : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = new Vector3(Random.Range(0.05f, 0.1f), 0.01f, Random.Range(0.05f, 0.1f));
        transform.parent = null;
        Destroy(gameObject, 3f);
    }
}
