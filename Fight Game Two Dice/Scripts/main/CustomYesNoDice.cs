using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomYesNoDice : MonoBehaviour
{
    [SerializeField] private Sprite[] _yesNoSprites; // [0] - yes; [1] - no

    private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[6];

    [HideInInspector]
    public bool[] IconValue = { false, false, false, false, false, false };

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        gameObject.SetActive(false);
        SetIconValue();
    }

    private void SetIconValue()
    {
        for (int i = 0; i < 6; i++)
        {
            switch (_spriteRenderers[i].sprite.name)
            {
                case "yesIcon":
                    IconValue[i] = true;
                    break;
                case "noIcon":
                    IconValue[i] = false;
                    break;
                default:
                    Debug.Log("!!! no value found !!!");
                    break;
            }
        }
    }

    public void SetEdgeYesNoSprites(int factor)
    {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < 6; i++)
        {
            if (factor > i)
                _spriteRenderers[i].sprite = _yesNoSprites[0];
            else
                _spriteRenderers[i].sprite = _yesNoSprites[1];
        }
        SetIconValue();
    }
}
