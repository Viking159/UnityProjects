using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketTimer : MonoBehaviour
{
    private MarketGlads _marketGlads;
    private TimeController _timeController;
    private Image _fillImg;

    private void OnEnable()
    {
        _marketGlads = FindObjectOfType<MarketGlads>();
        _timeController = FindObjectOfType<TimeController>();
        _fillImg = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        _fillImg.fillAmount = 1;
        _timeController.OnTick += UpdateFillAreaTimer;
        _marketGlads.OnMarketStartTimeSet += UpdateFillAreaTimer;
        UpdateFillAreaTimer(_timeController.GetCurrTime());
        //UpdateFillArea();
    }

    private void UpdateFillAreaTimer(int time)
    {
        if (_timeController.IsLocalTime) return;
        float timeLeft = _marketGlads.GetTimeLeft(time);
        _fillImg.fillAmount = timeLeft;
    }

    private void OnDisable()
    {
        if (_timeController != null)
            _timeController.OnTick -= UpdateFillAreaTimer;
        if (_marketGlads != null)
            _marketGlads.OnMarketStartTimeSet -= UpdateFillAreaTimer;
    }

}
