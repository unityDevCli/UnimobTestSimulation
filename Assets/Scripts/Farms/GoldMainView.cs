using Farms.Utilities;
using TMPro;
using UnityEngine;

namespace Farms
{
    public class GoldMainView : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txtGold;
        public void UpdateGold(float cash)
        {
            txtGold.text = StringUtils.ConvertMoneyAndAddText((long)cash);
        }
    }
}