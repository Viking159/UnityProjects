using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Wood
{
    private void OnEnable()
    {
        WoodType = WoodType.Log;
        Init();
    }

}
