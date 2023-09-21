using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public event Action OnAddGoldEvent;
    private int _goldCount = 0;

    private void Start()
    {
        _goldCount = 0;
    }

    public int GetGold()
    {
        return _goldCount;
    }

    public void AddGold(int addCount)
    {
        _goldCount += addCount;
        OnAddGoldEvent?.Invoke();
    }

    private void OnDestroy()
    {
        _goldCount = 0;
    }
}
