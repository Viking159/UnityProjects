using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrade : MonoBehaviour, IObserver
{
    [SerializeField] private UpgradeSO _upgradeInfo;
    [SerializeField] private Player _player;

    [SerializeField] private Image _upgrageImage;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeLvlValueText;
    [SerializeField] private TextMeshProUGUI _upgradeAddValueText;
    [SerializeField] private TextMeshProUGUI _upgradePriceValueText;
    [Space]

    [Header("Buy button & colors")]
    [SerializeField] private Image _upgradeBuyButtonImage;
    [SerializeField] private Color _lockColor;
    [SerializeField] private Color _unlockColor;

   
    private void Start()
    {
        _upgradeNameText.text = _upgradeInfo.Name;
        _upgrageImage.sprite = _upgradeInfo.Icon;
        CheckUpgradePrice();
    }

    public void OnNotify(Data data)
    {
        if (data.action == _upgradeInfo.Action)
        {
            UpdateUpgrade(data.values);
        }
        if (data.action == Actions.Coins)
        {
            CheckUpgradePrice();
        }
    }

    private void CheckUpgradePrice()
    {
        _upgradeBuyButtonImage.color = (Coins.Instance.Count >= _upgradeInfo.Price) ? _unlockColor : _lockColor;
    }

    private void UpdateUpgrade(UpgradeValues values)
    {
        _upgradeInfo.Level = values.level;
        _upgradeInfo.Value = values.currValue;
        _upgradeInfo.AddValue = values.addValue;
        _upgradeInfo.Price = values.price;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        _upgradeLvlValueText.text = $"lvl {_upgradeInfo.Level}: {_upgradeInfo.Value}";
        _upgradeAddValueText.text = $"+{_upgradeInfo.AddValue}";
        _upgradePriceValueText.text = (_upgradeInfo.Price < 1000) ? _upgradeInfo.Price.ToString() : $"{System.Math.Round(_upgradeInfo.Price / 1000.0, 2)}K";
    }

    public void BuyUpgrade()
    {
        if (Coins.Instance.Count < _upgradeInfo.Price)
            return;
        Coins.Instance.AddMoney(-(int)_upgradeInfo.Price);
        _upgradeInfo.Level++;
        _upgradeInfo.Value += _upgradeInfo.AddValue;
        _upgradeInfo.Price += 10;
        UpdateVisual();
        _player.UpgradeStat(_upgradeInfo.Action, _upgradeInfo.Value);
    }

    private void OnEnable()
    {
        _player.AddObserver(this);
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.RemoveObserver(this);
    }

}
