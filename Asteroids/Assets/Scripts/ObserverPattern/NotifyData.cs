using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotifyData
{
    public enum Action
    {
        Fuel,
        Score,
        EndGame
    }

    public struct Data
    {
        public Action Action;
        public float Value;

        public Data(Action action, float value)
        {
            Action = action;
            Value = value;
        }

    }


}
