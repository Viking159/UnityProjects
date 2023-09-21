using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSounds : MonoBehaviour
{
    [SerializeField] private AudioClip _shieldBlockSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _spearSound;
    [SerializeField] private AudioClip _swordSound;

    private AudioSource _audioSource;

    public enum SoundType
    {
        ShieldBlock,
        Hit,
        Spear,
        Sword
    };

    private void OnEnable()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(Destroyer());
    }

    public void PlaySound(SoundType sType)
    {
        switch (sType)
        {
            case SoundType.ShieldBlock:
                _audioSource.clip = _shieldBlockSound;
                break;
            case SoundType.Hit:
                _audioSource.clip = _hitSound;
                break;
            case SoundType.Spear:
                _audioSource.clip = _spearSound;
                break;
            case SoundType.Sword:
                _audioSource.clip = _swordSound;
                break;
        }
        _audioSource.Play();
    }

    private IEnumerator Destroyer()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
