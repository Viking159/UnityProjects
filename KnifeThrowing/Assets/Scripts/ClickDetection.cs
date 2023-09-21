using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    private Knife _knife;
    private Desk _desk;

    public event Action OnClickEvent;

    private void Start()
    {
        _knife = FindObjectOfType<Knife>();
        _desk = FindObjectOfType<Desk>();
        _desk.OnKnifeEnterEvent += OnKnifeEnterDesk;
    }

    private void OnKnifeEnterDesk()
    {
        gameObject.SetActive(true);
    }

    public void Clicked()
    {
        if (_knife.isActiveAndEnabled == false)
            return;
        _knife.ThrowKnife();
        OnClickEvent?.Invoke();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_knife.enabled == false) GetKnife();
    }

    private void GetKnife()
    {
        Knife[] knifes = FindObjectsOfType<Knife>();
        _knife =  knifes[0];
    }
}
