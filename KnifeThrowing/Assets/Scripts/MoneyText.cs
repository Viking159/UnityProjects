using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{
    private static Text _moneyText;

    private void Start()
    {
        _moneyText = GetComponent<Text>();
        UpdateText(MoneyCount.GetMoney(true));
    }

    public static void UpdateText(int moneyValue)
    {
        _moneyText.text = moneyValue.ToString();
    }
        

}
