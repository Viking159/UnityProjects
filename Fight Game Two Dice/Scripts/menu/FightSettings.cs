using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Structs;
using System;

public class FightSettings : MonoBehaviour
{
    [Header("Portraits")]
    [SerializeField]
    private Sprite[] _league1GladsPortraits;
    [SerializeField]
    private Sprite[] _league2GladsPortraits;
    [SerializeField]
    private Sprite[] _league3GladsPortraits;
    [Space]

    [SerializeField]
    private Text _leagueName;
    [Space]

    [Header("Glory")]
    [SerializeField]
    private Slider _leaguePercent;
    [SerializeField]
    private Text _gloryText;
    [SerializeField] 
    private GameObject _warningObj;
    [SerializeField] 
    private Text _warningText;
    [Space]

    [SerializeField]
    private GameObject[] _chooseSceneObjs;
    [SerializeField]
    private Outline[] _gladsPortraitsOutline;
    [SerializeField]
    private Outline _skirmishOutline;
    [SerializeField]
    private GameObject _leagueMenu;
    [SerializeField]
    private Text _enemyCountText;
    private ToFightGlads _toFightGlads;
    private Image[] _gladsPortraitsIamge;

    //private int _glory;
    private int _league = - 1;
    private int _enemyCount;
    private int _specialEnemyGladNum;
    private sbyte _fightType; // -1 - not choosen; 0 - skirmish; 1 - specialEnemy; -2 - training

    private OwnGladArenaSuccess _currLeagueInfo;

    //private const string PPKey = "LeagueInfo"; // + leagueNum
    private const byte leagueEnemyCount = 7;

    private readonly string[] leaguesName = new string[4] { "CATANIA", "PISA", "CAPUA", "ROME" };

