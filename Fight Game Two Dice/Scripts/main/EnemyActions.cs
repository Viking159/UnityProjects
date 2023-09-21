using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Helper;

public class EnemyActions : MonoBehaviour
{
    [SerializeField] private Sprite[] _enemyActionSprites; // Attack/Rest; Block/Dodge 
    [SerializeField] private Sprite[] _actionSprites; // Attack, Block, Dodge, Rest
    [SerializeField] private Image[] _actionImages;
    [SerializeField] private Text[] _actionText;
    [SerializeField] private Image _sliderBackgorund;
    [SerializeField] private GameObject _fightSoundPrefab;

    private Enemy _self;
    private Player _player;
    private Actions _actions;
    private EnemySpawner _enemySpawner;
    private List<EnemyAction> _enemyAction = new List<EnemyAction>();

    private readonly Vector3 normalPos = new Vector3(0, 0, 0);
    private readonly Vector3 specPos = new Vector3(-31.31f, 0, 0);
    private readonly EnemyAction[] trainEnemyActions = new EnemyAction[10]
    {
        //first enemy
        new EnemyAction(EAction.REST, 2, 0),
        new EnemyAction(EAction.BLOCK, 1, -5),
        new EnemyAction(EAction.BLOCK, 3, -5),
        //seconde enemy
        new EnemyAction(EAction.ATTACK, 2, 10),
        new EnemyAction(EAction.ATTACK, 1, 10),
        new EnemyAction(EAction.BLOCK, 3, -5),
        new EnemyAction(EAction.ATTACK, 1, 10),
        //third enemy
        new EnemyAction(EAction.BLOCK, 3, -5),
        new EnemyAction(EAction.BLOCK, 4, -5),
        new EnemyAction(EAction.BLOCK, 3, -5)
    };

    private bool _isTrainActionsEnd;
    private int _countTrainActions;

    public static bool SecondAttack = false;

    private struct EnemyAction
    {
        public EAction name;
        public int value;
        public int energyCost;

        public EnemyAction(EAction name, int value, int energyCost)
        {
            this.name = name;
            this.value = value;
            this.energyCost = energyCost;
        }

        public void PrintSelf()
        {
            Debug.Log(name + " => " + value.ToString() + " => " + energyCost.ToString());
        }

    }

    private void Start()
    {
        _isTrainActionsEnd = false;
        _countTrainActions = 0;
        foreach (var text in _actionText)
        {
            text.enabled = false;
        }
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _player = FindObjectOfType<Player>();
        _actions = FindObjectOfType<Actions>();
        _actions.OnActionButtonPressedEvent += OnActionButtonPressed;
        FindNewSelf();
        _enemySpawner.OnEnemyInstantientAction += FindNewSelf;
    }

    public void AddAttackScale(int val)
    {
        int prValue = _enemyAction[1].value;
        if (_enemyAction[1].name == EAction.ATTACK)
        {
            _enemyAction[1] = new EnemyAction(EAction.ATTACK, prValue + val, 10);
        }
        SetVisaul();
    }

    private void FindNewSelf()
    {
        _self = FindObjectOfType<Enemy>();
        _enemyAction = new List<EnemyAction>();
        for (int i = 0; i < 3; i++)
        {
            _enemyAction.Add(SetRandomAction(_self.Energy));
        }
        SecondAttack = false;
        SetVisaul();
    }
    
    private EnemyAction SetRandomAction(int energy)
    {
        if (ArenaFightSettings.FightType == -2 && _countTrainActions < 10)
            return trainEnemyActions[_countTrainActions++];
        _isTrainActionsEnd = true;
        int r;
        EnemyAction action = new EnemyAction(EAction.NONE, 0, 0);
        foreach (EnemyAction item in _enemyAction)
        {
            energy -= item.energyCost;
        }
        if (energy >= 10) r = Random.Range(1, 7);
        else r = Random.Range(4, 7);
        switch (r)
        {
            case 1:
            case 2:
                action.name = EAction.ATTACK;
                action.energyCost = 10;
                break;
            case 3:
                action.name = EAction.DODGE;
                action.energyCost = 10;
                break;
            case 4:
            case 5:
                action.name = EAction.BLOCK;
                action.energyCost = -5;
                break;
            case 6:
                action.name = EAction.REST;
                break;

        }

        r = Random.Range(0, 21);

        if (r <= 10) action.value = 1;
        else if (r <= 14) action.value = 2;
        else if (r <= 17) action.value = 3;
        else if (r <= 19) action.value = 4;
        else action.value = 5;
        return action;
    }

