using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public enum StatNames
{
    BarraksSizeLvl,
    ComfortLvl,
    TrainingDummyLvl,
    TrainingWeaponsLvl,
    TrainingShieldsLvl,
    HerkulesLvl,
    MerkuryLvl,
    CharitesLvl,
    MarsLvl,
    MinervaLvl,
    KitchenLvl,
    WellLvl,
    LaundryLvl,
    HospitalLvl
};

public enum StatLevelValueDependence
{
    Linear,
    X10
}

public enum MeasureUnit
{
    Unit,
    Percent
}

public class SchoolUpgradeDataSaver : MonoBehaviour
{
    private const string PPKey = "SchoolUpgradeInfo";

    private static SchoolUpgradeInfo _schoolUpgradeInfo;

    private void Awake()
    {
        LoadData();
    }

    private static void SaveData()
    {
        PlayerPrefs.SetString(PPKey, JsonUtility.ToJson(_schoolUpgradeInfo));
    }

    private static void LoadData()
    {
        if (PlayerPrefs.HasKey(PPKey))
        {
            _schoolUpgradeInfo = JsonUtility.FromJson<SchoolUpgradeInfo>(PlayerPrefs.GetString(PPKey));
            return;
        }
        _schoolUpgradeInfo = new SchoolUpgradeInfo
        {
            stats = new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
        SaveData();
    }

    public static int GetLevelData(int statNum) => _schoolUpgradeInfo.stats[statNum];

    public static void UpgradeStat(int statNum, int level)
    {
        _schoolUpgradeInfo.stats[statNum] = level;
        SaveData();
    }
}