    public static readonly string SpecialEnemie1Json =  "{\"glads\":[{\"elements\":[\"2\",\"000\",\"000\",\"000\",\"008\",\"000\",\"000\",\"000\",\"005\",\"010\",\"006\",\"005\",\"010\",\"005\",\"010\",\"007\",\"001\",\"000\",\"010\",\"004\",\"001\",\"009\",\"008\",\"009\",\"008\",\"200\",\"0\",\"3\"],\"stats\":[15,15,10,10]},{\"elements\":[\"2\",\"000\",\"000\",\"000\",\"008\",\"008\",\"008\",\"010\",\"002\",\"008\",\"009\",\"005\",\"006\",\"010\",\"009\",\"004\",\"004\",\"008\",\"009\",\"004\",\"004\",\"004\",\"003\",\"005\",\"005\",\"200\",\"0\",\"2\"],\"stats\":[15,10,11,16]},{\"elements\":[\"0\",\"000\",\"000\",\"000\",\"005\",\"000\",\"008\",\"010\",\"007\",\"000\",\"000\",\"007\",\"001\",\"008\",\"001\",\"003\",\"006\",\"002\",\"005\",\"004\",\"002\",\"010\",\"007\",\"003\",\"003\",\"200\",\"3\",\"3\"],\"stats\":[13,13,13,13]},{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"008\",\"000\",\"000\",\"000\",\"008\",\"003\",\"010\",\"004\",\"001\",\"003\",\"009\",\"007\",\"007\",\"009\",\"000\",\"010\",\"008\",\"010\",\"003\",\"008\",\"010\",\"200\",\"6\",\"0\"],\"stats\":[10,10,15,15]},{\"elements\":[\"5\",\"000\",\"000\",\"000\",\"006\",\"008\",\"008\",\"008\",\"010\",\"005\",\"003\",\"002\",\"001\",\"009\",\"005\",\"002\",\"001\",\"005\",\"006\",\"010\",\"006\",\"003\",\"004\",\"002\",\"006\",\"200\",\"0\",\"4\"],\"stats\":[20,10,10,20]},{\"elements\":[\"0\",\"000\",\"000\",\"000\",\"005\",\"008\",\"008\",\"003\",\"001\",\"001\",\"003\",\"006\",\"004\",\"010\",\"000\",\"010\",\"000\",\"000\",\"010\",\"008\",\"009\",\"001\",\"002\",\"009\",\"009\",\"4\",\"8\",\"0\"],\"stats\":[10,20,20,10]},{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"000\",\"000\",\"008\",\"003\",\"008\",\"008\",\"000\",\"002\",\"009\",\"008\",\"001\",\"001\",\"008\",\"000\",\"000\",\"003\",\"002\",\"002\",\"006\",\"001\",\"001\",\"8\",\"8\",\"4\"],\"stats\":[20,20,20,20]}]}";
    public static readonly string SpecialEnemie2Json = "{\"glads\":[{\"elements\":[\"0\",\"000\",\"000\",\"000\",\"010\",\"000\",\"000\",\"003\",\"010\",\"006\",\"007\",\"010\",\"005\",\"007\",\"000\",\"001\",\"008\",\"007\",\"001\",\"005\",\"003\",\"002\",\"005\",\"008\",\"002\",\"200\",\"1\",\"0\"],\"stats\":[23,23,23,23]},{\"elements\":[\"3\",\"000\",\"000\",\"000\",\"004\",\"000\",\"008\",\"008\",\"008\",\"004\",\"001\",\"003\",\"009\",\"007\",\"007\",\"006\",\"000\",\"001\",\"009\",\"008\",\"008\",\"010\",\"004\",\"001\",\"010\",\"200\",\"3\",\"200\"],\"stats\":[10,22,50,10]},{\"elements\":[\"4\",\"000\",\"000\",\"000\",\"010\",\"008\",\"000\",\"003\",\"008\",\"006\",\"009\",\"000\",\"007\",\"007\",\"006\",\"007\",\"001\",\"008\",\"008\",\"007\",\"004\",\"010\",\"004\",\"004\",\"000\",\"200\",\"6\",\"2\"],\"stats\":[22,10,20,40]},{\"elements\":[\"5\",\"000\",\"000\",\"000\",\"002\",\"008\",\"000\",\"010\",\"005\",\"008\",\"006\",\"006\",\"004\",\"003\",\"010\",\"009\",\"010\",\"008\",\"001\",\"001\",\"002\",\"009\",\"003\",\"009\",\"009\",\"0\",\"5\",\"0\"],\"stats\":[12,10,10,60]},{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"008\",\"008\",\"008\",\"000\",\"002\",\"000\",\"006\",\"005\",\"000\",\"009\",\"009\",\"008\",\"009\",\"003\",\"009\",\"001\",\"005\",\"006\",\"003\",\"005\",\"005\",\"4\",\"2\",\"3\"],\"stats\":[40,20,20,30]},{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"002\",\"008\",\"000\",\"010\",\"010\",\"004\",\"005\",\"003\",\"001\",\"003\",\"009\",\"005\",\"003\",\"004\",\"003\",\"007\",\"002\",\"004\",\"006\",\"004\",\"010\",\"200\",\"3\",\"2\"],\"stats\":[40,20,20,30]},{\"elements\":[\"5\",\"000\",\"000\",\"000\",\"000\",\"008\",\"008\",\"010\",\"002\",\"008\",\"003\",\"010\",\"003\",\"002\",\"008\",\"007\",\"008\",\"010\",\"002\",\"001\",\"000\",\"003\",\"000\",\"002\",\"003\",\"200\",\"4\",\"3\"],\"stats\":[30,30,30,30]}]}";
    public static readonly string SpecialEnemie3Json = "{\"glads\":[{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"008\",\"000\",\"008\",\"010\",\"002\",\"009\",\"001\",\"001\",\"002\",\"009\",\"003\",\"006\",\"003\",\"002\",\"006\",\"010\",\"010\",\"000\",\"005\",\"008\",\"004\",\"200\",\"6\",\"3\"],\"stats\":[37,35,35,32]},{\"elements\":[\"0\",\"000\",\"000\",\"000\",\"000\",\"008\",\"000\",\"008\",\"000\",\"000\",\"009\",\"002\",\"005\",\"010\",\"007\",\"006\",\"008\",\"009\",\"002\",\"006\",\"005\",\"005\",\"009\",\"005\",\"002\",\"7\",\"8\",\"0\"],\"stats\":[31,38,31,38]},{\"elements\":[\"2\",\"000\",\"000\",\"000\",\"008\",\"000\",\"000\",\"008\",\"000\",\"004\",\"000\",\"009\",\"007\",\"007\",\"009\",\"006\",\"003\",\"009\",\"001\",\"006\",\"008\",\"006\",\"000\",\"004\",\"009\",\"7\",\"7\",\"3\"],\"stats\":[25,39,42,31]},{\"elements\":[\"6\",\"000\",\"000\",\"000\",\"005\",\"008\",\"008\",\"008\",\"008\",\"007\",\"001\",\"008\",\"009\",\"002\",\"000\",\"001\",\"006\",\"009\",\"009\",\"002\",\"005\",\"001\",\"005\",\"002\",\"001\",\"200\",\"0\",\"200\"],\"stats\":[37,22,23,58]},{\"elements\":[\"4\",\"000\",\"000\",\"000\",\"007\",\"008\",\"000\",\"010\",\"005\",\"000\",\"007\",\"010\",\"010\",\"009\",\"005\",\"003\",\"003\",\"001\",\"000\",\"003\",\"004\",\"010\",\"010\",\"000\",\"010\",\"200\",\"7\",\"4\"],\"stats\":[20,45,65,20]},{\"elements\":[\"0\",\"000\",\"000\",\"000\",\"002\",\"000\",\"000\",\"000\",\"010\",\"006\",\"010\",\"004\",\"008\",\"006\",\"009\",\"010\",\"000\",\"003\",\"009\",\"007\",\"001\",\"006\",\"007\",\"004\",\"004\",\"200\",\"7\",\"2\"],\"stats\":[45,20,25,60]},{\"elements\":[\"1\",\"000\",\"000\",\"000\",\"007\",\"000\",\"000\",\"003\",\"008\",\"006\",\"004\",\"006\",\"010\",\"005\",\"000\",\"004\",\"006\",\"002\",\"005\",\"006\",\"008\",\"002\",\"009\",\"007\",\"008\",\"3\",\"1\",\"4\"],\"stats\":[40,40,40,40]}]}";
    
