using Farms.Managers;
using Farms.ScriptableObjects;
using Farms.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Farms
{
    public class Plot
    {
        public int index;
        public ConstructionData constructionData;
        public GameObject boxObject;
        public Animation boxAnimation;
        public BuildCountdownView countdownView;
        public GameObject constructionObject;
        public ConstructionView constructionView;
        public InformationView informationView;
        public PlotState state = PlotState.Locked;
        public bool built;
        public bool isUnlocking;
        public float unlockTimer;
        public int level = 1;
        public int readyFruits;
        public float growTimer;
        public float localProfitMultiplier = 1f;
        public List<GameObject> spawnedFruits = new List<GameObject>();
        public FarmSimulationManager farmManager;

        public float LevelProfitMultiplier
        {
            get
            {
                if (level <= 1) return 1f;
                if (level == 2) return 1.1f;
                return 1.2f;
            }
        }

        public float GetUpgradePrice()
        {
            return farmManager.BaseUpgradePrice * level;
        }

        public float GetCurrentProfit()
        {
            float baseProfit = farmManager.BaseFruitPrice;
            return baseProfit * LevelProfitMultiplier * localProfitMultiplier;
        }

        public float GetNextProfit()
        {
            float baseProfit = farmManager.BaseFruitPrice;
            float nextLevelMultiplier = 1.2f;
            if (level == 1) nextLevelMultiplier = 1.1f;
            else if (level >= 10) return GetCurrentProfit();

            return baseProfit * nextLevelMultiplier * localProfitMultiplier;
        }

        public float GetCurrentGrowthTime()
        {
            return farmManager.GrowthSeconds / (1f + (level - 1) * 0.1f);
        }

        public float GetNextGrowthTime()
        {
            if (level >= 10) return GetCurrentGrowthTime();
            return farmManager.GrowthSeconds / (1f + level * 0.1f);
        }

        public float TotalProfitMultiplier(float global)
        {
            return LevelProfitMultiplier * localProfitMultiplier * global;
        }
    }
}