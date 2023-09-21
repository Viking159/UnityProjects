using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;

public class Player : Character
{
    private int _charisma;
    private int _maxHealth = 100;
    private int _maxEnergy = 100;
    private int _combReward;
    private int _killsReward;

    private Actions _actions;

    public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public int Charisma { get { return _charisma; } }

    public event Action OnPlayerDie;

    public int GladNum { get; private set; }
    public int AddGoldValue { get; set; }
    public string CombinationName { get; set; }

    public int TotalRewardCount => _combReward + _killsReward; //{ get; private set; }
    public int TotalMovesCount { get; private set; }

    private float _audienceMoodCoef;

    private void OnEnable()
    {
        IsGoodMove = -2;
        _audienceMoodCoef = 1;
        _combReward = 0;
        _killsReward = 0;
        TotalMovesCount = 0;
        if (ArenaFightSettings.FightType == -2)
        {
            GetComponent<ModulesShaker>().RandomizeAll();
            _maxHealth = 30;
            _maxEnergy = 50;
            Damage = 10;
            BlockedDamage = 5;
            _charisma = 20;
        }
        else
        {
            GladNum = PlayerPrefs.GetInt("ToFightGladNum");
            OwnGladiator glad = JsonUtility.FromJson<OwnedGladsStruct>(PlayerPrefs.GetString("OwnGlads")).glads[GladNum];
            GetComponent<ModulesShaker>().LoadElemets(glad.elements);
            int[] stats = glad.stats;
            _maxHealth = stats[0] * 5;
            _maxEnergy = stats[1] * 5;
            Damage = stats[2];
            BlockedDamage = stats[3];
            _charisma = stats[4];
        }
        Health = _maxHealth;
        Energy = _maxEnergy;
        MaxEnergy = _maxEnergy;
        HasShield = (GetComponent<ModulesShaker>().GetShieldNum() == 200) ? false : true;
        HasSpear = GetComponent<ModulesShaker>().IsSpeadOn();
        EnemyCharacter = FindObjectOfType<Enemy>();
        NormalPosition = (HasShield && EnemyCharacter.HasShield) ? new Vector3(-0.8835016f, 0f, -11.0043f) : NormalPosition = new Vector3(-0.57f, 0f, -11.0043f);

        Initialize();
        transform.position = NormalPosition;
        Anim = GetComponent<Animator>();
        _actions = FindObjectOfType<Actions>();
        _actions.OnActionButtonPressedEvent += AfterActionChoose;
        GetComponent<EnemySpawner>().OnEnemyInstantientAction += FindeNewEnemy;
        if (EnemyCharacter.GetType() == typeof(Enemy))
        {
            ((Enemy)EnemyCharacter).OnEnemyDieActionLate += OnEnemyDie;
        }
        VisualTxt.ShowAudienceMoodCoef(_audienceMoodCoef);
    }

    private void AfterActionChoose()
    {
        TotalMovesCount++;
        AddGold();
    }

    public void AddGold()
    {
        if (AddGoldValue == 0) return;
        int val = (int)(AddGoldValue * _audienceMoodCoef);
        val = (val == 0) ? 1 : val;
        _combReward += val;
        //TotalRewardCount += val;
        VisualTxt.ShowAddCombMoney(val, CombinationName);
        AddGoldValue = 0;
    }

    public override void Attack(int x)
    {
        CurrentActionName = Helper.EAction.ATTACK;
        CurrentActionValue = x;
        if (EnemyCharacter.CurrentActionName == Helper.EAction.ATTACK && CurrentActionValue < EnemyCharacter.CurrentActionValue)
        {
            //Debug.Log(_enemyAction[0].name + " x" + _enemyAction[0].value.ToString());
            StartCoroutine(OnEnemyAttackEnd());
            return;
        }
        base.Attack(CurrentActionValue);
    }

    private IEnumerator OnEnemyAttackEnd()
    {
        yield return new WaitForSeconds(0.8f);
        while (Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01" ||
            EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01")
        {
            yield return null;
        }
        if (IsDead == false)
            base.Attack(CurrentActionValue);
    }

    private readonly int _rewardCountStandart = 100;
    private void OnEnemyDie()
    {
        int val = 50;
        if (ArenaFightSettings.FightType == 0)
        {
            val = (int)(_rewardCountStandart * (1 + Charisma / 20f));
            _killsReward += val;
        }
        else if (ArenaFightSettings.FightType == 1)
        {
            val = (int)(_rewardCountStandart * (1 + Charisma / 20f)) * 5;
            _killsReward += val;
        }
        VisualTxt.ShowAddMoney(val);
        ChangeAudienceMoodCoef(2f);
    }

    private void FindeNewEnemy()
    {
        EnemyCharacter = FindObjectOfType<Enemy>();
        Energy = _maxEnergy;
        if (EnemyCharacter.GetType() == typeof(Enemy))
        {
            ((Enemy)EnemyCharacter).OnEnemyDieActionLate += OnEnemyDie;
        }
    }
    public void ResetHealth()
    {
        Health = _maxHealth;
        VisualTxt.UpdateHealth(Health, 0);
        VisualTxt.UpdateEnergy(Energy / (float)_maxEnergy, 0);
        VisualTxt.UpdateStats(0, Mathf.RoundToInt(Damage * (Energy / (float)_maxEnergy)), BlockedDamage);
    }

    public void UpdateStat(float[] stats)
    {
        Damage = (int)stats[0];
        CrititcalDamage = stats[1];
        MaxHealth = (int)stats[2];
        BlockedDamage = (int)stats[3];
        VisualTxt.UpdateStats(0, Damage, BlockedDamage);
    }

    public void ChangeAudienceMoodCoef(float value)
    {
        if (_audienceMoodCoef + value >= 3f)  _audienceMoodCoef = 3;
        else if (_audienceMoodCoef + value <= 0f) _audienceMoodCoef = 0;
        else _audienceMoodCoef += value;
        VisualTxt.ShowAudienceMoodCoef(_audienceMoodCoef);
    }

    protected override void Death()
    {
        _killsReward = 0;
        GetComponent<EnemySpawner>().OnEnemyInstantientAction -= FindeNewEnemy;
        OnPlayerDie?.Invoke();
        if (EnemyCharacter == null) return;
        if (EnemyCharacter.GetType() == typeof(Enemy))
        {
            ((Enemy)EnemyCharacter).OnEnemyDieActionLate -= OnEnemyDie;
        }
        _actions.OnActionButtonPressedEvent -= AfterActionChoose;
    }
}
