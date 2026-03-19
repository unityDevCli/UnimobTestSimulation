using System;

namespace Farms.Core
{
    public static class GameEvents
    {
        public static Action<float> OnGoldChanged;
        public static Action<int> OnPlantUpgraded;
        public static Action<int> OnPlantBuild;
        public static Action OnCustomerQueueChanged;
    }
}