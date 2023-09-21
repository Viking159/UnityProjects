using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[System.Serializable]
public class CameraSwitcher
{
    public int locationNum;
    public GameObject camera;
}

public class MenuButtons : MonoBehaviour
{
    [Header("Menu buttons")]
    [SerializeField] private GameObject[] _menuButtons;
    [SerializeField] private GameObject _backButton;
    [Space]

    [Header("Areas objs")]
    [SerializeField] private GameObject[] _marketObjects;
    [SerializeField] private GameObject[] _coliseumObjs;
    [SerializeField] private GameObject[] _schoolObjs;
    [SerializeField] private GameObject[] _blacksmithObjs;
    [SerializeField] private GameObject _gladStat;
    [SerializeField] private GameObject _leagueMenu;
    [SerializeField] private MarketGlads _marketGlads;
    [Space]

    [Header("Stats info")]
    [SerializeField] private GameObject _statsInfo;
    [SerializeField] private GameObject[] _statsObjs;
    [Space]

    [Header("Camera switch")]
    [SerializeField] private CameraSwitcher[] cameraSwitch;
    [Space]

    [Header("Zones")]
    [SerializeField] private GameObject[] _zones;
    [Space]


    [Header("Zone Disable Objs")]
    [SerializeField] private GameObject[] _marketZoneDisableObjs;
    [SerializeField] private GameObject[] _blacksmithZoneDisableObjs;
    [SerializeField] private GameObject[] _campZoneDisableObjs;

    private Camera _mainCamera;

    private OwnGlads _ownGlads;

    private int currentActiveCamera;

    private enum Location
    {
        City = 0,
        Market = 1,
        Coliseum = 2,
        School = 3,
        Blacksmith = 4
    }

    private Location _prevLoc;

    private void Start()
    {
        _zones[0].SetActive(true);
        _zones[1].SetActive(false);
        _mainCamera = Camera.main;
        if (PlayerPrefs.HasKey("League")) PlayerPrefs.DeleteKey("League");
        if (PlayerPrefs.HasKey("EnemyCount")) PlayerPrefs.DeleteKey("EnemyCount");
        if (PlayerPrefs.HasKey("ToFightGladNum")) PlayerPrefs.DeleteKey("ToFightGladNum");
        _prevLoc = Location.City;
        _mainCamera = Camera.main;
        _ownGlads = FindObjectOfType<OwnGlads>();
    }

    private IEnumerator ChangeView(Location loc)
    {
        if (loc != Location.Coliseum && _zones[0].activeInHierarchy == false) _zones[0].SetActive(true);
        switch (_prevLoc)
        {
            case Location.City:
                foreach (GameObject btn in _menuButtons) btn.SetActive(false);
                break;
            case Location.Market:
                _gladStat.SetActive(false);
                foreach (GameObject obj in _marketObjects) obj.SetActive(false);
                foreach (GameObject item in _marketZoneDisableObjs) item.SetActive(true);
                _backButton.SetActive(false);
                _marketGlads.LightUpGlad();
                break;
            case Location.Coliseum:
                foreach (GameObject btn in _coliseumObjs) btn.SetActive(false);
                _leagueMenu.SetActive(false);
                _backButton.SetActive(false);
                break;
            case Location.School:
                foreach (GameObject obj in _schoolObjs) obj.SetActive(false);
                foreach (GameObject item in _campZoneDisableObjs) item.SetActive(true);
                _backButton.SetActive(false);
                break;
            case Location.Blacksmith:
                foreach (GameObject obj in _blacksmithObjs) obj.SetActive(false);
                foreach (GameObject item in _blacksmithZoneDisableObjs) item.SetActive(true);
                _backButton.SetActive(false);
                break;
        }
        yield return null;
        yield return new WaitForSeconds(1f);
        if (_prevLoc == Location.Coliseum) _zones[1].SetActive(false);
        switch (loc)
        {
            case Location.City:
                foreach (GameObject btn in _menuButtons) btn.SetActive(true);
                break;
            case Location.Market:
                for (int i = 0; i < _marketObjects.Length; i++)
                {
                    if (_marketGlads.IsBought(i)) continue;
                    _marketObjects[i].SetActive(true);
                }
                foreach (GameObject item in _marketZoneDisableObjs) item.SetActive(false);
                _backButton.SetActive(true);
                break;
            case Location.Coliseum:
                yield return new WaitForSeconds(0.05f);
                _zones[0].SetActive(false);
                foreach (GameObject obj in _coliseumObjs) obj.SetActive(true);
                _backButton.SetActive(true);
                break;
            case Location.School:
                for (int i = 0; i < 5; i++) _schoolObjs[i].SetActive(true);
                foreach (GameObject item in _campZoneDisableObjs) item.SetActive(false);
                _backButton.SetActive(true);
                break;
            case Location.Blacksmith:
                foreach (GameObject obj in _blacksmithObjs) obj.SetActive(true);
                foreach (GameObject item in _blacksmithZoneDisableObjs) item.SetActive(false);
                _backButton.SetActive(true);
                break;
        }
        
        _prevLoc = loc;
    }

