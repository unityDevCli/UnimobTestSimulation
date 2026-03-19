using UnityEngine;

namespace Farms.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlantConfig", menuName = "Farms/PlantConfig", order = 0)]
    public class PlantConfigSo : ScriptableObject
    {
        public float baseProfit = 10f;
        public float growthTime = 5f;
    }
}