    private void Start()
    {
        _gladsPortraitsIamge = new Image[_gladsPortraitsOutline.Length];
        for (int i = 0; i < _gladsPortraitsOutline.Length; i++)
        {
            _gladsPortraitsIamge[i] = _gladsPortraitsOutline[i].GetComponent<Image>();
        }
    }

#if UNITY_EDITOR

    public void AddGlory()
    {
        OwnGlads ownGlads = FindObjectOfType<OwnGlads>();
        string str = PlayerPrefs.GetString("OwnGlad" + _toFightGlads.GetGladNum() + "ArenaSuccess" + _league.ToString());
        OwnGladArenaSuccess temp = JsonUtility.FromJson<OwnGladArenaSuccess>(str);
        //string str = PlayerPrefs.GetString("LeagueInfo" + _league.ToString());
        //LeagueInfo temp = JsonUtility.FromJson<LeagueInfo>(str);
        ownGlads.AddGloryToGlad(_toFightGlads.GetGladNum(), 20);
        //_glory += 20;
        //PlayerPrefs.SetInt("PlayerGlory", _glory);
        int tempGloryNeed;
        if (_league == 1) tempGloryNeed = 50;
        else if (_league == 2) tempGloryNeed = 750;
        else tempGloryNeed = 750;

        if (ownGlads.GetOwnGladiator(_toFightGlads.GetGladNum()).glory/*_glory*/ >= tempGloryNeed && temp.isFirstStepAvailable == false)
        {
            temp.isFirstStepAvailable = true;
            string resStr = JsonUtility.ToJson(temp);
            PlayerPrefs.SetString("OwnGlad" + _toFightGlads.GetGladNum() + "ArenaSuccess" + _league.ToString(), resStr);
            LoadLeagueInfo();
            VisualLeagueInfo();
            CheckWarning(_league);
        }
        _gloryText.text = ownGlads.GetOwnGladiator(_toFightGlads.GetGladNum()).glory.ToString();
    }

#endif

    private void OnEnable()
    {
        _fightType = -1;
        _toFightGlads = gameObject.GetComponent<ToFightGlads>();
    }


    private bool LoadLeagueInfo()
    {
        if (PlayerPrefs.HasKey("OwnGlad" + _toFightGlads.GetGladNum() + "ArenaSuccess" + _league.ToString()) == false) return false;
        string json = PlayerPrefs.GetString("OwnGlad" + _toFightGlads.GetGladNum() + "ArenaSuccess" + _league.ToString());
        _currLeagueInfo = JsonUtility.FromJson<OwnGladArenaSuccess>(json);
        return true;
    }

