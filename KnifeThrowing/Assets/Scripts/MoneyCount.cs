using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    private const int MONEY_VALUE_NORMAL = 5;
    private const int MONEY_VALUE_ACORN = 10;

    private static int _moneyCount;

    public int moneyCount { get { return _moneyCount; } set { _moneyCount = value; } }

    private static int _moneyEarned;

    private Desk _desk;
    private Acorn _acorn;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MoneyCount")) _moneyCount = PlayerPrefs.GetInt("MoneyCount");
        else _moneyCount = 0;
        _moneyEarned = 0;
    }

    private void Start()
    {
        _moneyEarned = 0;
        _desk = FindObjectOfType<Desk>();
        _desk.OnKnifeEnterEvent += OnKnifeEnterDesk;
        _acorn = FindObjectOfType<Acorn>();
        if (_acorn != null) _acorn.OnKnifeEnterEvent += OnKnifeEnterAcorn;
    }

    private void OnKnifeEnterAcorn(float acornScale)
    {
        _acorn.OnKnifeEnterEvent -= OnKnifeEnterAcorn;
        int addMoney = (int)(MONEY_VALUE_ACORN * (1 / acornScale));
        AddMoney(addMoney);
    }

    private void OnKnifeEnterDesk()
    {
        AddMoney(MONEY_VALUE_NORMAL);
    }

    public void AddMoney(int addValue)
    {
        _moneyEarned += addValue;
        MoneyText.UpdateText(_moneyEarned);
    }

    public static int GetMoney(bool isEarned = true) 
    { 
        return isEarned? _moneyEarned : _moneyCount; 
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("MoneyCount", GetMoney(false));
    }


}
