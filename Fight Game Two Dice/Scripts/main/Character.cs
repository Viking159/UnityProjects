using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Helper;

public class Character : MonoBehaviour
{
    [SerializeField]
    private GameObject _fightSoundPrefab;
    private InfoMessages _infoMessagesObj;
    private VisualText _visualText;

    private Animator _animator;


    private readonly int AttackAnimCount = 6;
    private readonly int CriticalAnimCount = 4;
    private readonly float _dodgeLength = 0.686f;
    private readonly float _attackLength = 0.8f;

    private bool _isDead;
    private int _hp;
    private int _normalBlockedDamage;
    private int _energy;
    private int _currentActionValue = 0;
    private float _criticalDamage;

    private EAction _currentActionName;
    
    public sbyte IsGoodMove { get; set; }
    public bool HasSpear { get; set; }
    public bool HasShield { get; set; }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public int BlockedDamage { get; set; }
    public int Damage { get; set; }
    public int Health { get { return _hp; } set { _hp = value; } }
    public int Energy { get { return _energy; } set { _energy = value; } }
    public int MaxEnergy { get; set; }
    public int CurrentActionValue { get { return _currentActionValue; } set { _currentActionValue = value; } }
    public EAction CurrentActionName { get { return _currentActionName; } set { _currentActionName = value; } }
    public float CrititcalDamage { get { return _criticalDamage; } set { _criticalDamage = value; _criticalDamage = (float)Math.Round(_criticalDamage, 2); } }

    public VisualText VisualTxt { get { return _visualText; } }
    public Character EnemyCharacter { get; set; }
    public Animator Anim { set { _animator = value; } get { return _animator; } }
    public Vector3 NormalPosition { get; set; }

    

    protected DiceRoller DiceRoller { get; set; }
    private ReRollButtonAnim _reRollButtonAnim;
    

    private int _x;
    private int _randomAnim;
    private bool _critAttack = false;

    protected void Initialize()
    {
        //gameObject.transform.position = NormalPosition;
        _normalBlockedDamage = BlockedDamage;
        DiceRoller = FindObjectOfType<DiceRoller>();
        _reRollButtonAnim = FindObjectOfType<ReRollButtonAnim>();
        _infoMessagesObj = FindObjectOfType<InfoMessages>();
        _visualText = FindObjectOfType<VisualText>();
        _visualText.UpdateHealth(_hp, (GetComponent<Player>() == null) ? 1 : 0);
        _visualText.UpdateEnergy(_energy / (float)MaxEnergy, (GetComponent<Player>() == null) ? 1 : 0);
        int damage = Mathf.RoundToInt(Damage * (_energy / (float)MaxEnergy));
        damage = (damage <= 0) ? 1 : damage;
        _visualText.UpdateStats((GetComponent<Player>() == null) ? 1 : 0, damage, BlockedDamage);
    }

    public void AttackEvent()
    {
        //Debug.Log(gameObject.name + " - Attack x" + _x.ToString() + "; Damage = " + Damage + ", damage = " + ((Damage * _energy) / 100f).ToString() + "; full damage = " + (((Damage * _energy) / 100f) * _x).ToString());
        int damage = Mathf.RoundToInt(Damage * (_energy / (float)MaxEnergy));
        damage = (damage <= 0) ? 1 : damage;
        EnemyCharacter.GetHit(damage * _x, _randomAnim, _critAttack);
        //EnemyCharacter.GetHit(Damage * _x, _randomAnim, _critAttack);
        AddEnergy(-10);
    }

    public virtual void Attack(int x)
    {
        _x = x;
        _currentActionName = EAction.ATTACK;
        _currentActionValue = x;
        int randomAnim;
        if (x >= 4)
        {
            if (HasSpear)
            {
                randomAnim = Random.Range(0, 2); // spear crit attack animCount = 2
                _animator.Play("CriticalAttack0" + randomAnim.ToString());
            }
            else
            {
                randomAnim = Random.Range(0, CriticalAnimCount);
                _animator.Play("CriticalAttack" + randomAnim.ToString());
            }
            _randomAnim = 10 + randomAnim;
            _critAttack = true;
            if (_infoMessagesObj.IsEnabled() == false)
                ShowInfoMessage("CRITICAL HIT");
        }
        else
        {
            _critAttack = false;
            if (HasSpear)
            {
                randomAnim = Random.Range(1, 5);
                _animator.Play("Attack" + randomAnim.ToString());
            }
            else
            {
                randomAnim = Random.Range(0, AttackAnimCount);
                _animator.Play("Attack" + randomAnim.ToString());
            }
            
            _randomAnim = randomAnim;
        }
        Instantiate(_fightSoundPrefab).GetComponent<FightSounds>().PlaySound((HasSpear) ? FightSounds.SoundType.Spear : FightSounds.SoundType.Sword);
        StartCoroutine(TempC(EAction.ATTACK));
        Invoke("SetCurrentActionNameValueNull", 2f);
    }

    

