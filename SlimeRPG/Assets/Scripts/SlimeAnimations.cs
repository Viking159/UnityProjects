using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SlimeAnimations : MonoBehaviour, IObserver
{
    [SerializeField] private GameObject _slyakPrefab;
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private Canvas _canvas;

    private Animator _animator;

    public void OnNotify(Data data)
    {
        switch (data.action)
        {
            case Actions.Jumping:
                _animator.SetBool("IsJumping", System.Convert.ToBoolean(data.value));
                break;
            case Actions.Hitted:
                if (data.value > 0)
                    _animator.SetTrigger("IsHitted");
                else
                    _animator.SetTrigger("IsDead");
                break;
            case Actions.Attacking:
                _animator.SetTrigger("IsAttacking");
                break;
            case Actions.Slyak:
                SpawnSlyak();
                break;
            case Actions.DamageText:
                StartCoroutine(SpawnDamageText(data.value));
                break;
            case Actions.GameOver:
                ShowEndScreen();
                break;
        }
    }

    private void ShowEndScreen()
    {
        _endScreen.SetActive(true);
    }

    private IEnumerator SpawnDamageText(float val)
    {
        var text = Instantiate(_damageTextPrefab, _canvas.transform);
        text.GetComponent<TextMeshProUGUI>().text = $"-{val}";

        text.transform.DOLocalMove(new Vector3(-115f, -215f, 0f), 2f);
        text.transform.DOScale(1.5f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        text.transform.DOScale(0.5f, 1.5f);
        Destroy(text, 2f);
    }

    private void SpawnSlyak()
    {
        var slyak = Instantiate(_slyakPrefab, gameObject.transform);
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    private void OnEnable()
    {
        GetComponent<Slime>().AddObserver(this);
    }

    private void OnDisable()
    {
        var slime = GetComponent<Slime>();
        if (slime != null)
            slime.RemoveObserver(this);
    }

}
