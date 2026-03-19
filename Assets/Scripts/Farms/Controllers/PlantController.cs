using Farms.Managers;

namespace Farms.Controllers
{
    public class PlantController
    {
        private readonly GameManager _gameManager;

        public PlantController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void UpgradePlant(int plantId)
        {
            var plant = _gameManager.RunTime.plants.Find(x => x.plantId == plantId);
            if (plant == null) return;
            _gameManager.UpgradeSystem.UpgradePlantLevel(plant);
        }

        public void UpgradeSinglePlantProfit(int plantId, float multiplier)
        {
            _gameManager.UpgradeSystem.UpgradeSinglePlantProfit(_gameManager.RunTime.upgradeState, plantId, multiplier);
        }

        public void UpgradeAllProfit(float multiplier)
        {
            _gameManager.UpgradeSystem.UpgradeAllPlantProfit(_gameManager.RunTime.upgradeState, multiplier);
        }
    }
}