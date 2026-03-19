using Farms.Entities;

namespace Farms.Systems
{
    public class DeliverySystem
    {
        public float Sell(HarvestBundle harvestBundle)
        {
            if (harvestBundle is not { fruitAmount: > 0 }) return 0;
            return harvestBundle.fruitAmount * harvestBundle.unitPriceSnapshot;
        }
    }
}