    public void Block(int x)
    {
        _currentActionName = EAction.BLOCK;
        _currentActionValue = x;
        _normalBlockedDamage = BlockedDamage;
        StartCoroutine(CheckParry(x));
        BlockedDamage = (_normalBlockedDamage == 0) ? 5 * x : _normalBlockedDamage * x;
        AddEnergy(5);
        _animator.Play("BlockStay");
        StartCoroutine(TempC(EAction.BLOCK));
        Invoke("SetNormalBlockedDamage", 0.5f);
    }

    private IEnumerator TempC(EAction actionName,  EAction enemyActionName = EAction.NONE, int actionValue = int.MinValue, int enemyActionValue = int.MaxValue)
    {
        bool isAdded = true;
        yield return null;
        switch (actionName)
        {
            case EAction.ATTACK:
                if ((EnemyCharacter.CurrentActionName == EAction.ATTACK && _currentActionValue > EnemyCharacter.CurrentActionValue) ||
                    (EnemyCharacter.CurrentActionName == EAction.BLOCK && _currentActionValue > EnemyCharacter.CurrentActionValue) ||
                    (EnemyCharacter.CurrentActionName == EAction.NONE && EnemyCharacter.CurrentActionValue == 0) ||
                    (EnemyCharacter.CurrentActionName == EAction.REST))
                {
                    if (EnemyCharacter.GetType() == typeof(Enemy))
                    {
                        DiceRoller.ChangeRollCount(1, true);
                    }
                    IsGoodMove = 1;
                    EnemyCharacter.IsGoodMove = -1;

                }
                else isAdded = false;
                break;
            case EAction.BLOCK:
                if (EnemyCharacter.CurrentActionName == EAction.ATTACK && _currentActionValue > EnemyCharacter.CurrentActionValue)
                {
                    if (EnemyCharacter.GetType() == typeof(Enemy))
                    {
                        DiceRoller.ChangeRollCount(1, true);
                    }
                    IsGoodMove = 1;
                    EnemyCharacter.IsGoodMove = -1;

                }
                else isAdded = false;
                break;
            case EAction.DODGE:
                if (EnemyCharacter.CurrentActionName == EAction.ATTACK)
                {
                    if (EnemyCharacter.GetType() == typeof(Enemy))
                    {
                        DiceRoller.ChangeRollCount(1, true);
                    }
                    IsGoodMove = 1;
                    EnemyCharacter.IsGoodMove = -1;
                }
                else isAdded = false;
                break;
            case EAction.REST:
                if ((enemyActionName == EAction.BLOCK) || ((enemyActionName == EAction.DODGE || enemyActionName == EAction.NONE) && actionValue > enemyActionValue))
                {
                    if (EnemyCharacter.GetType() == typeof(Enemy))
                    {
                        DiceRoller.ChangeRollCount(1, true);
                    }
                    IsGoodMove = 1;
                    EnemyCharacter.IsGoodMove = -1;
                }
                else isAdded = false;
                break;
        }
        if (isAdded && GetComponent<Player>() != null)
        {
            _reRollButtonAnim.AddReRollAnimStart();
        }
        if (isAdded == false)
        {
            IsGoodMove = 0;
        }
        if (IsGoodMove == 1 && EnemyCharacter.IsGoodMove == -1)
        {
            if (EnemyCharacter.GetType() == typeof(Player))
            {
                ((Player)EnemyCharacter).ChangeAudienceMoodCoef(-0.5f);
            }
            else
            {
                GetComponent<Player>().ChangeAudienceMoodCoef(0.5f);
            }
            IsGoodMove = -2;
            EnemyCharacter.IsGoodMove = -2;
        }
        else if (IsGoodMove == 0 && EnemyCharacter.IsGoodMove == 0)
        {
            if (EnemyCharacter.GetType() == typeof(Player))
            {
                ((Player)EnemyCharacter).ChangeAudienceMoodCoef(-0.25f);
            }
            else
            {
                GetComponent<Player>().ChangeAudienceMoodCoef(-0.25f);
            }
            IsGoodMove = -2;
            EnemyCharacter.IsGoodMove = -2;
        }

    }

