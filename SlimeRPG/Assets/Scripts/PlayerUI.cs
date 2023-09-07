using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerUI : MonoBehaviour, IObserver
{
    [Header("Player")]
    [SerializeField] Slime _playerSlime;
    [SerializeField] private Slider _healthSlider;
    [Space]

    [Header("Top text")]
    [SerializeField] private TextMeshProUGUI _distanceText; 
    [SerializeField] private TextMeshProUGUI _coinsText;

    public void OnNotify(Data data)
    {
        switch (data.action)
        {
            case Actions.Distance:
                UpdateDistanceText((int)data.value);
                break;
            case Actions.Coins:
                UpdateCoinsText((int)data.value);
                break;
            case Actions.Health:
                UpdateHealth(data.value);
                break;
            case Actions.Hitted:
                if (data.value == 0)
                    _healthSlider.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateDistanceText(int value)
    {
        _distanceText.text = (value < 1000) ? $"{value}m" : $"{System.Math.Round(value / 1000.0, 2)}km";
    }

    private void UpdateCoinsText(int value)
    {
        _coinsText.text = (value < 1000) ? value.ToString() : $"{System.Math.Round(value / 1000.0, 2)}K";
    }

    private void UpdateHealth(float value)
    {
        _healthSlider.value = value;
    }

    private void OnEnable()
    {
        _playerSlime.AddObserver(this);
    }

    private void OnDisable()
    {
        _playerSlime.RemoveObserver(this);
    }

    
}