    private void SetVisaul()
    {
        int i = 0;
        foreach (var action in _enemyAction)
        {
            switch (action.name)
            {
                case EAction.REST:
                case EAction.ATTACK:
                    _actionImages[i].sprite = _enemyActionSprites[0];
                    _actionImages[i].sprite = _enemyActionSprites[0];
                    break;
                case EAction.DODGE:
                case EAction.BLOCK:
                    _actionImages[i].sprite = _enemyActionSprites[1];
                    _actionImages[i].sprite = _enemyActionSprites[1];
                    break;
            }
            i++;
        }
        _actionText[1].text = _enemyAction[0].value.ToString();
        HideRealAction();
    }

    private void OnActionButtonPressed()
    {
        if (_countTrainActions >= 10 && _player.CurrentActionName == EAction.ATTACK && _isTrainActionsEnd)
        {
            _enemyAction[0] = new EnemyAction(EAction.DODGE, 5, 10);
        }
        ShowRealAction();
        _self.CurrentActionName = _enemyAction[0].name;
        _self.CurrentActionValue = _enemyAction[0].value;
        if (_player.CurrentActionName == EAction.ATTACK && _enemyAction[0].name == EAction.ATTACK && _player.CurrentActionValue >= _self.CurrentActionValue)
        {
            SecondAttack = true;
            StartCoroutine(OnPlayerAttackEnd());
            return;
        }
        
        if (_self.Energy >= _enemyAction[0].energyCost)
        {
            switch (_enemyAction[0].name)
            {
                case EAction.ATTACK:
                    _self.Attack(_enemyAction[0].value);
                    break;
                case EAction.BLOCK:
                    _self.Block(_enemyAction[0].value);
                    break;
                case EAction.DODGE:
                    int val = _enemyAction[0].value;
                    if (_countTrainActions >= 10 && _player.CurrentActionName == EAction.ATTACK && _isTrainActionsEnd)
                        val = 6;
                    _self.Dodge(val);
                    break;
                case EAction.REST:
                    _self.Rest(_enemyAction[0].value);
                    break;
            }
        }
        else
        {
            StartCoroutine(AnimationNoEnergy());
        }

        if (_self.CurrentActionName == EAction.ATTACK && _self.EnemyCharacter.CurrentActionName == EAction.BLOCK
            || _self.CurrentActionName == EAction.BLOCK && _self.EnemyCharacter.CurrentActionName == EAction.ATTACK)
            Instantiate(_fightSoundPrefab).GetComponent<FightSounds>().PlaySound(FightSounds.SoundType.ShieldBlock);


        Invoke("UpdateActions", 1.5f);
    }

    private IEnumerator AnimationNoEnergy()
    {
        Color color = new Color(1, 1, 1);
        for (int i = 0; i < 3; i++)
        {
            while (color.g > 0 || color.b > 0)
            {
                color.g -= 0.1f;
                color.b -= 0.1f;
                if (color.g < 0) color.g = 0;
                if (color.b < 0) color.b = 0;
                _sliderBackgorund.color = color;
                yield return new WaitForFixedUpdate();
            }
            while (color.g < 1 || color.b < 1)
            {
                color.g += 0.1f;
                color.b += 0.1f;
                if (color.g > 1) color.g = 1;
                if (color.b > 1) color.b = 1;
                _sliderBackgorund.color = color;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void ShowRealAction()
    {
        switch (_enemyAction[0].name)
        {
            case EAction.ATTACK:
                _actionImages[0].sprite = _actionSprites[0];
                break;
            case EAction.BLOCK:
                _actionImages[0].sprite = _actionSprites[1];
                break;
            case EAction.DODGE:
                _actionImages[0].sprite = _actionSprites[2];
                break;
            case EAction.REST:
                _actionImages[0].sprite = _actionSprites[3];
                break;
        }
        _actionImages[0].gameObject.transform.localPosition = specPos;
        foreach (var text in _actionText)
        {
            text.enabled = true;
        }
    }

    private void HideRealAction()
    {
        _actionImages[0].gameObject.transform.localPosition = normalPos;
        foreach (var text in _actionText)
        {
            text.enabled = false;
        }
    }

    private void UpdateActions()
    {
        HideRealAction();
        if (_self.IsDead) return;
        _enemyAction.RemoveAt(0);
        _enemyAction.Add(SetRandomAction(_self.Energy));
        SetVisaul();
    }


    private IEnumerator ShowAnimName()
    {
        while (true)
        {
            print(_self.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            yield return null;
        }
    }

    private IEnumerator OnPlayerAttackEnd()
    {
        yield return new WaitForSeconds(0.2f);
        while (_self.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01" ||
            _self.EnemyCharacter.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01")
        {
            yield return null;
        }
        if (_self.IsDead == false && SecondAttack == true)
            _self.Attack(_enemyAction[0].value);
        SecondAttack = false;
        Invoke("UpdateActions", 1f);
    }

    private void OnDestroy()
    {
        if (_actions != null)
            _actions.OnActionButtonPressedEvent -= OnActionButtonPressed;
        if (_enemySpawner != null)
            _enemySpawner.OnEnemyInstantientAction -= FindNewSelf;
    }

}
