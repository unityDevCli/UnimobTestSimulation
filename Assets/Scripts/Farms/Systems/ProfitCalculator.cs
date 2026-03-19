using Farms.Entities;

namespace Farms.Systems
{
    public class ProfitCalculator
    {
        private readonly float _levelProfitStep;

        public ProfitCalculator(float levelProfitStep)
        {
            _levelProfitStep = levelProfitStep;
        }

        public float GetUnitPrice(Plant plant, FarmUpgradeState upgradeState)
        {
            var levelMultiplier = 1f + (plant.plantLevel - 1) * _levelProfitStep;
            var singlePlantMultiplier = upgradeState.GetPlantProfitMultiplier(plant.plantId);
            var globalMultiplier = upgradeState.globalProfitMultiplier;
            return plant.baseProfit * levelMultiplier * singlePlantMultiplier * globalMultiplier;
        }
    }
}