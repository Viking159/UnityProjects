using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "Upgrade", menuName = "ScriptableObjects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public Actions Action;
    public Sprite Icon;
    public string Name;
    public int Level;
    public float Value;
    public float AddValue;
    public float Price;
}
