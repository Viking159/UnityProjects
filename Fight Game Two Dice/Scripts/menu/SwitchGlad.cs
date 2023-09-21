using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;

public class SwitchGlad : MonoBehaviour
{
    [SerializeField] private Text[] _statsText;
    [SerializeField] private GameObject[] _swipeButtons;

    private ModulesShaker _modulesShaker;
    private OwnGlads _ownGlads;
    private School _school;

    private int _gladNum = 0;
    private OwnGladiator _glad;

    private readonly Color _greenColor = new Color(0.09803922f, 0.8196079f, 0.1882353f);
    private readonly Color _normalColor = new Color(0f, 0f, 0f);

    private void OnEnable()
    {
        _ownGlads = FindObjectOfType<OwnGlads>();
        if (_gladNum == 0) _swipeButtons[0].SetActive(false);
        if (_gladNum == _ownGlads.GetOwnGladsCount() - 1) _swipeButtons[1].SetActive(false);
        _modulesShaker = GetComponent<ModulesShaker>();
        _glad = _ownGlads.GetOwnGladiator(_gladNum);
        _school = FindObjectOfType<School>();
        SetNormalColor();
        SetGlad();
    }

    public void SetNormalColor()
    {
        for (int i = 0; i < _statsText.Length; i++)
            _statsText[i].color = _normalColor;
    }

    public void SetGreenColor(int statNum)
    {
        _statsText[statNum].color = _greenColor;
    }

    public void SetNormalColor(int statNum)
    {
        _statsText[statNum].color = _normalColor;
    }

    public void SetGlad(int gladNum = -1)
    {
        if (gladNum != -1) 
        {
            _gladNum = gladNum;
            if (_gladNum == 0) _swipeButtons[0].SetActive(false);
            if (_gladNum == _ownGlads.GetOwnGladsCount() - 1) _swipeButtons[1].SetActive(false);
        }
        _glad = _ownGlads.GetOwnGladiator(_gladNum);
        _modulesShaker.LoadElemets(_glad.elements);
        for (int i = 0; i < _statsText.Length; i++)
        {
            _statsText[i].text = (i < 2) ? (_glad.stats[i] * 5).ToString() : _glad.stats[i].ToString();
        }
        if (_school != null)
        {
            _school.Initialize(_glad, _gladNum);
        }
    }

    public void SetGlad(OwnGladiator glad)
    {
        for (int i = 0; i < _statsText.Length; i++)
            _statsText[i].text = (i < 2) ? (glad.stats[i] * 5).ToString() : glad.stats[i].ToString();
    }

    public void SetGladNum(bool right)
    {
        if (right && _gladNum < _ownGlads.GetOwnGladsCount() - 1)
        {
            _gladNum++;
            if (_gladNum == _ownGlads.GetOwnGladsCount() - 1) _swipeButtons[1].SetActive(false);
            if (_swipeButtons[0].activeInHierarchy == false) _swipeButtons[0].SetActive(true);
        }
        else if (right == false && _gladNum > 0)
        {
            _gladNum--;
            if (_gladNum == 0) _swipeButtons[0].SetActive(false);
            if (_swipeButtons[1].activeInHierarchy == false) _swipeButtons[1].SetActive(true);
        }
        SetGlad();
    }

    public  int GetGladNum() => _gladNum;
}
