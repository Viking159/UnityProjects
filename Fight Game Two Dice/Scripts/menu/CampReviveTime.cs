using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public class CampReviveTime : MonoBehaviour
{

    private TimeController _timeController;
    private OwnGlads _ownGlads;
    private School _school;

    private const int updateTime = 86400;

    private void Start()
    {
        _ownGlads = FindObjectOfType<OwnGlads>();
        _school = FindObjectOfType<School>();
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnTick += CheckReviveTime;
    }

    private void CheckReviveTime(int time)
    {
        for (int i = 0; i < _ownGlads.GetOwnGladsCount(); i++)
        {
            if (_ownGlads.GetOwnGladiator(i).isDead)
            {
                if (time - _ownGlads.GetOwnGladiator(i).deathTime >= updateTime)
                {
                    _ownGlads.RemoveGlad(i);
                    _school = FindObjectOfType<School>();
                    if (_school != null) _school.ShowNewGlad();
                    break;
                }
            }
        }
            
    }

    public float GetTimeLeft(int time, int i)
    {
        float timeLeft = 1 - ((time - _ownGlads.GetOwnGladiator(i).deathTime) / (float)updateTime);
        if (time - _ownGlads.GetOwnGladiator(i).deathTime >= updateTime)
        {
            CheckReviveTime(time);
            return 1;
        }
        return timeLeft;
    }

    public OwnGladiator GetGlad(int num) => _ownGlads.GetOwnGladiator(num);
}
