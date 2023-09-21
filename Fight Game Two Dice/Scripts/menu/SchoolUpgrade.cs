using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;




public class SchoolUpgrade : MonoBehaviour
{
    [SerializeField] private Text[] _statsLvlText;
    [SerializeField] private Text[] _statsPriceText;
    [SerializeField] private Text _totalPriceText;

    private SchoolUpgradeInfo _schoolUpgradeInfo;
    private SchoolUpgradeInfo _currSchoolUpgradeInfo;

    private int _totalPrice;

    private const string PPKey = "SchoolUpgradeInfo";

    private void OnEnable()
    {
        _totalPrice = 0;
        LoadData();
    }


    private void LoadData()
    {
        if (PlayerPrefs.HasKey(PPKey))
            _schoolUpgradeInfo = JsonUtility.FromJson<SchoolUpgradeInfo>(PlayerPrefs.GetString(PPKey));
        else
            _schoolUpgradeInfo = new SchoolUpgradeInfo
            {
                stats = new List<int> {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

        _currSchoolUpgradeInfo = new SchoolUpgradeInfo();
        _currSchoolUpgradeInfo.stats = new List<int>();
        for (int i = 0; i < _schoolUpgradeInfo.stats.Count; i++)
            _currSchoolUpgradeInfo.stats.Add(_schoolUpgradeInfo.stats[i]);

        SaveData();
        VisualizeMenu();
    }

    private int F(int lvl)
    {
        return 100 * lvl + 50;
    }

    private void VisualizeMenu(int index = -1)
    {
        _totalPriceText.text = _totalPrice.ToString();
        if (index != -1)
        {
            _statsLvlText[index].text = _currSchoolUpgradeInfo.stats[index].ToString();
            _statsPriceText[index].text = (F(_currSchoolUpgradeInfo.stats[index])).ToString();
            return;
        }
        for (int i = 0; i < _currSchoolUpgradeInfo.stats.Count; i++)
        {
            _statsLvlText[i].text = _currSchoolUpgradeInfo.stats[i].ToString();
            _statsPriceText[i].text = (F(_currSchoolUpgradeInfo.stats[i])).ToString();
        }
    }

    public void ChangeStat(StatNames statName, bool add, int max)
    {
        int index = (int)statName;
        if (add)
        {
            if (_currSchoolUpgradeInfo.stats[index] < max)
            {
                _totalPrice += F(_currSchoolUpgradeInfo.stats[index]);
                _currSchoolUpgradeInfo.stats[index]++;
                VisualizeMenu(index);
            }
            return;
        }
        if (_currSchoolUpgradeInfo.stats[index] > _schoolUpgradeInfo.stats[index])
        {
            _currSchoolUpgradeInfo.stats[index]--;
            _totalPrice -= F(_currSchoolUpgradeInfo.stats[index]);
            VisualizeMenu(index);
        }
    }

    public void BuyStats()
    {
        if (_totalPrice > Money.MoneyCount) return;
        Money.Change(-_totalPrice);
        SaveData();
    }

    public void CancelStats()
    {
        ResetCurrSchoolUpgrade();
        SaveData();
        VisualizeMenu();
    }

    private void ResetCurrSchoolUpgrade()
    {
        _totalPrice = 0;
        _currSchoolUpgradeInfo = new SchoolUpgradeInfo();
        _currSchoolUpgradeInfo.stats = new List<int>();
        for (int i = 0; i < _schoolUpgradeInfo.stats.Count; i++)
            _currSchoolUpgradeInfo.stats.Add(_schoolUpgradeInfo.stats[i]);
    }

    private void SaveData()
    {
        _schoolUpgradeInfo = _currSchoolUpgradeInfo;
        _schoolUpgradeInfo = new SchoolUpgradeInfo();
        _schoolUpgradeInfo.stats = new List<int>();
        for (int i = 0; i < _currSchoolUpgradeInfo.stats.Count; i++)
            _schoolUpgradeInfo.stats.Add(_currSchoolUpgradeInfo.stats[i]);
        PlayerPrefs.SetString(PPKey, JsonUtility.ToJson(_schoolUpgradeInfo));
    }

    private void OnDisable()
    {
        ResetCurrSchoolUpgrade();
        SaveData();
    }

}
