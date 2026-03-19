namespace Farms.Entities
{
    public class Plant
    {
        public int plantId;
        public int plantLevel;
        public float baseProfit;
        public float growthTime;
        public int storedFruited;
        public bool isReadyToHarvest;
        public float growTime;

        public void Tick(float deltaTime)
        {
            if (isReadyToHarvest) return;
            growTime += deltaTime;
            if (growTime >= growthTime)
            {
                growTime = 0;
                storedFruited = 1;
                isReadyToHarvest = true;
            }
        }
    }
}