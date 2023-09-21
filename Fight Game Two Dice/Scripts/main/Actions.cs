using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class Actions : MonoBehaviour
{

    [SerializeField] private GameObject _dices;
    [SerializeField] private GameObject _yesNoDice;

    public static bool YesNoDiceActive = false;

    private Player _player;
    private DiceRoller _diceRoller;

    public Action OnActionButtonPressedEvent;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _diceRoller = FindObjectOfType<DiceRoller>();
    }

    public void AttackButton()
    {
        if (DiceAnalysis.AddAttackFactor) DiceAnalysis.AddAttackFactor = false;
        _player.CurrentActionName = EAction.ATTACK;
        _player.CurrentActionValue = DiceAnalysis.result[EAction.ATTACK];
        OnActionButtonPressedEvent?.Invoke();
        _player.Attack(DiceAnalysis.result[EAction.ATTACK]);
        AfterActionDiceSettings();
    }

    public void BlockButton()
    {
        if (DiceAnalysis.AddAttackFactor) DiceAnalysis.AddAttackFactor = false;
        _player.CurrentActionName = EAction.BLOCK;
        _player.CurrentActionValue = DiceAnalysis.result[EAction.BLOCK];
        _player.Block(DiceAnalysis.result[EAction.BLOCK]);
        OnActionButtonPressedEvent?.Invoke();
        AfterActionDiceSettings();
    }

    public void DodgeButton()
    {
        DeactivateAllDiesButtons();
        if (DiceAnalysis.AddAttackFactor) DiceAnalysis.AddAttackFactor = false;
        _player.DodgeEnergy();
        _player.CurrentActionName = EAction.DODGE;
        _player.CurrentActionValue = DiceAnalysis.result[EAction.DODGE];
        _dices.SetActive(false);
        _yesNoDice.GetComponent<CustomYesNoDice>().SetEdgeYesNoSprites(DiceAnalysis.result[EAction.DODGE]);
        _yesNoDice.SetActive(true);
        YesNoDiceActive = true;
        Invoke("RollDodgeDice", 1f);
    }

    private void RollDodgeDice()
    {
        _diceRoller.YesNoRoll();
    }

    public IEnumerator DeactivateDodge()
    {
        yield return new WaitForSeconds(1f);
        _dices.SetActive(true);
        _yesNoDice.SetActive(false);
        AfterActionDiceSettings();
    }

    public void RestButton()
    {
        if (DiceAnalysis.AddAttackFactor) DiceAnalysis.AddAttackFactor = false;
        _player.CurrentActionName = EAction.REST;
        OnActionButtonPressedEvent?.Invoke();
        _player.Rest(DiceAnalysis.result[EAction.REST]);
        AfterActionDiceSettings();
    }

    public void NotEnoughEnergy()
    {
        OnActionButtonPressedEvent?.Invoke();
        AfterActionDiceSettings();
    }

    private void DeactivateAllDiesButtons()
    {
        for(int i = 0; i < 5; i++)
        {
            DieD6Interactable diceInteractable = _dices.transform.GetChild(i).GetComponent<DieD6Interactable>();
            diceInteractable.OnActionButtonPressed();
        }
    }

    private void AfterActionDiceSettings()
    {
        _diceRoller.ReRoll = false;
        _diceRoller.ResetRerollCount();
    }

   
}
