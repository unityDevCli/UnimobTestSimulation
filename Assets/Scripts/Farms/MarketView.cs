using UnityEngine;

namespace Farms
{
    public class MarketView : MonoBehaviour
    {
        [Header("Customer Points")]
        public Transform customerStart;
        public Transform customerEnd;
        
        [Header("Dock Points")]
        public Transform[] currency;
        public Transform[] delivery;
        public Transform deliveryEnd;
    }
}
