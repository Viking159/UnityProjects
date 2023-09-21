using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsSpawner : MonoBehaviour
{
    private ParticleSystem _ps;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    public void SpawnCoins(int count)
    {
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ParticleSystem.MainModule main = _ps.main;
        main.duration = count / 9.0f; // PS spawns 9 coins per second
        _ps.Play();
    }
}
