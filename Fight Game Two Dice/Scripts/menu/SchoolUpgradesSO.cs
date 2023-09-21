using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "School Upgrade Item", menuName  = "School upgrade", order = 1)]
public class SchoolUpgradesSO : ScriptableObject
{
    [SerializeField] private StatNames _statName;
    public int StatNum => (int)_statName;

    [SerializeField] private StatLevelValueDependence _dependence;

    [SerializeField] private MeasureUnit _measureUnit;
    public MeasureUnit MeasureUnit => _measureUnit;

    [SerializeField] private string _title;
    public string Title => _title;

    [TextArea]
    [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private int _minLevel;

    [SerializeField] private int _maxLevel;
    public int MaxLevel => _maxLevel;

    [SerializeField] private int _basePrice;

    public int CalcPrice(int level) => (int)(_basePrice * Mathf.Pow(1.07f, (level- _minLevel)));

    public int CalcValue(int level) => (_dependence == StatLevelValueDependence.Linear) ? level : 10 * level; 
}
