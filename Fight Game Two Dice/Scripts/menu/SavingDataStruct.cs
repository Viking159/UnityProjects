using System.Collections.Generic;

namespace Structs
{
    [System.Serializable]
    public struct MarketGladsStruct
    {
        public List<MarketGladiator> glads;
    }

    [System.Serializable]
    public struct MarketGladiator
    {
        public string name;
        public string[] elements;
        public int[] stats;
        public int cost;
        public bool bought;

        public void BuyMarketGladiator()
        {
            bought = true;
        }
    }

    [System.Serializable]
    public struct OwnedGladsStruct
    {
        public List<OwnGladiator> glads;
    }

    [System.Serializable]
    public struct OwnGladiator
    {
        public string name;
        public string[] elements;
        public int[] baseStats;
        public int[] stats;
        public int[] experience;
        public byte injuries;
        public bool isDead;
        public int deathTime;
        public int glory;
        public float energy;

        public void AddStat(int statNum)
        {
            stats[statNum]++;
        }

        public void SubtractStat(int statNum)
        {
            stats[statNum]--;
        }

        public void SetStats(int[] statsP)
        {
            for (int i = 0; i < statsP.Length; i++)
                stats[i] = statsP[i];
        }

        public void ChangeExp(int value, bool full = true)
        {
            experience[1] += value;
            if (full) experience[0] += value;
        }

        public void DeathReviveGlad(bool reverse = false)
        {
            if (reverse == true) { ++injuries; isDead = false; }
            else { isDead = true; }
        }
    }

    [System.Serializable]
    public struct OwnGladArenaSuccess
    {
        public sbyte percent;
        public bool isFirstStepAvailable;
        public bool isSecondStepAvailable;
        public bool isThirdStepAvailable;
        public bool[] beatenEnemies; // true - beaten, false - not beaten
    }

    [System.Serializable]
    public struct SchoolProgress
    {
        public int currExperience;
        public int levelExperience;
        public int level;
    }

    [System.Serializable]
    public struct LeagueEnemyGlads
    {
        public List<EnemyGladPrefab> glads;
    }

    [System.Serializable]
    public struct EnemyGladPrefab
    {
        public string[] elements;
        public int[] stats;
    }

    [System.Serializable]
    public struct NameIndexes
    {
        public List<int> indexes;
    }

    [System.Serializable]
    public struct SchoolUpgradeInfo
    {
        public List<int> stats;
    }

}

