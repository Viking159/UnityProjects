using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaFightSettings : MonoBehaviour
{
    public static sbyte FightType { get; private set; }
    public static byte EnemyCount { get; private set; }
    public static byte SpecialEnemyNum { get; private set; }
    public static byte?  League { get; private set; }
    
    private void Awake()
    {
        FightType = (sbyte)PlayerPrefs.GetInt("FightType");
        if (FightType == -2)
            League = null;
        else
            League = (byte)PlayerPrefs.GetInt("League");
        switch (FightType)
        {
            case -2://training
                SpecialEnemyNum = 255;
                EnemyCount = 3;
                break;
            case 0:
                SpecialEnemyNum = 255;
                EnemyCount = (byte)PlayerPrefs.GetInt("EnemyCount"); //against enemies
                break;
            case 1:
                EnemyCount = 1;
                SpecialEnemyNum = (byte)PlayerPrefs.GetInt("SpecialEnemyGladNum"); // against spec enemy
                break;
            default:
                Debug.LogError("ArenaFightSettings FightType Error");
                break;
        }
        if (PlayerPrefs.HasKey("SpecialEnemyGladNum")) PlayerPrefs.DeleteKey("SpecialEnemyGladNum");
        if (PlayerPrefs.HasKey("EnemyCount")) PlayerPrefs.DeleteKey("EnemyCount");
        if (PlayerPrefs.HasKey("League"))  PlayerPrefs.DeleteKey("League");
        PlayerPrefs.DeleteKey("FightType");
    }


}
