using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualMenu : MonoBehaviour
{
    [Header ("Money visual")]
    [SerializeField] private Text _moneyText;

    [Header ("Exp visual")]
    [SerializeField] private Text _lvlText;
    [SerializeField] private Slider _expSlider;

    private void Start()
    {
        UpdateMoneyText();
        UpdateExpText();
    }


    public void UpdateMoneyText()
    {
        _moneyText.text = Money.MoneyCount.ToString();
    }

    public void UpdateExpText()
    {
        _lvlText.text = ProgressExp.Level.ToString();
        _expSlider.value = ProgressExp.ExpCount / (float)ProgressExp.MaxExpCount;
    }
}
