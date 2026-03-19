using System.Collections.Generic;

namespace Farms.Entities
{
    public class FarmUpgradeState
    {
        public float globalProfitMultiplier = 1f;
        public int extraCustomerCount = 0;

        private readonly Dictionary<int, float> _plantProfitMultipliers = new Dictionary<int, float>();

        public float GetPlantProfitMultiplier(int plantId)
        {
            return _plantProfitMultipliers.GetValueOrDefault(plantId, 1f);
        }

        public void MultiplyPlantProfit(int plantId, float multiplier)
        {
            _plantProfitMultipliers.TryAdd(plantId, 1f);
            _plantProfitMultipliers[plantId] *= multiplier;
        }

        public void MultiplyAllProfit(float multiplier)
        {
            globalProfitMultiplier *= multiplier;
        }

        public void AddCustomers(int count) => extraCustomerCount += count;
    }
}