using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceAddStats : MonoBehaviour
{
    public Text[] _texts = new Text[3];
    private string[] _statsName;
    private int[] _statsPrice;

    private void Start()
    {
        _statsName = new string[3] { "ATTACK", "HEALTH", "DEFENSE" };
        _statsPrice = new int[3] { 100, 100, 100 };
    }

    public void UpdatePrice(int index)
    {
        _statsPrice[index] = Mathf.CeilToInt(_statsPrice[index] * 1.2f);
        _texts[index].text = _statsName[index] + "\n" + _statsPrice[index].ToString();
    }

    public int GetStatPrice(int index)
    {
        return _statsPrice[index];
    }
}
