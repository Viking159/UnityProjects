using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Structs;

public class MarketGlads : MonoBehaviour
{
    [SerializeField]
    private Material[] _projectorMats;
    [SerializeField]
    private Projector[] _gladsProjs;

    [SerializeField]
    private ModulesShaker[] _gladsModulesShaker;
    [SerializeField]
    private Text _gladsNameText;
    [SerializeField]
    private Text[] _gladsStatsText;
    [SerializeField]
    private Text[] _gladsCostText;
    [SerializeField]
    private GameObject _gladStat;
    [SerializeField]
    private GameObject[] _marketObjects;
    [SerializeField]
    private AudioSource _coinsAS;

    private OwnGlads _ownGlads;
    private TimeController _timeController;

    private List<MarketGladiator> _glads;

    private int _gladNum;
    private int _marketStartTime;

    private readonly byte marketGladsCount = 4;
    private readonly byte marketGladsStatsCount = 5;

    private readonly string PPKey = "MarketGlads";

    private readonly Vector3 normalScale = new Vector3(1.3f, 1.3f, 1.3f);

    private const int updateTime = 86400;

    public Action<int> OnMarketStartTimeSet;

    private static readonly string[] maleNames = { "Acidus", "Armipotens", "Asper", "Asprinus", "Atrox", "Audacius", "Audax", "Bellicus", "Belliger", "Caldus", "Carpophorus", "Celer", "Celerianus", "Celerius", "Citatus", "Citior", "Citus", "Crixus", "Durus", "Excitatus", "Exitiosus", "Ferocianus", "Ferox", "Ferullus", "Flamma", "Frontosus", "Funestus", "Imperiosus", "Importunus", "Incitatus", "Iniuriosus", "Malus", "Mordax", "Nocentianus", "Pollentius", "Potens", "Potentinus", "Priscus", "Properatus", "Prudes", "Rapidus", "Robaratus", "Roburius", "Robustianus", "Spiculus", "Strenuus", "Tetraites", "Tetricus", "Torentius", "Valentinus", "Validus", "Vegetianus", "Vegetinus", "Vegetius", "Vegetus", "Vehemens", "Velocianus", "Velox", "Verus", "Vigor", "Violens", "Viratus", "Virilianus", "Volantius" };
    private static readonly string[] femaleNames = {"Aspra", "Asprilla", "Audacia", "Bellica", "Bellicina", "Celeriana", "Cita", "Citata", "Excitata", "Ferocia", "Ferociana", "Ferocilla", "Ferocina", "Fervida", "Fervidia", "Importuna", "Incitata", "Mala", "Malificia", "Mevia", "Nerviana", "Nervilla", "Periculosa", "Potentia", "Potentilla", "Potentina", "Tetricilla", "Valentia", "Valentilla", "Valentina", "Violentilla"};
    private readonly int maleNamesCount = maleNames.Length;
    private readonly int femaleNamesCount = femaleNames.Length;
    private List<int> _maleNameIndexes;

    private void Awake()
    {
        SetGlads();
        for (int i = 0; i < marketGladsCount; i++)
        {
            _gladsModulesShaker[i].gameObject.GetComponent<Animator>().speed = Random.Range(0.8f, 1.2f);
        }
        _ownGlads = FindObjectOfType<OwnGlads>();
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnTimeStarted += LoadMarketStartTime;
        _timeController.OnTick += CheckMarketUpdate;
    }

    private void CheckMarketUpdate(int time)
    {
        if (_timeController.IsLocalTime) return;
        if ((time - _marketStartTime) >= updateTime)
        {
            _marketStartTime = _timeController.GetCurrTime();
            SetGlads(true);
        }       
    }

    public float GetTimeLeft(int time)
    {
        float timeLeft = 1 - ((time - _marketStartTime) / (float)updateTime);
        if ((time - _marketStartTime) >= updateTime)
        {
            _marketStartTime = _timeController.GetCurrTime();
            SetGlads(true);
            return 1;
        }
        return timeLeft;
    }

    private void LoadMarketStartTime(int time)
    {
        if (_timeController.IsLocalTime) return;
        _marketStartTime = _timeController.GetCurrTime();
        if (PlayerPrefs.HasKey("MarketStartTime") == false)
        {
            _marketStartTime = time;
            PlayerPrefs.SetString("MarketStartTime", _marketStartTime.ToString());
        }
        else
        {
            _marketStartTime = int.Parse(PlayerPrefs.GetString("MarketStartTime"));
        }
        GetTimeLeft(time);
        OnMarketStartTimeSet?.Invoke(time);
    }

    public void SetStatsText(int num)
    {
        if (_glads[num].bought) return;
        LightUpGlad(num);
        _gladsNameText.text = _glads[num].name.ToUpper();
        for (int i = 0; i < marketGladsStatsCount; i++)
        {
            _gladsStatsText[i].text = (i < 2)? (_glads[num].stats[i] * 5).ToString() : _glads[num].stats[i].ToString();
        }
    }

    public void LightUpGlad(int num = -1)
    {
        for (int i = 0; i < marketGladsCount; i++)
        {
            _gladsProjs[i].material = (i == num) ? _projectorMats[1] : _projectorMats[0];
        }
    }

