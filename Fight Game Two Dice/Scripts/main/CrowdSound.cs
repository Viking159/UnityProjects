using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSound : MonoBehaviour
{
    [SerializeField] private AudioClip _goodMood;
    [SerializeField] private AudioClip _badMood;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMood(bool good)
    {
        _audioSource.Stop();
        _audioSource.clip = (good) ? _goodMood : _badMood;
        _audioSource.Play();
    }
}
