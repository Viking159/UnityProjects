using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieD6Interactable : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonGO;

    private DiceRoller _diceRoller;
    private Actions _actions;

    private GameObject _border;
    private GameObject _lockImage;

    private bool _isChoosen = false;

    private void Start()
    {
        _diceRoller = FindObjectOfType<DiceRoller>();
        _actions = FindObjectOfType<Actions>();
        _actions.OnActionButtonPressedEvent += OnActionButtonPressed;
        _border = _buttonGO.transform.GetChild(0).gameObject;
        _lockImage = _buttonGO.transform.GetChild(1).gameObject;
    }

    public void ChangeChoose()
    {
        if (_diceRoller.IsRotating()) return;
        _isChoosen = !_isChoosen;
        _border.SetActive(!_border.activeInHierarchy);
        _lockImage.SetActive(!_lockImage.activeInHierarchy);
    }


    public void SetButtonActivation(bool val)
    {
        _buttonGO.SetActive(val);
    }


    public bool GetChoose() => _isChoosen;

    public void OnActionButtonPressed()
    {
        _isChoosen = false;
        _border.SetActive(false);
        _lockImage.SetActive(false);
        _buttonGO.SetActive(false);
    }

    public int GetDiceIndex() => (int.Parse(_buttonGO.name[_buttonGO.name.Length - 1].ToString()) - 1);
    public bool IsBorderActive() => _border.activeInHierarchy;

    private void OnDestroy()
    {
        _actions.OnActionButtonPressedEvent -= OnActionButtonPressed;
    }
}
