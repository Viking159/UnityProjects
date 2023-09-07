using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : Wood
{
    private void Start()
    {
        WoodType = WoodType.Branch;
        Init();
    }
}