    private void SaveLeagueInfo()
    {
        string json = JsonUtility.ToJson(_currLeagueInfo);
        PlayerPrefs.SetString("OwnGlad" + _toFightGlads.GetGladNum() + "ArenaSuccess" + _league.ToString(), json);
    }

    private void VisualLeagueInfo()
    {
        _leagueName.text = leaguesName[_league-1];
        _leaguePercent.value = _currLeagueInfo.percent / 100f;
        for (int i = 0; i < leagueEnemyCount; i++)
        {
           _gladsPortraitsOutline[i].gameObject.transform.GetChild(0).gameObject.SetActive(_currLeagueInfo.beatenEnemies[i]);
            if (i <= 3) _gladsPortraitsOutline[i].gameObject.transform.GetChild(1).gameObject.SetActive(!_currLeagueInfo.isFirstStepAvailable);
            if (i > 3 && i < leagueEnemyCount - 1) _gladsPortraitsOutline[i].gameObject.transform.GetChild(1).gameObject.SetActive(!_currLeagueInfo.isSecondStepAvailable);
        }
        _gladsPortraitsOutline[leagueEnemyCount - 1].gameObject.transform.GetChild(1).gameObject.SetActive(!_currLeagueInfo.isThirdStepAvailable);
        
    }

    public void LeagueButton(int league)
    {
        //if (_toFightGlads.GetGladEnergy() <= 1)
        //{
        //    StartCoroutine(_toFightGlads.FlashEnergy());
        //    return;
        //}
        _league = league;
        if (LoadLeagueInfo() == false)
        {
            if (PlayerPrefs.HasKey("LeaguesCount"))
            {
                if (league > PlayerPrefs.GetInt("LeaguesCount")) return;
            }
            else
            {
                if (league != 1) return;
                PlayerPrefs.SetInt("LeaguesCount", 1);
            }
            _currLeagueInfo.percent = 0;
            _currLeagueInfo.isFirstStepAvailable = false;
            _currLeagueInfo.isSecondStepAvailable = false;
            _currLeagueInfo.isThirdStepAvailable = false;
            _currLeagueInfo.beatenEnemies = new bool[7] { false, false, false, false, false, false, false };
            SaveLeagueInfo();
        }
        _gloryText.text = _toFightGlads.GetGladGlory().ToString();
        VisualLeagueInfo();
        foreach (var item in _chooseSceneObjs) item.SetActive(false);
        DisableAllOutline();
        Sprite[] tempSprites;
        if (league == 1) tempSprites = _league1GladsPortraits;
        else if (league == 2) tempSprites = _league2GladsPortraits;
        else tempSprites = _league3GladsPortraits;
        for (int i = 0; i < _gladsPortraitsIamge.Length; i++)
        {
            _gladsPortraitsIamge[i].sprite = tempSprites[i];
        }
        _leagueMenu.SetActive(true);
        _enemyCount = 1;
        _enemyCountText.text = _enemyCount.ToString();
        CheckWarning(_league);
    }

    public void CheckWarning(int league)
    {
        int tempGloryNeed;
        if (_league == 1) tempGloryNeed = 50;
        else if (_league == 2) tempGloryNeed = 750;
        else tempGloryNeed = 750;
        if (tempGloryNeed > _toFightGlads.GetGladGlory())
        {
            _warningObj.SetActive(true);
            _warningText.text = GetWarningText(_league);
        }
        else
        {
            _warningObj.SetActive(false);
        }
        
    }

    private string GetWarningText(int league)
    {
        int temp = 0;
        switch (league)
        {
            case 1:
                temp = 50;
                break;
            case 2:
                temp = 750;
                break;
            case 3:
                temp = 10000;
                break;
        }
        return "TO CHALLENGE CHAMPIONS YOU NEED A GLADIATOR WITH " + temp.ToString() + " GLORY";
    }

    public void SpecialGladButton(int gladNum)
    {
        if ((_currLeagueInfo.beatenEnemies[gladNum]) || (gladNum <= 3 && _currLeagueInfo.isFirstStepAvailable == false) || (gladNum > 3 && _currLeagueInfo.isSecondStepAvailable == false) || (gladNum == 6 && _currLeagueInfo.isThirdStepAvailable == false)) return;
        _skirmishOutline.enabled = false;
        _enemyCount = 1;
        _fightType = 1;
        _enemyCountText.text = _enemyCount.ToString();
        for (int i = 0; i < _gladsPortraitsOutline.Length; i++) _gladsPortraitsOutline[i].enabled = (i == gladNum);
        _specialEnemyGladNum = gladNum;
    }