    private void SetGlads(bool ignore = false)
    {
        if (LoadFromRR() == false || ignore == true)
        {
            LoadNameIndexes();
            _glads = new List<MarketGladiator>();
            for (int i = 0; i < marketGladsCount; i++)
            {

                MarketGladiator temp = new MarketGladiator();
                _gladsModulesShaker[i].RandomizeAll();
                temp.name = GetGladName();
                temp.elements = _gladsModulesShaker[i].GetElements().Split(' ');
                temp.stats = RandomizeStats(i, out temp.cost);
                temp.bought = false;
                _glads.Add(temp);
            }
            SaveToRR();
            SaveNameIndexes();
        }

        for (int i = 0; i < marketGladsCount; i++)
        {
            if (_glads[i].bought && ignore == false) { _gladsModulesShaker[i].gameObject.transform.localScale = Vector3.zero; continue; }
            if (ignore)
            {
                _gladsModulesShaker[i].gameObject.transform.localScale = normalScale;
                if (FindObjectOfType<MarketTimer>() != null)
                {
                    _marketObjects[i].SetActive(true);
                }
            }
            _gladsModulesShaker[i].Init();
            _gladsModulesShaker[i].LoadElemets(_glads[i].elements);
            _gladsCostText[i].text = _glads[i].cost.ToString();
        }
        
    }

    private string GetGladName()
    {
        int randIndex;
        int iter = 0;
        do
        {
            randIndex = Random.Range(0, maleNamesCount);
            if (iter++ > 1000)
                break;
        } while (_maleNameIndexes.Contains(randIndex));
        if (iter >= 1000)
        {
            print("!!! name iter = " + iter.ToString());
        }
        _maleNameIndexes.Add(randIndex);
        return maleNames[randIndex];
    }

    private void LoadNameIndexes()
    {
        if (PlayerPrefs.HasKey("GladNameIndexes") == false)
        {
            _maleNameIndexes = new List<int>();
            return;
        }
        string json = PlayerPrefs.GetString("GladNameIndexes");
        _maleNameIndexes = JsonUtility.FromJson<NameIndexes>(json).indexes;
    }

    private void SaveNameIndexes()
    {
        if (_maleNameIndexes.Count >= maleNamesCount * 0.9f) _maleNameIndexes = new List<int>();
        string json = JsonUtility.ToJson(new NameIndexes { indexes = _maleNameIndexes });
        PlayerPrefs.SetString("GladNameIndexes", json);
    }

    private int[] RandomizeStats(int num, out int cost)
    {
        int[] res = new int[marketGladsStatsCount];
        int[] range = new int[2];
        switch (num)
        {
            case 0:
                range[0] = 1;
                range[1] = 10;
                break;
            case 1:
                range[0] = 1;
                range[1] = 30;
                break;
            case 2:
                range[0] = 10;
                range[1] = 35;
                break;
            case 3:
                range[0] = 35;
                range[1] = 50;
                break;
        }
        res[0] = Random.Range(range[0], range[1] + 1);
        int temp = Random.Range(range[0], range[1] + 1);
        res[1] = temp == 1 ? 2 : temp;
        res[2] = Random.Range(range[0], range[1] + 1);
        res[3] = Random.Range(range[0], range[1] + 1);
        res[4] = Random.Range(range[0], range[1] + 1);
        cost = (int)(Mathf.Pow(res[0] + res[1] + res[2] + res[3] + res[4], 2) * Random.Range(0.7f, 1.3f)/10);
        return res;
    }

    public void GladStatShowButton(int num)
    {
        _gladNum = num;
        SetStatsText(num);
        _gladStat.SetActive(true);
    }

    public void GladStatHideButton()
    {
        _gladStat.SetActive(false);
        LightUpGlad();
    }

    public void BuyGlad()
    {
        if (Money.MoneyCount < _glads[_gladNum].cost) return;
        _coinsAS.Play();
        Money.Change(-_glads[_gladNum].cost);
        Buoght(_gladNum);
        _ownGlads.AddGlad(_glads[_gladNum]);
        GladStatHideButton();
        _marketObjects[_gladNum].SetActive(false);
        //AppMetrica.Instance.ReportEvent("Buy New Gladiator");
    }

    private void SaveToRR()
    {
        string json = JsonUtility.ToJson(new MarketGladsStruct{glads = _glads });
        PlayerPrefs.SetString(PPKey, json);
    }

    private bool LoadFromRR()
    {
        if (PlayerPrefs.HasKey(PPKey) == false) return false;

        string json = PlayerPrefs.GetString(PPKey);
        _glads = JsonUtility.FromJson<MarketGladsStruct>(json).glads;
        return true;
    }

    public void NewGlads()
    {
        SetGlads(true);
    }

    public bool IsBought(int num) => (num < 4) ? _glads[num].bought : false;
    public int GetCost(int num) => _glads[num].cost;
    public MarketGladiator GetMarketGlad(int num) => _glads[num];

    public void Buoght(int num)
    {
        _glads[num] = new MarketGladiator
        {
            name = _glads[num].name,
            bought = true,
            elements = _glads[num].elements,
            cost = _glads[num].cost,
            stats = _glads[num].stats
        };
        _gladsModulesShaker[num].gameObject.transform.localScale = Vector3.zero;
        SaveToRR();
    }

    public string GetPPKey() => PPKey;

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            PlayerPrefs.SetString("MarketStartTime", _marketStartTime.ToString());
        }
    }

    private void OnDestroy()
    {
        if (_timeController != null)
        {
            _timeController.OnTimeStarted -= LoadMarketStartTime;
            _timeController.OnTick += CheckMarketUpdate;
        }
        PlayerPrefs.SetString("MarketStartTime", _marketStartTime.ToString());

    }
}
