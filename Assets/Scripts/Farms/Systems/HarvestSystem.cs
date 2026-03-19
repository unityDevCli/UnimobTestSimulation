using Farms.Entities;

namespace Farms.Systems
{
    public class HarvestSystem
    {
        private readonly ProfitCalculator _profitCalculator;

        public HarvestSystem(ProfitCalculator profitCalculator)
        {
            _profitCalculator = profitCalculator;
        }

        public bool CanHarvest(Plant plant)
        {
            return plant is { isReadyToHarvest: true, storedFruited: > 0 };
        }

        public HarvestBundle Harvest(Plant plant, FarmUpgradeState upgradeState)
        {
            if (!CanHarvest(plant)) return null;
            var bundle = new HarvestBundle()
            {
                plantId = plant.plantId,
                fruitAmount = plant.storedFruited,
                unitPriceSnapshot = _profitCalculator.GetUnitPrice(plant, upgradeState)
            };
            plant.storedFruited = 0;
            plant.isReadyToHarvest = false;
            return bundle;
        }
    }
}