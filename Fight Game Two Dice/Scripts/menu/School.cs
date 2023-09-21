using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;

public class School : MonoBehaviour
{
    [SerializeField] private Text _expText;
    [SerializeField] private Text _nameText;
    [SerializeField] private Image _energyBar;
    [SerializeField] private Text _gloryText;
    [SerializeField] private Text[] _statsPriceText;
    [SerializeField] private Text _totalPriceText;
    [SerializeField] private GameObject[] _injuriesObjs;
    [SerializeField] private GameObject _shadow;
    [SerializeField] private GameObject _reviveButton;
    [SerializeField] private GameObject[] SettingsScreen;
    [SerializeField] private GameObject[] UpdatesScreen;
    [SerializeField] private Outline[] UpdateScreenButtons;
    [SerializeField] private AudioSource _torchAS;

    private OwnGlads _ownGlads;
    private SwitchGlad _switchGlad;
    private MenuButtons _menuButtons;

    private OwnGladiator _glad;

    private int[] _statsPrice;
    private int[] _startStats;
    private int _expCount;
    private int _totalPrice;
    private int _gladNum;

    private bool _exist;

    public void Initialize(OwnGladiator glad, int gladNum)
    {
        _menuButtons = FindObjectOfType<MenuButtons>();
        _switchGlad = FindObjectOfType<SwitchGlad>();
        _ownGlads = FindObjectOfType<OwnGlads>();
        _statsPrice = new int[_statsPriceText.Length];
        SetGladText(glad, gladNum, true);
        UpdateScreenChange(false);
    }

    private void CheckDeaths(bool death, int injuries)
    {
        _exist = !death;
        _shadow.SetActive(false);
        _reviveButton.SetActive(false);
        foreach (var item in _injuriesObjs)
        {
            item.SetActive(false);
        }
        
        for (int i = 0; i < injuries; i++)
        {
            _injuriesObjs[i].SetActive(true);
        }

        if (death == false) return;
        _shadow.SetActive(true);
        _reviveButton.SetActive(true);
    }
    

    private void SetGladText(OwnGladiator glad, int num = -1, bool change = false)
    {
        if (change)
        {
            CheckDeaths(glad.isDead, glad.injuries);
            _totalPrice = 0;
            _totalPriceText.text = _totalPrice.ToString();
            _glad = new OwnGladiator();
            _glad.name = glad.name;
            _glad.stats = new int[glad.stats.Length];
            _glad.baseStats = new int[glad.stats.Length];
            _glad.experience = new int[glad.experience.Length];
            _startStats = new int[glad.stats.Length];
            for (int i = 0; i < glad.stats.Length; i++)
            {
                if (i < glad.experience.Length) _glad.experience[i] = glad.experience[i];
                _glad.stats[i] = glad.stats[i];
                _startStats[i] = glad.stats[i];
                _glad.baseStats[i] = glad.baseStats[i];
            }
            _glad.injuries = glad.injuries;
            _glad.isDead = glad.isDead;
            _glad.glory = glad.glory;
            _glad.energy = glad.energy;
            _gladNum = num;
            _expCount = _glad.experience[1];
            _expText.text = _expCount.ToString();
            _switchGlad.SetNormalColor();
            _nameText.text = _glad.name.ToUpper();
            _gloryText.text = _glad.glory.ToString();
        }

        _energyBar.fillAmount = (float)_glad.energy / (_glad.stats[1] * 5f);//(stats[1] * 5) is max energy
        for (int i = 0; i < _statsPriceText.Length; i++)
        {
            _statsPrice[i] = CalculateStatParice(_glad.stats[i], _glad.baseStats[i]);
            _statsPriceText[i].text = _statsPrice[i].ToString();
        }
    }

    private int CalculateStatParice(int val, int baseVal) => (int)((100 * (val + 1)) / (1 + 0.25f * Mathf.Log(baseVal, 2)) / 10);

    public void UpdateStat(int statNum)
    {
        if (_exist == false) return;
        _totalPrice += _statsPrice[statNum];
        _totalPriceText.text = _totalPrice.ToString();
        _glad.AddStat(statNum);
        SetGladText(_glad);
        _switchGlad.SetGreenColor(statNum);
        _statsPrice[statNum] = CalculateStatParice(_glad.stats[statNum], _glad.baseStats[statNum]);
        _statsPriceText[statNum].text = _statsPrice[statNum].ToString();
        _switchGlad.SetGlad(_glad);
    }

    public void DegradeStat(int statNum)
    {
        if (_exist == false) return;
        if (_startStats[statNum] == _glad.stats[statNum]) return;
        _glad.SubtractStat(statNum);
        _statsPrice[statNum] = CalculateStatParice(_glad.stats[statNum], _glad.baseStats[statNum]);
        _statsPriceText[statNum].text = _statsPrice[statNum].ToString();
        _totalPrice -= _statsPrice[statNum];
        _totalPriceText.text = _totalPrice.ToString();
        _switchGlad.SetGlad(_glad);
        SetGladText(_glad);
        if (_startStats[statNum] == _glad.stats[statNum]) _switchGlad.SetNormalColor(statNum);
    }

    public void BuyStats()
    {
        if (_exist == false) return;
        if (_expCount < _totalPrice || _totalPrice == 0) return;
        _torchAS.Play();
        _ownGlads.UpdateStats(_gladNum, _glad.stats, -_totalPrice);
        _expCount -= _totalPrice;
        _switchGlad.SetGlad();
    }

    public void CancelUpdate()
    {
        if (_exist == false) return;
        if (_totalPrice == 0) return;
        _switchGlad.SetGlad();
    }

    public void ReviveGlad()
    {
        if (_glad.injuries < 2)
        {
            _ownGlads.UpdateGladsLife(_gladNum, true);
            _glad = _ownGlads.GetOwnGladiator(_gladNum);
            SetGladText(_glad, _gladNum, true);
            return;
        }
        ProgressExp.AddExp(_glad.experience[0]);
        _ownGlads.RemoveGlad(_gladNum);
        ShowNewGlad();
    }

    public void ShowNewGlad()
    {
        if (_ownGlads.GetOwnGladsCount() == 0)
        {
            _menuButtons.CameraChanger(0);
            return;
        }
        _switchGlad.SetGlad(0);
    }

    public void ColorOwnGlad(Material mat)
    {
        int gladNum = _switchGlad.GetGladNum();
        if (_ownGlads.GetOwnGladiator(gladNum).isDead) return;
        GetComponent<ModulesShaker>().LoadElemets(_ownGlads.GetOwnGladiator(gladNum).elements, mat);
        _ownGlads.UpdateColor(gladNum, GetComponent<ModulesShaker>().GetMatNum());
    }

    public void ResetEnergy()
    {
        _ownGlads.ResetEnergy(_gladNum);
        _switchGlad.SetGlad(_gladNum);
    }

    public void SettingsScreenChange(bool colors)
    {
        SettingsScreen[0].SetActive(!colors);
        SettingsScreen[1].SetActive(colors);
    }

    public void UpdateScreenChange(bool school)
    {
        UpdatesScreen[0].SetActive(!school);
        UpdateScreenButtons[0].enabled = !school;
        UpdatesScreen[1].SetActive(school);
        UpdateScreenButtons[1].enabled = school;
    }

}
