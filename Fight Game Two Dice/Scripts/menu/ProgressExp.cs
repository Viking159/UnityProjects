using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public class ProgressExp : MonoBehaviour
{
    public static int ExpCount { get; private set; }
    public static int Level { get; private set; }
    public static int MaxExpCount { get; private set; }

    private static VisualMenu _visualMenu;

    private const string PPKey = "SchoolProgress";

    private void Awake()
    {
        _visualMenu = FindObjectOfType<VisualMenu>();
        if (PlayerPrefs.HasKey(PPKey)) LoadProgress();
        else
        {
            ExpCount = 0;
            Level = 1;
            MaxExpCount = 1000;
            SaveProgress();
        }
        UpadteVisual();
    }

    public static void AddExp(int value)
    {
        if (value < 0) { Debug.LogError("Add exp value < 0!"); return; }
        ExpCount += value;
        while (ExpCount >= MaxExpCount)
        {
            ExpCount -= MaxExpCount;
            Level++;
            MaxExpCount += 1000;
            UpadteVisual();
        }
        UpadteVisual();
        SaveProgress();
    }

    private static void UpadteVisual()
    {
        if (_visualMenu != null) _visualMenu.UpdateExpText();
        else Debug.LogError("Meoney.VisualMenu == null!");
    }

    private static void SaveProgress()
    {
        string json = JsonUtility.ToJson(new SchoolProgress { currExperience = ExpCount, level = Level, levelExperience = MaxExpCount });
        PlayerPrefs.SetString(PPKey, json);
    }

    private void LoadProgress()
    {
        string json = PlayerPrefs.GetString(PPKey);
        SchoolProgress temp = JsonUtility.FromJson<SchoolProgress>(json);
        ExpCount = temp.currExperience;
        Level = temp.level;
        MaxExpCount = temp.levelExperience;
    }

    private void OnApplicationPause(bool pause)
    {
        SaveProgress();
    }

    private void OnDestroy()
    {
        SaveProgress();
    }

}
