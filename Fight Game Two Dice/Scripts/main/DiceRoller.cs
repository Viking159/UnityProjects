using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DiceRoller : MonoBehaviour
{
    [SerializeField]
    private Die_d6[] _dice;
    [SerializeField]
    private Die_d6 _yesNoDice;


    private DiceAnalysis _diceAnalysis;
    private Character[] _characters;
    private ActionButtons _actionButtons;
    private VisualText _visualText;

    private bool _addedReRoll;
    private bool _train;
    private bool _rotating;
    private bool _turning;

    private int _rollCount;
    private int _actionRollNum;

    private Helper.EAction[] _diceValues;

    public bool IsCheating { get; set; }

    private readonly Helper.EAction[][] ACTIONS = new Helper.EAction[8][]
    {
        new Helper.EAction[5] { Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.BLOCK, Helper.EAction.ATTACK, Helper.EAction.BLOCK },
        new Helper.EAction[5] { Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.BLOCK },
        new Helper.EAction[5] { Helper.EAction.ATTACK, Helper.EAction.BLOCK, Helper.EAction.ATTACK, Helper.EAction.BLOCK, Helper.EAction.DODGE },
        new Helper.EAction[5] { Helper.EAction.BLOCK, Helper.EAction.BLOCK, Helper.EAction.BLOCK, Helper.EAction.BLOCK, Helper.EAction.BLOCK },
        new Helper.EAction[5] { Helper.EAction.DODGE, Helper.EAction.DODGE, Helper.EAction.DODGE, Helper.EAction.DODGE, Helper.EAction.DODGE },
        new Helper.EAction[5] { Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.BLOCK },
        new Helper.EAction[5] { Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.ATTACK },
        new Helper.EAction[5] { Helper.EAction.REST, Helper.EAction.REST, Helper.EAction.ATTACK, Helper.EAction.ATTACK, Helper.EAction.BLOCK }
    };

    public int RollCount { get { return _rollCount; } }

    public bool ReRoll { private get; set; }

    private void Awake()
    {
        IsCheating = false;
        _actionRollNum = 0;
        _addedReRoll = false;
        ReRoll = false;
        _train = false;
        _diceValues = new Helper.EAction[_dice.Length];
        _diceAnalysis = FindObjectOfType<DiceAnalysis>();
        _characters = FindObjectsOfType<Character>();
        _actionButtons = FindObjectOfType<ActionButtons>();
        _visualText = FindObjectOfType<VisualText>();
        ResetRerollCount();
        if (ArenaFightSettings.FightType == -2)
            _train = true;
    }

    public void ResetRerollCount()
    {
        //if (_rollCount >= 2) return;

        _rollCount = (_addedReRoll) ? 3 : 2;
        _visualText.UpdateReRollText(_rollCount);
        _addedReRoll = false;
    }

    public void ChangeRollCount(int val, bool spec = false)
    {
        if (_rollCount >= 3 && spec) { _rollCount = 3; return; }
        if (spec) _addedReRoll = true;
        _rollCount += val;
        _visualText.UpdateReRollText(_rollCount);
    }

    public void Roll()
    {
        if (_turning || _rotating || CheckDeaths() || CheckDeactivatedButtons() || CheckActiveAnimations()
            || EnemyActions.SecondAttack || _dice[0].gameObject.activeInHierarchy == false
            || _rollCount == 0 || Actions.YesNoDiceActive)
            return;
        _addedReRoll = false;
        ChangeRollCount(-1);

        _actionButtons.DeactivateAll();


        if (_train)
        {
            SpecialRotation(ACTIONS[_actionRollNum++]);
            if (_actionRollNum == 8)
                _train = false;
        } 
        else if (IsCheating)
        {
            SpecialRotation(FindObjectOfType<Cheats>().GetDiceValues());
            IsCheating = false;
        }
        else
        {
            RandomRotation();
        }

        //if (_train == false) RandomRotation();
        //else if (IsCheating)
        //{
        //    SpecialRotation(FindObjectOfType<Cheats>().GetDiceValues());
        //    IsCheating = false;
        //}
        //else
        //{
        //    SpecialRotation(ACTIONS[_actionRollNum++]);
        //    if (_actionRollNum == 8)
        //        _train = false;
        //}

        _rotating = true;
        ReRoll = true;
    }

    private void RandomRotation()
    {
        foreach (Die_d6 die in _dice)
        {
            DieD6Interactable dieInteractable = die.GetComponent<DieD6Interactable>();

            if (dieInteractable.GetChoose())
                continue;
            dieInteractable.SetButtonActivation(false);
            float[] randomRotationValues = new float[3];
            for (int i = 0; i < randomRotationValues.Length; i++)
                randomRotationValues[i] = 90f * Random.Range(1, 5) * (Random.value > 0.5f ? -1 : 1);

            Vector3 rotationVector = new Vector3(randomRotationValues[0], randomRotationValues[1], randomRotationValues[2]);

            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(die.transform.DORotate(rotationVector, 1f, RotateMode.LocalAxisAdd));
            rotationSequence.AppendCallback(RotationEnded);
        }
    }

    private void SpecialRotation(Helper.EAction[] values)
    {
        byte i = 0;
        Debug.Log("Special!");
        for (int k = 0; k < values.Length; k++)
        {
            Debug.Log(values[k]);
        }
        foreach (Die_d6 die in _dice)
        {
            DieD6Interactable dieInteractable = die.GetComponent<DieD6Interactable>();

            if (dieInteractable.GetChoose())
            {
                i++;
                continue;
            }
            dieInteractable.SetButtonActivation(false);

            Vector3 rotationVector = GetVector3(values[i], die.transform.rotation.eulerAngles, i);
            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(die.transform.DORotate(die.transform.rotation.eulerAngles + rotationVector, 1f, RotateMode.FastBeyond360));
            rotationSequence.AppendCallback(RotationEnded);
            i++;
        }
    }

    private Vector3 GetVector3(Helper.EAction val, Vector3 dieRot, byte diceNum)
    {
        float x, y, z;
        float tempVal;
        float xP, yP, zP;
        List<byte> edges = _dice[diceNum].GetComponent<CustomDice>().GetEdges(val);
        int edgeNum = edges[Random.Range(0, edges.Count)];
        switch (edgeNum)
        {
            case 0:
                xP = 270;
                yP = 0;
                zP = 0;
                break;
            case 1:
                xP = 0;
                yP = 270;
                zP = 180;
                break;
            case 2:
                xP = 0;
                yP = 90;
                zP = 270;
                break;
            case 3:
                xP = 0;
                yP = 90;
                zP = 90;
                break;
            case 4:
                xP = 0;
                yP = 0;
                zP = 0;
                break;
            case 5:
                xP = 90;
                yP = 0;
                zP = 0;
                break;
            default:
                Debug.LogError("Dice Roller -> GetVector3 -> edgeNum error!");
                return Vector3.zero;
        }
        tempVal = xP - dieRot.x;
        x = (tempVal < 0) ? tempVal + 360 : ((tempVal <= 10 && tempVal >= 0) ? 360 : tempVal);
        tempVal = yP - dieRot.y;
        y = (tempVal < 0) ? tempVal + 360 : ((tempVal <= 10 && tempVal >= 0) ? 360 : tempVal);
        tempVal = zP - dieRot.z;
        z = (tempVal < 0) ? tempVal + 360 : ((tempVal <= 10 && tempVal >= 0) ? 360 : tempVal);
        return new Vector3(x, y, z);
    }

    public void YesNoRoll()
    {
        Actions.YesNoDiceActive = false;
        if (_train || Debug.isDebugBuild)
        {
            RollOnlyYes();
            return;
        }
        float[] randomRotationValues = new float[3];
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = 90f * Random.Range(1, 5) * (Random.value > 0.5f ? -1 : 1);
        }

        Vector3 rotationVector = new Vector3(randomRotationValues[0], randomRotationValues[1], randomRotationValues[2]);
        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence.Append(_yesNoDice.transform.DORotate(rotationVector, 1f, RotateMode.LocalAxisAdd));
        rotationSequence.AppendCallback(YesNoRotationEnded);
        _rotating = true;
    }

    private void RollOnlyYes()
    {
        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence.Append(_yesNoDice.transform.DORotate(_yesNoDice.transform.rotation.eulerAngles + new Vector3(270, 360, 360), 1f, RotateMode.FastBeyond360));
        rotationSequence.AppendCallback(YesNoRotationEnded);
        _rotating = true;
    }

    private void YesNoRotationEnded()
    {
        if (!_rotating)
            return;
        _rotating = false;
        _diceAnalysis.CheckYesNoDice(_yesNoDice.GetComponent<CustomYesNoDice>().IconValue[_yesNoDice.value - 1]);
    }

    private void RotationEnded()
    {

        if (!_rotating)  //prevent method from execution on each die
            return;
        _rotating = false;
        
        StartCoroutine(TurnDices());
        for (int i = 0; i < _dice.Length; i++)
        {
            _diceValues[i] = _dice[i].GetComponent<CustomDice>().IconValue[_dice[i].value - 1];
        }
        _diceAnalysis.CheckDice(_diceValues);

    }

    public Helper.EAction GetDiceValue(int i) => _diceValues[i];

    public bool IsRotating() => _rotating;

    private readonly float _turnSpeed = 350;

    private readonly Quaternion[] normals = new Quaternion[12] {
        new Quaternion(0.7071068f, 0f, 0f, -0.7071068f),
        new Quaternion(0f, -0.01490116f, -1f, 0f),
        new Quaternion(-0.01053671f, -0.01053671f, -0.7071068f, 0.7071068f),
        new Quaternion(-0.5f, -0.5f, 0.5f, 0.5f),
        new Quaternion(-0.01490116f, 0f, 0f, 1f),
        new Quaternion(0f, -0.7071068f, 0.7071068f, 0f),

        new Quaternion(-0.7071068f, 0f, 0f, 0.7071068f),
        new Quaternion(0f, 0.01490116f, 1f, 0f),
        new Quaternion(0.01053671f, 0.01053671f, 0.7071068f, -0.7071068f),
        new Quaternion(0.5f, 0.5f, -0.5f, -0.5f),
        new Quaternion(0.01490116f, 0f, 0f, -1f),
        new Quaternion(0f, 0.7071068f, -0.7071068f, 0f),
    };

    private IEnumerator TurnDices()
    {
        _turning = true;
        int direction;
        float angle;
        Vector3[] step = new Vector3[_dice.Length];
        bool[] turned = new bool[_dice.Length];
        for (int i = 0; i < _dice.Length; i++)
        {
            Quaternion dice = RoundQuaternion(_dice[i].transform.rotation.normalized, 5);
            Quaternion normal = RoundQuaternion(normals[_dice[i].value - 1], 5);
            Quaternion rNormal = RoundQuaternion(normals[_dice[i].value + 5], 5);
            if ((dice == normal) || (dice == rNormal))
            {
                turned[i] = true;
                continue;
            }
            direction = 1;
            angle = Quaternion.Angle(dice, rNormal);

            Quaternion temp = Quaternion.Euler(_dice[i].transform.rotation.normalized.eulerAngles + new Vector3(0, angle, 0));
            if (RoundQuaternion(temp, 1) != RoundQuaternion(normals[_dice[i].value + 5], 1))
                direction = -1;
            step[i] = new Vector3(0, angle / (_turnSpeed * Time.fixedDeltaTime) * direction, 0);
        }

        int k;
        while (true)
        {
            k = 0;
            for (int i = 0; i < _dice.Length; i++)
            {
                if (turned[i] == true || (_dice[i].value == 1 || _dice[i].value == 5))
                {
                    _dice[i].GetComponent<DieD6Interactable>().SetButtonActivation(true);
                    k++;
                    continue;
                }
                else
                {
                    k = 0;
                }

                if (RoundQuaternion(_dice[i].transform.rotation.normalized, 5) == RoundQuaternion(normals[_dice[i].value - 1], 5) ||
                    Quaternion.Angle(_dice[i].transform.rotation.normalized, normals[_dice[i].value - 1]) <= 10)
                {
                    _dice[i].transform.rotation = RoundQuaternion(normals[_dice[i].value - 1], 1);
                    turned[i] = true;
                    continue;
                }
                if (RoundQuaternion(_dice[i].transform.rotation.normalized, 5) == RoundQuaternion(normals[_dice[i].value + 5], 5) ||
                    Quaternion.Angle(_dice[i].transform.rotation.normalized, normals[_dice[i].value + 5]) <= 10)
                {
                    _dice[i].transform.rotation = RoundQuaternion(normals[_dice[i].value + 5], 1);
                    turned[i] = true;
                    continue;
                }
                _dice[i].transform.rotation = Quaternion.Euler(_dice[i].transform.rotation.eulerAngles + step[i]);
            }
            if (k == _dice.Length)
                break;

            yield return new WaitForFixedUpdate();
        }
        _turning = false;
    }

    private Quaternion RoundQuaternion(Quaternion a, int digs) => new Quaternion((float)System.Math.Round(a.x, digs), (float)System.Math.Round(a.y, digs), (float)System.Math.Round(a.z, digs), (float)System.Math.Round(a.w, digs));

    private bool CheckDeaths()
    {
        _characters = FindObjectsOfType<Character>();
        foreach (Character character in _characters)
        {
            if (character.IsDead) return true;
        }
        return false;
    }

    private bool CheckAnyDeactivatedButton()
    {
        foreach (Die_d6 die in _dice)
            if (die.GetComponent<DieD6Interactable>().GetChoose()) return true;
        return false;
    }

    private bool CheckDeactivatedButtons()
    {
        int count = 0;
        foreach (Die_d6 die in _dice)
        {
            if (die.GetComponent<DieD6Interactable>().GetChoose()) count++;
        }
        if (count == _dice.Length) return true;
        return false;
    }

    private bool CheckActiveAnimations()
    {
        _characters = FindObjectsOfType<Character>();
        foreach (Character character in _characters)
            if (character.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "1H@CombatIdle01") return true;
        return false;
    }



}
