using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrain : MonoBehaviour
{
    [SerializeField]
    private Transform _trainObj;
    private GameObject[] _trainStepObjs;
    private byte _curTrainStage;

    private const string PPKEY = "Training";

    private void Start()
    {
        if (PlayerPrefs.GetInt(PPKEY) == 2)
        {
            Destroy(_trainObj.gameObject);
            Destroy(this);
            return;
        }
        _trainObj.gameObject.SetActive(true);
        _curTrainStage = 0;
        _trainStepObjs = new GameObject[_trainObj.childCount];
        for (int i = 0; i < _trainObj.childCount; i++)
        {
            _trainStepObjs[i] = _trainObj.GetChild(i).gameObject;
            _trainStepObjs[i].SetActive(false);
        }
        StartCoroutine(StartNextStep(1f, true));
    }

    public void GoToNextStep(float timeDelay)
    {
        if (_curTrainStage == 11) PlayerPrefs.SetInt(PPKEY, 2);
        _trainStepObjs[_curTrainStage].SetActive(false);
        if (_curTrainStage == 12)
        {
            Destroy(_trainObj.gameObject);
            Destroy(this);
            return;
        }
        StartCoroutine(StartNextStep(timeDelay));
    }

    private IEnumerator StartNextStep(float delay, bool isFirst = false)
    {
        yield return new WaitForSeconds(delay);
        if (isFirst == false)
            _curTrainStage++;
        _trainStepObjs[_curTrainStage].SetActive(true);
    }

}
