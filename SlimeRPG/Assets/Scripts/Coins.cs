using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{

    #region Singleton
    private static Coins _instance;
    public static Coins Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        _coinsCount = 10;
    }
    #endregion

    private int _coinsCount;
    public int Count => _coinsCount;

    public void AddMoney(int value)
    {
        if (_coinsCount + value < 0)
            return;
        _coinsCount += value;
    }
}
