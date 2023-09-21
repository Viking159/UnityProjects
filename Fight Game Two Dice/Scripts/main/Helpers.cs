using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public enum Combination
    {
        NONE = -1,
        PAIR,
        TWO_PAIRS,
        THREE_OF_A_KIND,
        FOUR_OF_A_KIND,
        FIVE_OF_A_KIND,
        FULL_HOUSE
    }

    public enum EAction
    {
        NONE = -1,
        ATTACK,
        BLOCK,
        DODGE,
        REST
    }
}
