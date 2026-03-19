using Farms.Entities;
using Farms.ScriptableObjects;

namespace Farms.Systems
{
    public class BuildSystem
    {
        public Plant Build(ConstructionBox box, PlantConfigSo config)
        {
            if (box.isBuilt) return null;
            var plant = new Plant()
            {
                plantId = box.constructionId,
                plantLevel = 1,
                baseProfit = config.baseProfit,
                growthTime = config.growthTime,
                storedFruited = 0,
                isReadyToHarvest = false,
                growTime = 0f
            };
            box.isBuilt = true;
            box.currentPlant = plant;
            return plant;
        }
    }
}