    private IEnumerator CheckParry(int x)
    {
        yield return new WaitForSeconds(0.15f);
        if (EnemyCharacter.CurrentActionName == EAction.ATTACK && x > EnemyCharacter.CurrentActionValue)
        {
            yield return new WaitForSeconds(0.5f);
            ShowInfoMessage("PARRY");
            EnemyCharacter.AddEnergy(-5 * (x - EnemyCharacter.CurrentActionValue));
        }
    }

    public void GetHit(int damage, int attackAnimNum, bool crit = false)
    {
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "1H_dodge_to_left_humanoid" || _currentActionName == EAction.DODGE) return;
        Instantiate(_fightSoundPrefab).GetComponent<FightSounds>().PlaySound(FightSounds.SoundType.Hit);
        int takeDamageNum = GetRanTakeDamagehNum(attackAnimNum);
        int deathNum = GetRanDeathNum(takeDamageNum);
        _animator.SetInteger("DeathNum", deathNum);

        int takeDamage = damage;

        if (CurrentActionName == EAction.BLOCK) takeDamage = (damage - BlockedDamage) <= 0 ? 0 : (damage - BlockedDamage);
        //int realDamage = crit ? damage : takeDamage;
        _hp -= takeDamage;
        _hp = _hp < 0 ? 0 : _hp;
        _isDead = _hp == 0;
        _animator.SetBool("IsDead", _isDead);
        _visualText.UpdateHealth(_hp, (GetComponent<Player>() == null) ? 1 : 0);
        _visualText.ShowLostHealth(takeDamage, (GetComponent<Player>() == null) ? 1 : 0);

