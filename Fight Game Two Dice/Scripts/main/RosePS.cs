using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosePS : MonoBehaviour
{

    private ParticleSystem _ps;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    public void StartPlay()
    {
        _ps.Play();
    }

}
