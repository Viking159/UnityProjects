using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainController : MonoBehaviour
{
    [SerializeField]
    private Transform _trainObj;
    private GameObject[] _trainStepObjs;

    private bool _isEnabling;

    private byte _curTrainStage;

    private int _pinCount;

    private float _downTime;

    private void Start()
    {
        if (ArenaFightSettings.FightType != -2)
        {
            Destroy(_trainObj.gameObject);
            Destroy(this);
            return;
        }
        _trainObj.gameObject.SetActive(true);
        _curTrainStage = 0;
        _downTime = 0;
        _isEnabling = false;
        _trainStepObjs = new GameObject[_trainObj.childCount];
        for(int i = 0; i < _trainObj.childCount; i++)
        {
            _trainStepObjs[i] = _trainObj.GetChild(i).gameObject;
            _trainStepObjs[i].SetActive(false);
        }
        _pinCount = 0;
        StartCoroutine(StartNextStep(1.5f, true));
    }

    public void GoToNextStep(float timeDelay)
    {
        _trainStepObjs[_curTrainStage].SetActive(false);
        if (_curTrainStage == 23)
        {
            Destroy(_trainObj.gameObject);
            Destroy(this);
            return;
        }
        if (_curTrainStage == 17 || _curTrainStage == 19)
            _trainObj.GetComponent<Image>().enabled = false;
        if (_curTrainStage == 18 || _curTrainStage == 22)
            _trainObj.GetComponent<Image>().enabled = true;
        StartCoroutine(StartNextStep(timeDelay));   
    }
    
    private IEnumerator StartNextStep(float delay, bool isFirst = false)
    {
        yield return new WaitForSeconds(delay);
        if (isFirst == false)
            _curTrainStage++;
        _trainStepObjs[_curTrainStage].SetActive(true);
    }

    
    public void PinDice(int pinValue)
    {
        _pinCount++;
        if (_pinCount == pinValue)
        {
            GoToNextStep(0.5f);
            _pinCount = 0;
        }

    }

    
    public void CheckDiceDown()
    {
        _downTime = Time.time;
        _isEnabling = !_isEnabling;
    }

    public void CheckDiceUp()
    {
        if (Time.time - _downTime > 0.3f && _isEnabling)
            GoToNextStep(0.5f);
    }

}
