using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualText : MonoBehaviour
{
    [Header("Audience mood coef")]
    [SerializeField] private Image _AMIcon;
    [SerializeField] private Text _AMLabel;
    [SerializeField] private Text _AMValueText;
    [Space]

    [SerializeField] private Text[] _charactersStats;
    [SerializeField] private Text[] _healthText;
    [SerializeField] private Slider[] _energyVal;
    [SerializeField] private Text _actionInfoText;
    [SerializeField] private Text _rerollCountText;
    [SerializeField] private Transform _playerLostTextTransform;
    [SerializeField] private Transform _enemyLostTextTransform;
    [Space]
    [Header("Pop up prefabs")]
    [SerializeField] private GameObject _lostEnergyPrefab;
    [SerializeField] private GameObject _lostHealthPrefab;
    [SerializeField] private GameObject _addRerollPrefab;
    [SerializeField] private GameObject _addMoneyPrefab;
    [SerializeField] private GameObject _addCombMoneyPrefab;

    private CoinsSpawner _coinsSpawner;
    private void Start()
    {
        _coinsSpawner = FindObjectOfType<CoinsSpawner>();
    }

    public void UpdateStats(int characterNum, int damage, int defense)
    {
        if (characterNum == 0)
        {
            _charactersStats[0].text = damage.ToString();
            _charactersStats[1].text = defense.ToString();
        }
        else
        {
            _charactersStats[2].text = damage.ToString();
            _charactersStats[3].text = defense.ToString();
        }
    }

    public void UpdateReRollText(int value)
    {
        _rerollCountText.text = value.ToString();
    }

    public void UpdateHealth(int health, int characterNum)
    {
        _healthText[characterNum].text = health.ToString();
    }

    public void UpdateEnergy(float energy, int characterNum)
    {
        _energyVal[characterNum].value = energy;
    }

    public void ShowLostHealth(int val, int characterNum, bool isChecked = false)
    {
        int dir = (characterNum == 0) ? -1 : 1;
        string strName = "health";
        MoveSideway temp = FindObjectOfType<MoveSideway>();
        if (temp != null && !isChecked)
            if (temp.gameObject.name.Contains(strName) && temp.Direction == dir)
            {
                StartCoroutine(ShowTextLater(strName, val, characterNum));
                return;
            }

        Transform transform = (characterNum == 0) ? _playerLostTextTransform : _enemyLostTextTransform;
        GameObject textGO = Instantiate(_lostHealthPrefab, transform);
        textGO.GetComponent<Text>().text = "-" + val.ToString();
        textGO.GetComponent<MoveSideway>().Initialize(characterNum);
    }

    public void ShowLostEnergy(int val, int characterNum, bool isChecked = false)
    {
        int dir = (characterNum == 0) ? -1 : 1;
        string strName = "energy";
        MoveSideway temp = FindObjectOfType<MoveSideway>();
        if (temp != null && !isChecked)
            if (temp.gameObject.name.Contains(strName) && temp.Direction == dir)
            {
                StartCoroutine(ShowTextLater(strName, val, characterNum));
                return;
            }

        Transform transform = (characterNum == 0) ? _playerLostTextTransform : _enemyLostTextTransform;
        GameObject textGO = Instantiate(_lostEnergyPrefab, transform);
        textGO.GetComponent<Text>().text = (val > 0) ? "+" + val.ToString() : val.ToString();
        textGO.GetComponent<MoveSideway>().Initialize(characterNum);
    }

    public void ShowAddRerolCount(int val, int characterNum, bool isChecked = false)
    {
        int dir = (characterNum == 0) ? -1 : 1;
        string strName = "reroll";
        MoveSideway temp = FindObjectOfType<MoveSideway>();
        if (temp != null && !isChecked)
            if (temp.gameObject.name.Contains(strName) && temp.Direction == dir)
            {
                StartCoroutine(ShowTextLater(strName, val, characterNum));
                return;
            }

        Transform transform = (characterNum == 0) ? _playerLostTextTransform : _enemyLostTextTransform;
        GameObject textGO = Instantiate(_addRerollPrefab, transform);
        textGO.GetComponent<Text>().text = "+" + val.ToString();
        textGO.GetComponent<MoveSideway>().Initialize(characterNum);
    }

    public void ShowAudienceMoodCoef(float value)
    {
        _AMValueText.text = value.ToString();
        Color color = new Color((value >= 1.5f) ? (3.75f - value) / 2.5f : 0.9f, (value <= 1.5f) ? 0.4f * value + 0.3f : 0.9f, 0.3f);
        _AMIcon.color = color;
        _AMLabel.color = color;
        _AMValueText.color = color;

    }

    public void ShowAddMoney(int value)
    {
        GameObject temp = Instantiate(_addMoneyPrefab, transform);
        temp.GetComponentInChildren<Text>().text = "+" + value.ToString();
        temp.GetComponent<MoveSideway>().Initialize(false);
    }

    public void ShowAddCombMoney(int value, string combName)
    {
        GameObject temp = Instantiate(_addCombMoneyPrefab, transform);
        temp.transform.GetChild(2).GetComponent<Text>().text = combName;
        temp.transform.GetChild(0).GetComponent<Text>().text = "+"+value.ToString();
        temp.GetComponent<MoveSideway>().Initialize(true);
        _coinsSpawner.SpawnCoins(value);
    }

    private IEnumerator ShowTextLater(string methodName, int val, int characterNum)
    {
        yield return new WaitForSeconds(1.2f);
        switch (methodName)
        {
            case "energy":
                ShowLostEnergy(val, characterNum, true);
                break;
            case "health":
                ShowLostHealth(val, characterNum, true);
                break;
            case "reroll":
                ShowAddRerolCount(val, characterNum, true);
                break;
        }
    }

}
