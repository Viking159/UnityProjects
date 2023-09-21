using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceButton : MonoBehaviour
{
    [SerializeField]
    private DieD6Interactable[] _dices;
    private DieD6Interactable _diceSelf;
    private DiceRoller _diceRoller;
    private int _diceNum;
    private float _downTime;
    private bool _isDown;

    private void Start()
    {
        _diceRoller = FindObjectOfType<DiceRoller>();
        _diceSelf = GetComponent<DieD6Interactable>();
        _isDown = false;
    }

    public void Down()
    {
        _diceSelf.ChangeChoose();
        _isDown = false;
        StopCoroutine(CheckDownTime());
        _downTime = Time.time;
        _isDown = true;
        StartCoroutine(CheckDownTime());
    }

    private IEnumerator CheckDownTime()
    {
        while (_isDown)
        {
            if (Time.time - _downTime > 0.3f)
            {
                _isDown = false;
                _downTime = 0;
                ActiveSameDices();
                yield break;
            }
            yield return null;
        }
    }

    public void Up()
    {
        _isDown = false;
        _downTime = 0;
    }

    private void ActiveSameDices()
    {
        bool activePos = _diceSelf.IsBorderActive();
        int index = _diceSelf.GetDiceIndex();
        Helper.EAction res = _diceRoller.GetDiceValue(index);
        for (int i = 0; i < 5; i++)
        {
            if (i == index) continue;
            if (_diceRoller.GetDiceValue(i) == res)
            {
                if (_dices[i].IsBorderActive() != activePos)
                    _dices[i].ChangeChoose();
            }
        }
    }
}
