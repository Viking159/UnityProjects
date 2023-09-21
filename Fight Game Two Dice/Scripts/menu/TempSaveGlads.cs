using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public class TempSaveGlads : MonoBehaviour
{
    [Header("Generated count")]
    [SerializeField] private int _generatedCount = 0;
    [SerializeField] private int[] stats;


    private List<EnemyGladPrefab> _glads;
    private EnemyGladPrefab _tempGlad;
    private ModulesShaker _ms;

    private string PPKey = "League3EnemyGlads";

    private void Start()
    {
        _glads = new List<EnemyGladPrefab>();
        _generatedCount = _glads.Count;
        _ms = GetComponent<ModulesShaker>();
    }

    public void SaveGladElements()
    {
        if (_generatedCount == 7)
        {
            print("All already saved!");
            return;
        }
        _generatedCount++;
        _tempGlad = new EnemyGladPrefab();
        _tempGlad.elements = _ms.GetElements().Split(' ');
        _tempGlad.stats = new int[4] { stats[0], stats[1], stats[2], stats[3] };
        _glads.Add(_tempGlad);
        print("Saved: " + _generatedCount.ToString());
        if (_generatedCount == 7)
        {
            SaveAll();
        }
    }

    public void RandomizeElements()
    {
        _ms.RandomizeAll();
    }

    private void SaveAll()
    {
        string json = JsonUtility.ToJson(new LeagueEnemyGlads { glads = _glads });
        PlayerPrefs.SetString(PPKey, json);
        print("Saved to PP:");
        print(json);
    }
}
