using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScreen : MonoBehaviour
{
    private readonly Color _borderColorGreen = new Color(0.0796f, 0.528f, 0f);
    private readonly Color _borderColorRed = new Color(0.5294f, 0.0545f, 0f);

    private Knife _knife;
    private MoneyCount _moneyCount;
    private Text[] _texts = new Text[2];
    private Image _finalBorder;
    private GameObject _finalScreen;

    private void Start()
    {
        _finalScreen = GameObject.FindGameObjectWithTag("finalScreen");
        _texts = _finalScreen.GetComponentsInChildren<Text>();
        _finalBorder = _finalScreen.GetComponentsInChildren<Image>()[1];
        GetKnife();
        _moneyCount = FindObjectOfType<MoneyCount>();

        _finalScreen.SetActive(false);
    }

    private void GetKnife()
    {
        _knife = FindObjectOfType<Knife>();
        _knife.OnGameEndAction += OnGameEnd;
        _knife.OnKnifeCreating += GetKnife;
    }

    private void OnGameEnd(bool success)
    {
        _finalScreen.SetActive(true);
        if (success) Victory();
        else Lose();
        _knife.OnGameEndAction -= OnGameEnd;
    }

    private void Victory()
    {
        _texts[0].text = "Пройдено";
        _finalBorder.color = _borderColorGreen;
        StartCoroutine(MoneyAddAnimation());
    }
    private void Lose()
    {
       _texts[0].text = "Провалено";
        _finalBorder.color = _borderColorRed;
        _texts[1].text = MoneyCount.GetMoney(false).ToString();
    }

    private IEnumerator MoneyAddAnimation()
    {
        int allMoney = MoneyCount.GetMoney(false);
        int eranedMoney = MoneyCount.GetMoney(true);
        _moneyCount.moneyCount = eranedMoney + allMoney;
        while (eranedMoney > 0)
        {
            _moneyCount.AddMoney(-1); eranedMoney--; allMoney++;
            _texts[1].text = allMoney.ToString();
            yield return null;
        }
    }
}
