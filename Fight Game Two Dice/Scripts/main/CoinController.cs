using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(DestroyCoin());
    }

    private IEnumerator DestroyCoin()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
