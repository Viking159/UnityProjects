using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class DiceAnalysis : MonoBehaviour
{
    private Player _self;
    private ActionButtons _actionButtons;
    private Actions _actions;

    public static Dictionary<EAction, int> result;
    public static bool AddAttackFactor = false;
    public static int AddAttackFactorValue = 0;

    private void Start()
    {
        _actions = FindObjectOfType<Actions>();
        _self = GetComponent<Player>();
        _actionButtons = FindObjectOfType<ActionButtons>();
    }

    public void CheckDice(EAction[] diceResult)
    {
        _self.IsGoodMove = -2;
        _self.EnemyCharacter.IsGoodMove = -2;
        result = new Dictionary<EAction, int>();
        for (int i = 0; i < diceResult.Length; i++)
        {
            if (result.ContainsKey(diceResult[i])) result[diceResult[i]]++;
            else result.Add(diceResult[i], 1);
        }
        CheckReward();
        if (AddAttackFactor)
        {
            if (result.ContainsKey(EAction.ATTACK)) result[EAction.ATTACK] += AddAttackFactorValue;
            else result.Add(EAction.ATTACK, AddAttackFactorValue);
        }
        _actionButtons.Activate(result);
    }

    private const int baseRewardLeague1 = 2;

    private void CheckReward()
    {
        Combination comb = Combination.NONE;
        _self.AddGoldValue = 0;
        foreach (var item in result)
        {
            if (item.Value == 1) continue;
            if (item.Value == 2 && comb == Combination.NONE) comb = Combination.PAIR;
            else if (item.Value == 2 && comb == Combination.PAIR) comb = Combination.TWO_PAIRS;
            else if (item.Value == 3 && comb == Combination.NONE) comb = Combination.THREE_OF_A_KIND;
            else if (item.Value == 4) comb = Combination.FOUR_OF_A_KIND;
            else if (item.Value == 5) comb = Combination.FIVE_OF_A_KIND;
            else if ((item.Value == 2 && comb == Combination.THREE_OF_A_KIND) ||
                (item.Value == 3 && comb == Combination.PAIR)) comb = Combination.FULL_HOUSE;
            _self.AddGoldValue += (int)Mathf.Pow(baseRewardLeague1, item.Value);
        }
        _self.CombinationName = comb.ToString().Replace("_", " ");
    }

    public void CheckYesNoDice(bool res)
    {
        if (res)
        {
            _self.CurrentActionName = EAction.DODGE;
            _self.CurrentActionValue = result[EAction.DODGE];
            _self.Dodge(result[EAction.DODGE]);
        }
        else
        {
            _self.CurrentActionName = EAction.NONE;
            _self.CurrentActionValue = 0;
        }
        _actions.OnActionButtonPressedEvent?.Invoke();
        StartCoroutine(_actions.DeactivateDodge());
    }

}