        if (_currentActionName != EAction.BLOCK) _animator.Play("TakeDamage" + takeDamageNum.ToString());
        if (_currentActionName == EAction.BLOCK) _animator.Play("Block");
        if (_isDead) Death();
    }

    public void DodgeEnergy()
    {
        IsGoodMove = 0;
        AddEnergy(-10);
    }

    public virtual void Dodge(int x)
    {
        IsGoodMove = -2;
        StartCoroutine(TempC(EAction.DODGE));
        _animator.applyRootMotion = true;
        _animator.Play("Dodge");
        Invoke("DisableAnimatorRootMotion", _dodgeLength);
        StartCoroutine(CheckCounterAttack(x));
    }

    private IEnumerator CheckCounterAttack(int x)
    {
        yield return new WaitForSeconds(0.15f);
        if (EnemyCharacter.CurrentActionName == EAction.ATTACK && x > EnemyCharacter.CurrentActionValue)
            StartCoroutine(CounterAttack(x - EnemyCharacter.CurrentActionValue));
    }

    public void Rest(int x)
    {
        _currentActionName = EAction.REST;
        _currentActionValue = x;
        StartCoroutine(CheckAnyDamage(x));
    }

    protected virtual void Death() { }

    private IEnumerator CheckAnyDamage(int x)
    {
        int currentHealth = Health;
        int val = 0;
        bool addAttack = false;

        EAction enemyActionName = EnemyCharacter.CurrentActionName;
        int actionValue = x;
        int enemyActionValue = EnemyCharacter.CurrentActionValue;

        if ((EnemyCharacter.CurrentActionName == EAction.DODGE || EnemyCharacter.CurrentActionName == EAction.BLOCK) && x > EnemyCharacter.CurrentActionValue)
        {
            addAttack = true;
            val = x - EnemyCharacter.CurrentActionValue;
        }

        yield return new WaitForSeconds(0.15f);
        while (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01" || EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01")
            yield return null;
        if (Health == currentHealth)
        {
            AddEnergy(5 * x);
            if (addAttack)
            {
                ShowInfoMessage("+" + val.ToString() + " Attack");
                if (GetComponent<DiceAnalysis>() != null)
                {
                    DiceAnalysis.AddAttackFactor = true;
                    DiceAnalysis.AddAttackFactorValue = val;
                }
                else FindObjectOfType<EnemyActions>().AddAttackScale(val);
                yield return new WaitForSeconds(0.4f);
            }
            StartCoroutine(TempC(EAction.REST, enemyActionName, actionValue, enemyActionValue));
            ShowInfoMessage("-" + (x * 10).ToString() + "% Defense");
            EnemyCharacter.DropDefense(x * 10);
        }
    }

    private void DropDefense(int percents)
    {
        int norm = BlockedDamage;
        BlockedDamage = Mathf.RoundToInt((BlockedDamage * (1 - percents / 100f)));
        int damage = Mathf.RoundToInt(Damage * (_energy / (float)MaxEnergy));
        damage = (damage <= 0) ? 1 : damage;
        _visualText.UpdateStats((GetComponent<Player>() == null) ? 1 : 0, damage, BlockedDamage);
        StartCoroutine(ResetDefense(norm));
    }

    private IEnumerator ResetDefense(int normalVal)
    {
        yield return null;
        while (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "1H@CombatIdle01" && EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "1H@CombatIdle01")
            yield return null;
        yield return new WaitForSeconds(0.2f);
        while (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01" || EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01")
            yield return null;
        BlockedDamage = normalVal;
        int damage = Mathf.RoundToInt(Damage * (_energy / (float)MaxEnergy));
        damage = (damage <= 0) ? 1 : damage;
        _visualText.UpdateStats((GetComponent<Player>() == null) ? 1 : 0, damage, BlockedDamage);
    }

    private IEnumerator CounterAttack(int attackValue)
    {
        yield return new WaitForSeconds(0.15f);
        while (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01" || EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01")
            yield return null;
        ShowInfoMessage("COUNTERATTACK");
        Attack(attackValue);
    }

    private void ShowInfoMessage(string text)
    {
        InfoMessages.Message = text;
        _infoMessagesObj.Enable();
    }

    private void SetNormalBlockedDamage()
    {
        BlockedDamage = _normalBlockedDamage;
        SetCurrentActionNameValueNull();
    }

    private void SetCurrentActionNameValueNull()
    {
        _currentActionName = EAction.NONE;
        _currentActionValue = 0;
    }
    private void DisableAnimatorRootMotion()
    {
        _animator.applyRootMotion = false;
        transform.position = NormalPosition;
    }

    protected void AddEnergy(int val)
    {
        _energy += val;
        _energy = (_energy < 0) ? 0 : _energy;
        _energy = (_energy > MaxEnergy) ? MaxEnergy : _energy;
        if (_visualText == null) print("NULL");
        _visualText.UpdateEnergy(_energy /(float)MaxEnergy, (GetComponent<Player>() == null) ? 1 : 0);
        _visualText.ShowLostEnergy(val, (GetComponent<Player>() == null) ? 1 : 0);
        int damage = Mathf.RoundToInt(Damage * (_energy / (float)MaxEnergy));
        damage = (damage <= 0) ? 1 : damage;
        _visualText.UpdateStats((GetComponent<Player>() == null) ? 1 : 0, damage, _normalBlockedDamage);
    }

    private int GetRanDeathNum(int takeDamageNum)
    {
        switch (takeDamageNum)
        {
            case 0:
                return 0;
            case 1:
            case 3:
                return Random.Range(1, 3);
            case 2:
                return Random.Range(0, 2);
            case 4:
                return 3;

        }
        throw new Exception("Rand Death Num Error. Character.cs -> GetRanDeathNum()");
    }

    private int GetRanTakeDamagehNum(int attackNum)
    {
        switch (attackNum)
        {
            case 0:
                int num = Random.Range(0, 3);
                if (num == 1) return 2;
                if (num == 2) return 4;
                return num;
            case 1:
                return Random.Range(1, 4);
            case 2:
                return Random.Range(1, 3);
            case 3:
            case 4:
            case 5:
            case 12:
                return 4;
            case 10:
            case 11:
            case 13:
                int num1 = Random.Range(1, 3);
                if (num1 == 2) return 3;
                return num1;
        }
        throw new Exception("Rand TakeDamage Num Error. Character.cs -> GetRanTakeDamagehNum()");
    }

}
