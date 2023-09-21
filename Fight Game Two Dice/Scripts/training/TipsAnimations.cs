using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TipsAnimations : MonoBehaviour
{
    [SerializeField]
    private AnimationType _animType;
    private enum AnimationType
    {
        Blinking,
        SideToSide,
        ShowUp,
        UpDownLocal
    }

    private Vector3 _startPos;

    private void OnEnable()
    {
        StopCoroutine(Blinking());
        StopCoroutine(SideToSide());
        switch (_animType)
        {
            case AnimationType.Blinking:
                StartCoroutine(Blinking());
                break;
            case AnimationType.SideToSide:
                _startPos = transform.localPosition;
                StartCoroutine(SideToSide());
                break;
            case AnimationType.UpDownLocal:
                _startPos = transform.localPosition;
                StartCoroutine(UpDownLocal());
                break;
            case AnimationType.ShowUp:
                StartCoroutine(ShowUp());
                break;
        }
    }

    private IEnumerator ShowUp()
    {
        transform.localScale = Vector3.zero;
        SetChildActive(false);
        transform.DOScale(Vector3.one, 1f);
        yield return new WaitForSeconds(1f);
        SetChildActive(true);
    }

    private IEnumerator Blinking()
    {
        Image img = GetComponent<Image>();
        Color color1 = img.color;
        Color color2 = new Color(color1.r, color1.g, color1.b, 0.4f);
        while (gameObject.activeInHierarchy)
        {
            img.DOColor(color2, 0.5f);
            yield return new WaitForSeconds(0.5f);
            img.DOColor(color1, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SideToSide()
    {
        float deltaPos = 50f;
        float delay = 0.5f;
        while (gameObject.activeInHierarchy)
        {
            transform.DOLocalMoveX(_startPos.x + deltaPos, 1f);
            yield return new WaitForSeconds(delay);
            transform.DOLocalMoveX(_startPos.x - deltaPos, 1f);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator UpDownLocal()
    {
        float deltaPos = 50f;
        float delay = 0.5f;
        while (gameObject.activeInHierarchy)
        {
            transform.DOLocalMoveY(_startPos.y + deltaPos, 1f);
            yield return new WaitForSeconds(delay);
            transform.DOLocalMoveY(_startPos.y - deltaPos, 1f);
            yield return new WaitForSeconds(delay);
        }
    }


    private void SetChildActive(bool active, int ignor = 0)
    {
        for (int i = 0; i < transform.childCount; i++)
            if (i != ignor)
                transform.GetChild(i).gameObject.SetActive(active);
    }

    private void OnDisable()
    {
        switch (_animType)
        {
            case AnimationType.Blinking:
                StopCoroutine(Blinking());
                GetComponent<Image>().color = Color.clear;
                break;
            case AnimationType.SideToSide:
                StopCoroutine(SideToSide());
                transform.localPosition = _startPos;
                break;
            case AnimationType.UpDownLocal:
                StopCoroutine(UpDownLocal());
                transform.localPosition = _startPos;
                break;
            case AnimationType.ShowUp:
                StopCoroutine(ShowUp());
                transform.localScale = Vector3.one;
                SetChildActive(true, -1);
                break;
        }
    }
}
