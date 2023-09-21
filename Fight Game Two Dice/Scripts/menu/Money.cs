using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    private static VisualMenu _visualMenu;

    public static int MoneyCount { get; private set; }
    private void Awake()
    {

        _visualMenu = FindObjectOfType<VisualMenu>();
        if (Debug.isDebugBuild)
            MoneyCount = PlayerPrefs.GetInt("MoneyCount", 100000);
        else
            MoneyCount = PlayerPrefs.GetInt("MoneyCount", 250);
        _visualMenu.UpdateMoneyText();
        SaveMoneyCount();
    }

    public static void Change(int value)
    {
        if (MoneyCount + value < 0) return;
        MoneyCount += value;
        if (Debug.isDebugBuild)
            if (MoneyCount < 20000) MoneyCount = 100000;
        if (_visualMenu != null) _visualMenu.UpdateMoneyText();
        SaveMoneyCount();
    }

    private static void SaveMoneyCount()
    {
        PlayerPrefs.SetInt("MoneyCount", MoneyCount);
    }

    private void OnApplicationPause(bool pause)
    {
        SaveMoneyCount();
    }

    private void OnDestroy()
    {
        SaveMoneyCount();
    }

}