    public void SkirmushButton()
    {
        _skirmishOutline.effectColor = Color.black;
        for (int i = 0; i < _gladsPortraitsOutline.Length; i++) if (_currLeagueInfo.beatenEnemies[i] == false) _gladsPortraitsOutline[i].enabled = false;
        _skirmishOutline.enabled = true;
        _fightType = 0;
    }

    public void ChangeEnemyCount(bool positive)
    {
        SkirmushButton();
        if (positive && _enemyCount < 3) _enemyCount++;
        else if (positive == false && _enemyCount > 1) _enemyCount--;
        else return;
        _enemyCountText.text = _enemyCount.ToString();
    }

    private void DisableAllOutline(bool reverse = false)
    {
        for (int i = 0; i < _gladsPortraitsOutline.Length; i++)
            if ((_currLeagueInfo.beatenEnemies[i] == false) && ((i <= 3 && _currLeagueInfo.isFirstStepAvailable) || (i > 3 && _currLeagueInfo.isSecondStepAvailable && i < 6) || (i == 6 && _currLeagueInfo.isThirdStepAvailable)))
                _gladsPortraitsOutline[i].enabled = reverse;
        _skirmishOutline.enabled = reverse;
    }

    public void DisableLeagueMenu()
    {
        if (_leagueMenu.activeInHierarchy)
            foreach (var item in _chooseSceneObjs) item.SetActive(true);
        _leagueMenu.SetActive(false);
        DisableAllOutline();
        _enemyCount = 1;
        _fightType = -1;
        _enemyCountText.text = _enemyCount.ToString();
    }

    private IEnumerator EmptyChoiceAnim()
    {
        Color color = Color.red;
        color.a = 0;
        DisableAllOutline(true);
        while (color.a < 1)
        {
            color.a += 0.1f;
            for (int i = 0; i < _gladsPortraitsOutline.Length; i++) 
                if ((_currLeagueInfo.beatenEnemies[i] == false) && ((i <= 3 && _currLeagueInfo.isFirstStepAvailable) || (i > 3 && _currLeagueInfo.isSecondStepAvailable && i < 6) || (i == 6 && _currLeagueInfo.isThirdStepAvailable)))
                    _gladsPortraitsOutline[i].effectColor = color;
            _skirmishOutline.effectColor = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);
        while (color.a > 0)
        {
            color.a -= 0.1f;
            for (int i = 0; i < _gladsPortraitsOutline.Length; i++)
                if ((_currLeagueInfo.beatenEnemies[i] == false) && ((i <= 3 && _currLeagueInfo.isFirstStepAvailable) || (i > 3 && _currLeagueInfo.isSecondStepAvailable && i < 6) || (i == 6 && _currLeagueInfo.isThirdStepAvailable)))
                    _gladsPortraitsOutline[i].effectColor = color;

            _skirmishOutline.effectColor = color;
            yield return null;
        }
        DisableAllOutline();
        foreach (var item in _gladsPortraitsOutline)
        {
            item.effectColor = Color.black;
        }
        _skirmishOutline.effectColor = Color.black;
    }

    public void FightButton()
    {
        if (PlayerPrefs.HasKey("League")) PlayerPrefs.DeleteKey("League");
        if (PlayerPrefs.HasKey("EnemyCount")) PlayerPrefs.DeleteKey("EnemyCount");
        if (PlayerPrefs.HasKey("FightType")) PlayerPrefs.DeleteKey("FightType");
        if (_fightType == -1)
        {
            StartCoroutine(EmptyChoiceAnim());
            return;
        }
        else if (_fightType == 0)
        {
            PlayerPrefs.SetInt("EnemyCount", _enemyCount);
        }
        else
        {
            PlayerPrefs.SetInt("SpecialEnemyGladNum", _specialEnemyGladNum);
        }
        PlayerPrefs.SetInt("League", _league);
        PlayerPrefs.SetInt("FightType", _fightType);
        PlayerPrefs.SetInt("ToFightGladNum", _toFightGlads.GetGladNum());
        PlayerPrefs.SetString("SceneName", "main");
        SceneManager.LoadScene("loading");
    }

}
