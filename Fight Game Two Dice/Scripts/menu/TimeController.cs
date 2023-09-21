using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public struct ServerTimeResponse
{
    public int unixtime;
}

public class TimeController : MonoBehaviour
{
    private int _servetStartTime;
    private int _servetCurrTime;

    private const string curl = "http://worldtimeapi.org/api/timezone/Europe/Moscow";

    private bool _countTime;

    public Action<int> OnTick;
    public Action<int> OnTimeStarted;

    public bool IsLocalTime = false;

    private const string PPKey = "LastTime";
    public static readonly int TickTime = 30;
    public static int LastTime;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PPKey)) LastTime = int.Parse(PlayerPrefs.GetString(PPKey));
        else LastTime = -1;
        IsLocalTime = false;
        _servetStartTime = 0;
        _servetCurrTime = 0;
        StartCoroutine(SendTimeRequest());
        StartCoroutine(TimeCount());
    }

    private IEnumerator SendTimeRequest()
    {
        _countTime = true;
        UnityWebRequest request = UnityWebRequest.Get(curl);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            IsLocalTime = true;
            if (PlayerPrefs.HasKey(PPKey))
            {
                _servetStartTime = int.Parse(PlayerPrefs.GetString(PPKey));
            }
            else
            {
                DateTime now = DateTime.Now;
                _servetStartTime = (int)((DateTimeOffset)now).ToUnixTimeSeconds();
            }
            _servetCurrTime = _servetStartTime;
            yield break;
        }
        IsLocalTime = false;
        string json = request.downloadHandler.text;
        ServerTimeResponse time = JsonUtility.FromJson<ServerTimeResponse>(json);
        _servetStartTime = time.unixtime;
        _servetCurrTime = time.unixtime;
        OnTimeStarted?.Invoke(_servetStartTime);
    }

    private IEnumerator TimeCount()
    {
        print("time started!"); 
        while (_servetCurrTime == 0)
            yield return null;
        while (_countTime)
        {
            yield return new WaitForSeconds(TickTime);
            _servetCurrTime += TickTime;
            OnTick?.Invoke(_servetCurrTime);
        }
    }

    public int GetStartTime()
    {
        return _servetStartTime;
    }

    public int GetCurrTime()
    {
        return _servetCurrTime;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (_countTime) return;
            StartCoroutine(SendTimeRequest());
            StartCoroutine(TimeCount());
        }
        else
        {
            _countTime = false;
            StopCoroutine(TimeCount());
            PlayerPrefs.SetString(PPKey, _servetCurrTime.ToString());
        }
    }

    private void OnDestroy()
    {
        _countTime = false;
        StopCoroutine(TimeCount());
        PlayerPrefs.SetString(PPKey, _servetCurrTime.ToString());
    }
}
