using System;
using UnityEngine;

public class CharacterStats 
{

    private string[] _statsName;
    private int[] _statsPrice;
    private float[] _statsValue;

    public CharacterStats(int attackValue, float critValue, int healthValue, int defenseValue)
    {
        _statsName = new string[4] { "ATTACK", "CRIT VALUE", "HEALTH", "DEFENSE" };
        _statsPrice = new int[4] { 100, 100, 100, 100 };
        _statsValue = new float[4] { attackValue, critValue, healthValue, defenseValue };
    }

    public void BuyStat(int index)
    {
        switch (index)
        {
            case 0:
            case 1:
                _statsValue[index] += 10;
                _statsPrice[index] = Mathf.CeilToInt(_statsPrice[index] * 1.2f);
                break;

            /*case 1:
                _statsValue[index] += 0.2f;
                _statsPrice[index] = Mathf.CeilToInt(_statsPrice[index] * 1.2f);
                break;*/

            case 2:
                _statsValue[index] += 20;
                _statsPrice[index] = Mathf.CeilToInt(_statsPrice[index] * 1.2f);
                break;

            case 3:
                _statsValue[index] += 10;
                _statsPrice[index] = Mathf.CeilToInt(_statsPrice[index] * 1.2f);
                break;

        }
    }

    public int GetPrice(int index) => _statsPrice[index];
    public string GetName(int index) => _statsName[index]; 
    public float[] GetStats() => _statsValue; 
    public int[] GetStatsPrice() => _statsPrice;
}
