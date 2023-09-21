using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public class OwnGlads : MonoBehaviour
{
    private TimeController _time;

    private List<OwnGladiator> _ownGlads;
    private int _ownGladsCount;

    private readonly string PPKey = "OwnGlads";

    private void Awake()
    {
        if (LoadFromRR() == false)
        {
            _ownGlads = new List<OwnGladiator>();
            _ownGladsCount = 0;
        }
        _time = FindObjectOfType<TimeController>();
        _time.OnTick += UpdateEnergy;
        _time.OnTimeStarted += UpdateLastEnergy;
    }

    private void UpdateLastEnergy(int unixtime)
    {
        int passedTime = unixtime - TimeController.LastTime;
        for(int i = 0; i < _ownGlads.Count; i++)
        {
            if (_ownGlads[i].stats[1] * 5 == _ownGlads[i].energy) continue;
            float speed = _ownGlads[i].stats[1] * 5f / 28800f; //speed of regeneration energy per sec; 28800sec = 8h
            float newEnergy = (_ownGlads[i].energy + speed * passedTime);
            newEnergy = (newEnergy > _ownGlads[i].stats[1] * 5) ? _ownGlads[i].stats[1] * 5 : newEnergy;
            //print($"!!! gladNum: {i}; energy was: {_ownGlads[i].energy}; energy now: {newEnergy}; delta energy:{newEnergy - _ownGlads[i].energy}; delta time{passedTime}");
            ChangeEnergy(i, newEnergy);
        }
    }
    private void UpdateEnergy(int unixtime)
    {
        for (int i = 0; i < _ownGlads.Count; i++)
        {
            if (_ownGlads[i].stats[1] * 5 == _ownGlads[i].energy) continue;
            float speed = _ownGlads[i].stats[1] * 5f / 28800f; //speed of regeneration energy per sec; 28800sec = 8h
            float newEnergy = (_ownGlads[i].energy + speed * TimeController.TickTime);
            newEnergy = (newEnergy > _ownGlads[i].stats[1] * 5f) ? _ownGlads[i].stats[1] * 5f : newEnergy;
            //print($"gladNum: {i}; energy was: {_ownGlads[i].energy}; energy now: {newEnergy}; delta energy:{newEnergy - _ownGlads[i].energy}; delta time{TimeController.TickTime}");
            ChangeEnergy(i, newEnergy);
        }
    }

    private void SaveToPP()
    {
        string json = JsonUtility.ToJson(new OwnedGladsStruct { glads = _ownGlads });
        PlayerPrefs.SetString(PPKey, json);
    }

    private bool LoadFromRR()
    {
        if (PlayerPrefs.HasKey(PPKey) == false) return false;

        string json = PlayerPrefs.GetString(PPKey);
        _ownGlads = JsonUtility.FromJson<OwnedGladsStruct>(json).glads;
        _ownGladsCount = _ownGlads.Count;
        return true;
    }

    public void UpdateColor(int gladNum, int colorNum)
    {
        _ownGlads[gladNum].elements[0] = colorNum.ToString();
        SaveToPP();
    }

    public void AddGlad(MarketGladiator glad)
    {
        OwnGladiator temp = new OwnGladiator
        {
            name = glad.name,
            elements = glad.elements,
            baseStats = glad.stats,
            stats = glad.stats,
#if UNITY_EDITOR
            experience = new int[2] { 5000, 5000 },
#else
            experience = new int[2] { 0, 0 },
#endif
            injuries = 0,
            isDead = false,
            deathTime = 0,
            glory = 0,
            energy = glad.stats[1]*5
        };
        _ownGlads.Add(temp);
        _ownGladsCount++;
        SaveToPP();
    }

    public void RemoveGlad(int gladNum)
    {
        _ownGlads.RemoveAt(gladNum);
        _ownGladsCount--;
        SaveToPP();
    }

    public void UpdateStats(int gladNum, int[] statsP, int exp)
    {
        _ownGlads[gladNum].SetStats(statsP);
        _ownGlads[gladNum].ChangeExp(exp, false);
        SaveToPP();
    }

    public void UpdateGladsLife(int gladNum, bool lifeValue)
    {
        OwnGladiator glad = new OwnGladiator
        {
            name = _ownGlads[gladNum].name,
            elements = _ownGlads[gladNum].elements,
            baseStats = _ownGlads[gladNum].baseStats,
            stats = _ownGlads[gladNum].stats,
            experience = _ownGlads[gladNum].experience,
            injuries = (byte)(_ownGlads[gladNum].injuries + 1),
            isDead = false,
            deathTime = 0,
            glory = _ownGlads[gladNum].glory,
            energy = _ownGlads[gladNum].energy
        };
        _ownGlads[gladNum] = glad;
        SaveToPP();
    }

    public void AddGloryToGlad(int num, int value)
    {
        OwnGladiator glad = new OwnGladiator
        {
            name = _ownGlads[num].name,
            elements = _ownGlads[num].elements,
            baseStats = _ownGlads[num].baseStats,
            stats = _ownGlads[num].stats,
            experience = _ownGlads[num].experience,
            injuries = _ownGlads[num].injuries,
            isDead = _ownGlads[num].isDead,
            deathTime = _ownGlads[num].deathTime,
            glory = _ownGlads[num].glory + value,
            energy = _ownGlads[num].energy
        };
        _ownGlads[num] = glad;
        SaveToPP();
    }

    public void ChangeEnergy(int num, int movesCount)
    {
        float resEnergy = ((_ownGlads[num].energy - movesCount) <= 0) ? 0 : _ownGlads[num].energy - movesCount;
        OwnGladiator glad = new OwnGladiator
        {
            name = _ownGlads[num].name,
            elements = _ownGlads[num].elements,
            baseStats = _ownGlads[num].baseStats,
            stats = _ownGlads[num].stats,
            experience = _ownGlads[num].experience,
            injuries = _ownGlads[num].injuries,
            isDead = _ownGlads[num].isDead,
            deathTime = _ownGlads[num].deathTime,
            glory = _ownGlads[num].glory,
            energy = resEnergy
        };
        _ownGlads[num] = glad;
        SaveToPP();
    }

    public void ChangeEnergy(int num, float newEnergy)
    {
        OwnGladiator glad = new OwnGladiator
        {
            name = _ownGlads[num].name,
            elements = _ownGlads[num].elements,
            baseStats = _ownGlads[num].baseStats,
            stats = _ownGlads[num].stats,
            experience = _ownGlads[num].experience,
            injuries = _ownGlads[num].injuries,
            isDead = _ownGlads[num].isDead,
            deathTime = _ownGlads[num].deathTime,
            glory = _ownGlads[num].glory,
            energy = newEnergy
        };
        _ownGlads[num] = glad;
        SaveToPP();
    }

    public void ResetEnergy(int num)
    {
        ChangeEnergy(num, _ownGlads[num].stats[1] * 5f);
    }

    public void AddExp(int num, int value, bool full = true)
    {
        _ownGlads[num].ChangeExp(value, full);
        SaveToPP();
    }

    public OwnGladiator GetOwnGladiator(int num) => _ownGlads[num];

    public string[] GetElements(int num) => _ownGlads[num].elements;

    public int[] GetStats(int num) => _ownGlads[num].stats;

    public int GetFullExp(int num) => _ownGlads[num].experience[0];

    public int GetCurrExp(int num) => _ownGlads[num].experience[1];

    public int[] GetBaseStats(int num) => _ownGlads[num].baseStats;

    public int GetOwnGladsCount() => _ownGladsCount;

    public string GetPPKey() => PPKey;
}
