using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NotifyData;

public class PlayerUI : MonoBehaviour, IObserver
{
    [SerializeField] private PlayerShuttle _player;
    [SerializeField] private TextMeshProUGUI _fuelText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameObject _endScreen;

    private int _seconds;

    private readonly WaitForSeconds waitSecond = new WaitForSeconds(1f);

    public void OnNotify(Data data)
    {
        switch (data.Action)
        {
            case Action.Fuel:
                _fuelText.text = data.Value.ToString();
                break;
            case Action.Score:
                _scoreText.text = data.Value.ToString();
                break;
            case Action.EndGame:
                StopAllCoroutines();
                ShowEndScreen();
                break;
        }
    }

    private void OnEnable()
    {
        _player.AddObserver(this);
        _seconds = 0;
        //_timeText.text = "00:00";
        StartCoroutine(CountTime());
    }

    private IEnumerator CountTime()
    {
        while (_player != null)
        {
            int seconds = _seconds % 60;
            int minutes = _seconds / 60;
            _timeText.text = ((minutes < 10) ? $"0{minutes}" : minutes.ToString()) + ":" + ((seconds < 10) ? $"0{seconds}" : seconds.ToString());
            yield return waitSecond;
            _seconds++;
        }
    }

    private void ShowEndScreen()
    {
        _endScreen.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        _player?.RemoveObserver(this);
    }



}
