using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;

public class EndFight : MonoBehaviour
{
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private Text _resultText;
    [SerializeField] private Text _rewardText;
    [SerializeField] private Text _expText;
    [SerializeField] private GameObject _fatalInjuryText;

    private Enemy _enemy;
    private Player _player;
    private EnemySpawner _enemySpawner;
    private RosePS _rosePS;

    private readonly int _rewardCountStandart = 100;
    private readonly int _expCountStandart = 100;

    private void OnEnable()
    {
        _rosePS = FindObjectOfType<RosePS>();
        _enemy = FindObjectOfType<Enemy>();
        _enemy.OnEnemyDieActionLate += OnEnemyDie;
        _player = FindObjectOfType<Player>();
        _player.OnPlayerDie += Lose;
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _enemySpawner.OnEnemyInstantientAction += OnEnemyInstantientAction;
    }
    private void Start()
    {
        AppMetrica.Instance.ReportEvent("status: Start Fight");
        FBManager.instance.FireBaseFunction("L1_01_Fight");
    }

    private void OnEnemyInstantientAction()
    {
        _enemy.OnEnemyDieActionLate -= OnEnemyDie;
        _enemy = FindObjectOfType<Enemy>();
        if (_enemy != null) _enemy.OnEnemyDieActionLate += OnEnemyDie;
    }

    private void OnEnemyDie()
    {
        if (EnemySpawner.enemyCount >= ArenaFightSettings.EnemyCount) Win();
    }

    private void Win()
    {
        OwnGlads ownGlads = Camera.main.GetComponent<OwnGlads>();
        int mult = (ArenaFightSettings.FightType != 0)? 5 : ArenaFightSettings.EnemyCount;
        int expCount = _expCountStandart * mult;

        string str = PlayerPrefs.GetString("OwnGlad"+_player.GladNum.ToString() + "ArenaSuccess" + ArenaFightSettings.League.ToString());
        OwnGladArenaSuccess temp = JsonUtility.FromJson<OwnGladArenaSuccess>(str);
        int gloryVal;
        int needGloryVal;
        if (ArenaFightSettings.League == 1) { gloryVal = 1; needGloryVal = 50; }
        else if (ArenaFightSettings.League == 2) { gloryVal = 10; needGloryVal = 750; }
        else { gloryVal = 100; needGloryVal = 10000; }
        if (ArenaFightSettings.FightType == 1)
        {
            temp.percent += 10;
            if (temp.percent > 100) temp.percent = 100;
            temp.beatenEnemies[ArenaFightSettings.SpecialEnemyNum] = true;
            int tempCount = 0;
            if (ArenaFightSettings.SpecialEnemyNum <= 3)
            {
                gloryVal *= 25;
                for (int i = 0; i < 4; i++) if (temp.beatenEnemies[i]) tempCount++;
                temp.isSecondStepAvailable = (tempCount >= 2);
            }
            else if (ArenaFightSettings.SpecialEnemyNum <= 5)
            {
                gloryVal *= 50;
                temp.isThirdStepAvailable = true;
            }
            else
            {
                gloryVal *= 100;
                if (ArenaFightSettings.League <= 2)
                {
                    int tempLeagueCount = PlayerPrefs.GetInt("LeaguesCount");
                    PlayerPrefs.SetInt("LeaguesCount", tempLeagueCount + 1);
                    OwnGladArenaSuccess newLeague = new OwnGladArenaSuccess
                    {
                        percent = 0,
                        beatenEnemies = new bool[7] { false, false, false, false, false, false, false },
                        isFirstStepAvailable = false,
                        isSecondStepAvailable = false,
                        isThirdStepAvailable = false
                    };
                    PlayerPrefs.SetString("OwnGlad" + _player.GladNum.ToString() + "ArenaSuccess" + (ArenaFightSettings.League + 1).ToString(), JsonUtility.ToJson(newLeague));
                }
            }
        }
        else
        {
            if (temp.percent < 30) temp.percent = (temp.percent + 10 * ArenaFightSettings.EnemyCount > 30) ? (sbyte)30 : (sbyte)(temp.percent + 10 * ArenaFightSettings.EnemyCount);
            if (ArenaFightSettings.EnemyCount == 2) gloryVal *= 3;
            else if (ArenaFightSettings.EnemyCount == 3) gloryVal *= 5;
        }
        ownGlads.AddGloryToGlad(_player.GladNum, gloryVal);
        if (gloryVal >= needGloryVal && temp.isFirstStepAvailable == false) temp.isFirstStepAvailable = true;
        string resStr = JsonUtility.ToJson(temp);
        PlayerPrefs.SetString("OwnGlad"+_player.GladNum.ToString() + "ArenaSuccess" + ArenaFightSettings.League.ToString(), resStr);
        ownGlads.AddExp(_player.GladNum, expCount);
        ownGlads.ChangeEnergy(_player.GladNum, _player.TotalMovesCount);
        StartCoroutine(AfterEndFight(expCount, true));
    }

