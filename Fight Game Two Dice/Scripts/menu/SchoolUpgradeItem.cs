using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchoolUpgradeItem : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _description;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _valueText;
    [SerializeField] private Text _priceText;
    [SerializeField] private Button _upgradeBtn;

    [SerializeField] private SchoolUpgradesSO _itemInfo;

    private int _level;

    private readonly Color grayColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private void OnEnable()
    {
        _upgradeBtn.onClick.AddListener(UpgradeStat);
        _level = SchoolUpgradeDataSaver.GetLevelData(_itemInfo.StatNum);
        UpdateVisual();
        CheckMaxLevel();
    }

    private void CheckMaxLevel()
    {
        _upgradeBtn.GetComponent<Image>().color = (_level < _itemInfo.MaxLevel) ? Color.white : grayColor;
        _upgradeBtn.enabled = _level < _itemInfo.MaxLevel;
        if (_level == _itemInfo.MaxLevel)
            _priceText.text = "-";
    }

    private void UpdateVisual()
    {
        _title.text = _itemInfo.Title;
        _description.text = _itemInfo.Description;
        _levelText.text = _level.ToString();
        _valueText.text = _itemInfo.CalcValue(_level).ToString() + ((_itemInfo.MeasureUnit == MeasureUnit.Percent) ? "%" : "");
        _priceText.text = _itemInfo.CalcPrice(_level).ToString();
    }

    private void UpgradeStat()
    {
        int price = _itemInfo.CalcPrice(_level);
        if (Money.MoneyCount < price)
            return;
        Money.Change(-price);
        _level++;
        SchoolUpgradeDataSaver.UpgradeStat(_itemInfo.StatNum, _level);
        UpdateVisual();
        CheckMaxLevel();
    }

    private void OnDisable()
    {
        _upgradeBtn.onClick.RemoveListener(UpgradeStat);
    }
}
