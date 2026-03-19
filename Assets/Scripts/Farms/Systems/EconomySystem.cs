using Farms.Core;

namespace Farms.Systems
{
    public class EconomySystem
    {
        public float Gold { get; private set; }

        public void AddGold(float amount)
        {
            Gold += amount;
            GameEvents.OnGoldChanged?.Invoke(Gold);
        }

        public bool Spend(float amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            return true;
        }
    }
}