    private IEnumerator AfterEndFight(int expCount, bool win, bool fatalInjury = false)
    {
        if (win)
            _rosePS.StartPlay();
        FindObjectOfType<CrowdSound>().ChangeMood(win);
        yield return new WaitForSeconds(2.5f);
        _resultText.text = (win) ? "VICTORY" : "DEFEAT";
        _rewardText.text = _player.TotalRewardCount.ToString();
        _expText.text = expCount.ToString();
        _fatalInjuryText.SetActive(fatalInjury);
        _resultScreen.SetActive(true);
        if (win)
        {
            AppMetrica.Instance.ReportEvent("status: Win Fight");
            FBManager.instance.FireBaseFunction("L1_02_Victory ");
        }
        else
        {
            AppMetrica.Instance.ReportEvent("status: Fail Fight");
            FBManager.instance.FireBaseFunction("L1_02_Defeat");
        }
        Money.Change(_player.TotalRewardCount);
    }

    private void TrainLose()
    {
        int[] stats = new int[5] {6, 10, 10, 5, 20};
        OwnGladiator glad = new OwnGladiator
        {
            name = "Asper",
            elements = _player.GetComponent<ModulesShaker>().GetElements().Split(' '),
            baseStats = stats,
            stats = stats,
            experience = new int[2] { 150, 150 },
            injuries = 2,
            isDead = true,
            deathTime = FindObjectOfType<TimeController>().GetCurrTime(),
            glory = 5000,
            energy = 25
        };
        string json = JsonUtility.ToJson(new OwnedGladsStruct { glads = new List<OwnGladiator> { glad }  });
        PlayerPrefs.SetString("OwnGlads", json);
        StartCoroutine(AfterEndFight(0, false, true));
        PlayerPrefs.SetInt("Training", 1);
    }

    private void Lose()
    {
        if (ArenaFightSettings.FightType == -2)
        {
            TrainLose();
            return;
        }
        List<OwnGladiator> ownGlads = JsonUtility.FromJson<OwnedGladsStruct>(PlayerPrefs.GetString("OwnGlads")).glads;
        int resEnergy = ((_player.MaxEnergy - _player.TotalMovesCount) <= 0) ? 0 : _player.MaxEnergy - _player.TotalMovesCount;
        OwnGladiator glad = new OwnGladiator
        {
            name = ownGlads[_player.GladNum].name,
            elements = ownGlads[_player.GladNum].elements,
            baseStats = ownGlads[_player.GladNum].baseStats,
            stats = ownGlads[_player.GladNum].stats,
            experience = ownGlads[_player.GladNum].experience,
            injuries = ownGlads[_player.GladNum].injuries,
            isDead = true,
            deathTime = FindObjectOfType<TimeController>().GetCurrTime(),
            glory = ownGlads[_player.GladNum].glory,
            energy = resEnergy
        };
        ownGlads[_player.GladNum] = glad;
        string json = JsonUtility.ToJson(new OwnedGladsStruct { glads = ownGlads });
        PlayerPrefs.SetString("OwnGlads", json);
        bool fatal = Random.Range(0, 2) == 0;
        StartCoroutine(AfterEndFight(0, false, fatal));
    }
}
