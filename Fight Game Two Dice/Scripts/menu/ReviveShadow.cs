using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveShadow : MonoBehaviour
{

    private CampReviveTime _campReviveTime;
    private TimeController _timeController;
    private Image _fillImg;
    private SwitchGlad _switchGlad;
    private void OnEnable()
    {
        _fillImg = GetComponent<Image>();
        _campReviveTime = FindObjectOfType<CampReviveTime>();
        _timeController = FindObjectOfType<TimeController>();
        _switchGlad = FindObjectOfType<SwitchGlad>();
        _timeController.OnTick += UpdateTimer;
        UpdateTimer(_timeController.GetCurrTime());
    }

    private void UpdateTimer(int time)
    {
        if (time == 0) return;
        if (_switchGlad == null) return;
        try
        {
            if (_campReviveTime.GetGlad(_switchGlad.GetGladNum()).isDead == false) return;
        }
        catch
        {
            return;
        }
        float timeLeft = _campReviveTime.GetTimeLeft(time, _switchGlad.GetGladNum());
        _fillImg.fillAmount = timeLeft;
    }

    private void OnDisable()
    {
        if (_timeController != null)
            _timeController.OnTick -= UpdateTimer;
    }
}
