using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class TestDiceRotate : MonoBehaviour
{
    [SerializeField] private Die_d6[] _dice;

    public Helper.EAction[] Vals;

    private bool _rotating;

    public void Roll()
    {
        if (_rotating)
            return;
        byte i = 0;
        
        foreach (Die_d6 die in _dice)
        {
            Vector3 rotationVector = GetVector3(Vals[i++], die.transform.rotation.eulerAngles, i);
            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(die.transform.DORotate(die.transform.rotation.eulerAngles + rotationVector, 0.3f, RotateMode.FastBeyond360));
            rotationSequence.AppendCallback(RotationEnded);
        }

        _rotating = true;
    }

    private Vector3 GetVector3(Helper.EAction val, Vector3 dieRot, byte diceNum)
    {
        float x, y, z;
        float tempVal;
        float xP, yP, zP;
        List<byte> edges = _dice[diceNum].GetComponent<CustomDice>().GetEdges(val);
        int edgeNum = Random.Range(0, edges.Count);
        switch (edgeNum)
        {
            case 1:
                xP = 270;
                yP = 0;
                zP = 0;
                break;
            case 2:
                xP = 0;
                yP = 270;
                zP = 180;
                break;
            case 3:
                xP = 0;
                yP = 90;
                zP = 270;
                break;
            case 4:
                xP = 0;
                yP = 90;
                zP = 90;
                break;
            case 5:
                xP = 0;
                yP = 0;
                zP = 0;
                break;
            case 6:
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

    /*
    Val1: (270, 0, 0)
    Val2: (0, 270, 180)
    Val3: (0, 90, 270)
    Val4: (0, 90, 90)
    Val5: (0, 0, 0)
    Val6: (90, 0, 0)
    */

    private void RotationEnded()
    {
        if (!_rotating)  //prevent method from execution on each die
            return;
        _rotating = false;
    }
}
