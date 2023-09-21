using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class CustomDice : MonoBehaviour
{

    private SpriteRenderer[] _spriteRenderers;

    [HideInInspector]
    public EAction[] IconValue = { EAction.NONE, EAction.NONE, EAction.NONE, EAction.NONE, EAction.NONE, EAction.NONE };

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        SetIconValue();
    }

    private void SetIconValue()
    {
        for (int i = 0; i < 6; i++)
        {
            switch (_spriteRenderers[i].sprite.name)
            {
                case "attackIcon":
                    IconValue[i] = EAction.ATTACK;
                    break;
                case "blockIcon":
                    IconValue[i] = EAction.BLOCK;
                    break;
                case "dodgeIcon":
                    IconValue[i] = EAction.DODGE;
                    break;
                case "restIcon":
                    IconValue[i] = EAction.REST;
                    break;
                default:
                    IconValue[i] = EAction.NONE;
                    Debug.LogError("!!! no value found !!!");
                    break;
            }
        }
    }

    public List<byte> GetEdges(EAction action)
    {
        List<byte> res = new List<byte>();
        for (byte i = 0; i < 6; i++)
            if (IconValue[i] == action)
                res.Add(i);
        return res;
    }

    /* 1 - блок
     * 2 - атака
     * 3 - отдых
     * 4 - уворот
     * 5 - блок
     * 6 - атака
     * */
}
