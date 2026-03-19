using Farms.Entities;

namespace Farms.Systems
{
    public class UpgradeSystem
    {
        public void UpgradePlantLevel(Plant plant)
        {
            plant.plantLevel++;
        }

        public void UpgradeSinglePlantProfit(FarmUpgradeState upgradeState, int plantId, float multiplier)
        {
            upgradeState.MultiplyPlantProfit(plantId, multiplier);
        }

        public void UpgradeAllPlantProfit(FarmUpgradeState upgradeState, float multiplier)
        {
            upgradeState.MultiplyAllProfit(multiplier);
        }

        public void AddCustomers(FarmUpgradeState upgradeState, int customerId)
        {
            upgradeState.AddCustomers(customerId);
        }
    }
}