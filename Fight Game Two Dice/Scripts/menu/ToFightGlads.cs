using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;


public class ToFightGlads : MonoBehaviour
{
    [SerializeField] private Text _gloryText;
    [SerializeField] private Text _nameText;
    [SerializeField] private Image _energyBar;
    [SerializeField] private Text[] _statsText;
    [SerializeField] private GameObject[] _swipeButtons;
    [SerializeField] private GameObject[] _injuriesObjs;
    [SerializeField] private CameraSwitcher[] _cameraSwitch;
    [SerializeField] private ModulesShaker[] _toFightGlads;
    [SerializeField] private GameObject[] _disableObjects;
    [SerializeField] private GameObject[] _leaguesLocker;

    private OwnGlads _ownGlads;

    private OwnGladiator _glad;

    private int _gladNum;
    private List<int> _gladsNums;
    private int _pointer;
    private int _cameraSwitchIndex;

    private readonly Vector3 _startGladPos = new Vector3(-0.75f, 0.24f, 18.20001f);
    private readonly Vector3 _startCamPos = new Vector3(-14.7f, 2.56f, 15.55f);

    private readonly float _delta = 2.19f;

    private int _leagueCount;

    public void Init()
    {
        _leagueCount = PlayerPrefs.GetInt("LeaguesCount"); // Можно закинуть в Start
        for (int i = 0; i < 3; i++)
        {
            _toFightGlads[i].gameObject.transform.localPosition = new Vector3(_startGladPos.x - _delta * i, _startGladPos.y, _startGladPos.z);
            _toFightGlads[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            _cameraSwitch[i].camera.gameObject.transform.localPosition = new Vector3(_startCamPos.x, _startCamPos.y, _startCamPos.z + _delta * i);
        }
        _ownGlads = FindObjectOfType<OwnGlads>();
        _gladsNums = new List<int>();
        for (int i = 0; i < _ownGlads.GetOwnGladsCount(); i++)
        {
            if (_ownGlads.GetOwnGladiator(i).isDead) continue;
            _gladsNums.Add(i);
        }
        int len = (_gladsNums.Count >= 3) ? 3 : _gladsNums.Count;
        for (int i = 0; i < len; i++)
        {
            _toFightGlads[i].gameObject.SetActive(true);
            _toFightGlads[i].LoadElemets(_ownGlads.GetElements(_gladsNums[i]));
        }
        _cameraSwitchIndex = 0;
        _pointer = 0;
        _gladNum = _gladsNums[_pointer];
        Invoke("CheckSwipeButtons", 1.08f);
        _glad = _ownGlads.GetOwnGladiator(_gladNum);
        SetGlad();
        CheckLeaguesAccess();
    }

    public void SetGlad()
    {
        foreach (var item in _injuriesObjs)
        {
            item.SetActive(false);
        }
        _glad = _ownGlads.GetOwnGladiator(_gladNum);
        _gloryText.text = _glad.glory.ToString();
        _nameText.text = _glad.name.ToUpper();
        for (int i = 0; i < _statsText.Length; i++)
        {
            if (i < _glad.injuries) _injuriesObjs[i].SetActive(true);
            _statsText[i].text = (i < 2) ? (_glad.stats[i] * 5).ToString() : _glad.stats[i].ToString();
        }
        _energyBar.fillAmount = (float)_glad.energy/(_glad.stats[1] * 5f);//(stats[1] * 5) is max energy
    }

    private IEnumerator HideObjects(float time = 1f)
    {
        Queue<bool> PrValuse = new Queue<bool>();
        foreach (var item in _disableObjects)
        {
            PrValuse.Enqueue(item.activeInHierarchy);
            item.SetActive(false);
        }
        yield return new WaitForSeconds(time);
        foreach (var item in _disableObjects) item.SetActive(PrValuse.Dequeue());
        CheckSwipeButtons();
    }

    public void SetGladNum(bool right)
    {
        if ((right && _pointer >= _gladsNums.Count - 1) || (right == false && _pointer <= 0)) return;
        StartCoroutine(HideObjects());
        if (right && _pointer < _gladsNums.Count - 1)
        {
            _gladNum = _gladsNums[++_pointer];
        }
        else if (right == false && _pointer > 0)
        {
            _gladNum = _gladsNums[--_pointer];

        }
        MoveCamera(right);
        StartCoroutine(MoveGlads(right));
        SetGlad();
        CheckLeaguesAccess();
    }

    private void CheckLeaguesAccess()
    {
        _leaguesLocker[0].SetActive(false);
        _leaguesLocker[1].SetActive(false);
        if (_leagueCount <= 1) _leaguesLocker[0].SetActive(true);
        if (_leagueCount <= 2) _leaguesLocker[1].SetActive(true);
    }

    private void MoveCamera(bool right)
    {
        int prIndex = _cameraSwitchIndex;
        _cameraSwitch[_cameraSwitchIndex].camera.SetActive(false);
        _cameraSwitchIndex = (_cameraSwitchIndex + 1) % 2;
        _cameraSwitch[_cameraSwitchIndex].camera.transform.localPosition = _cameraSwitch[prIndex].camera.transform.localPosition;
        if (right && _pointer <= _gladsNums.Count - 1)
        {
            Vector3 temp = _cameraSwitch[_cameraSwitchIndex].camera.transform.localPosition;
            _cameraSwitch[_cameraSwitchIndex].camera.transform.localPosition = new Vector3(temp.x, temp.y, temp.z + _delta);
        }
        else if (right == false && _pointer >= 0)
        {
            Vector3 temp = _cameraSwitch[_cameraSwitchIndex].camera.transform.localPosition;
            _cameraSwitch[_cameraSwitchIndex].camera.transform.localPosition = new Vector3(temp.x, temp.y, temp.z - _delta);
        }
        _cameraSwitch[_cameraSwitchIndex].camera.SetActive(true);
    }

    private IEnumerator MoveGlads(bool right)
    {
        yield return new WaitForSeconds(0.15f);
        if (right)
        {
            if (_pointer <= 1 || _pointer == _gladsNums.Count - 1) yield break; 
            for (int i = 0; i < 3; i++)
            {
                Vector3 temp = _toFightGlads[i].gameObject.transform.localPosition;
                _toFightGlads[i].gameObject.transform.localPosition = new Vector3(temp.x - _delta, temp.y, temp.z);
                _toFightGlads[i].LoadElemets(_ownGlads.GetElements(_gladsNums[_pointer - 1 + i]));
            }
        }
        else
        {
            if (_pointer == _gladsNums.Count - 2 || _pointer == 0) yield break; 
            for (int i = 0; i < 3; i++)
            {
                Vector3 temp = _toFightGlads[i].gameObject.transform.localPosition;
                _toFightGlads[i].gameObject.transform.localPosition = new Vector3(temp.x + _delta, temp.y, temp.z);
                _toFightGlads[i].LoadElemets(_ownGlads.GetElements(_gladsNums[_pointer - 1 + i]));
            }
        }
    }

    private void CheckSwipeButtons()
    {
        if (_pointer == 0) _swipeButtons[0].SetActive(false);
        else _swipeButtons[0].SetActive(true);
        if (_pointer == _gladsNums.Count - 1) _swipeButtons[1].SetActive(false);
        else _swipeButtons[1].SetActive(true);
    }

    public IEnumerator FlashEnergy()
    {
        float val = _energyBar.fillAmount;
        _energyBar.fillAmount = 1;
        Color normColor = new Color(0.9450981f, 0.8470589f, 0.3568628f);
        Color temp = Color.white;
        while (temp.g > 0.1f)
        {
            temp.g -= 0.05f;
            temp.b -= 0.05f;
            _energyBar.color = temp;
            yield return null;
        }
        while (temp.g < 0.9f)
        {
            temp.g += 0.05f;
            temp.b += 0.05f;
            _energyBar.color = temp;
            yield return null;
        }
        _energyBar.fillAmount = val;
        _energyBar.color = normColor;
    }
    public int GetGladNum() => _gladNum;

    public int GetGladGlory() => _glad.glory;

    public double GetGladEnergy() => _glad.energy;

    //public OwnGladiator GetGladToFight() => _glad;
}