    public void ResetMarket()
    {
        _marketGlads.NewGlads();
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("loading");
    }


    public void ColiseumButton()
    {
        if (_ownGlads.GetOwnGladsCount() <= 0) return;
        int tempCount = 0;
        for (int i = 0; i < _ownGlads.GetOwnGladsCount(); i++)
        {
            if (_ownGlads.GetOwnGladiator(i).isDead) continue;
            tempCount++;
        }
        if (tempCount == 0) return;
        CameraChanger((int)Location.Coliseum);
    }
    public void SchoolButton()
    {
        if (_ownGlads.GetOwnGladsCount() <= 0) return;
        CameraChanger((int)Location.School);
    }

    public void StatsInfoButton(int statNum)
    {
        _statsInfo.SetActive(true);
        _statsObjs[statNum].SetActive(true);
    }

    public void StatsInfoDisable()
    {
        _statsInfo.SetActive(false);
        foreach (var item in _statsObjs) item.SetActive(false);
    }

    public void CameraChanger(int locNum)
    {
        if (locNum == cameraSwitch[0].locationNum) // active if back button pressed
        {
            if (_prevLoc == Location.Coliseum)
                StartCoroutine(FromChooseZone());
            else
            {
                cameraSwitch[currentActiveCamera].camera.SetActive(false);
            }
            StartCoroutine(ChangeView((Location)locNum));
        }
        else
        {
            for (int i = 1; i < cameraSwitch.Length; i++)
            {
                if (cameraSwitch[i].locationNum == locNum)
                {
                    currentActiveCamera = i;
                    //currentCameraObject = cameraSwitch[i].camera;
                    cameraSwitch[i].camera.SetActive(true);
                    break;
                }
            }
            StartCoroutine(ChangeView((Location)locNum));
        }
        if (locNum == (int)Location.Coliseum)
        {
            StartCoroutine(ToChooseZone());
        }
    }

    private IEnumerator ToChooseZone()
    {
        yield return new WaitForSeconds(1f);
        _mainCamera.fieldOfView = 40;
        cameraSwitch[0].camera.SetActive(false);
        cameraSwitch[currentActiveCamera].camera.SetActive(false);
        _mainCamera.GetComponent<Animation>().Play("cameraFromColiseumToSelect");
        yield return null;
        _zones[1].SetActive(true); 
        FindObjectOfType<ToFightGlads>().Init();
        yield return null;
        cameraSwitch[5].camera.SetActive(true);
    }


    private IEnumerator FromChooseZone()
    {
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        _mainCamera.GetComponent<Animation>().Play("cameraFromSelectToMain");
        cameraSwitch[5].camera.SetActive(false);
        cameraSwitch[6].camera.SetActive(false);
        yield return null;
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        cameraSwitch[2].camera.SetActive(true);
        yield return null;
        yield return null;
        cameraSwitch[0].camera.SetActive(true);
        cameraSwitch[2].camera.SetActive(false);
    }
}
