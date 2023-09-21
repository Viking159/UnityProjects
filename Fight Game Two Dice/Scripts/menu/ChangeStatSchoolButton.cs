using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStatSchoolButton : MonoBehaviour
{
    [SerializeField] StatNames _statName;
    [SerializeField] bool _add;
    [SerializeField] int _max;

    private SchoolUpgrade _schoolUpgrade;

    private void OnEnable()
    {
        _schoolUpgrade = FindObjectOfType<SchoolUpgrade>();
    }

    public void OnClick()
    {
        _schoolUpgrade.ChangeStat(_statName, _add, _max);
    }

}
