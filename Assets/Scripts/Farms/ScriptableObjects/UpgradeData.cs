using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.ScriptableObjects
{
    public enum UpgradeType
    {
        GlobalProfit,
        ConstructionProfit,
        AddCustomer
    }

    [CreateAssetMenu(fileName = "UpgradeData", menuName = "_Farm/UpgradeData", order = 1)]
    public class UpgradeData : ScriptableObject
    {
        [PreviewField] public Sprite icon;
        public string upgradeName;
        public UpgradeType type;
        public int constructionIndex; // Chỉ dùng cho ConstructionProfit
        public float multiplier; // Dùng cho profit (x2, x3)
        public int addCount; // Dùng cho AddCustomer
        public double price;
    }
}