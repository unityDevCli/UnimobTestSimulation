using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class InformationView : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txtProductionName;
        [SerializeField] private TMP_Text txtProductionRate;
        [SerializeField] private Image imgIcon;

        public void UpdateProduction(string productionName, int hourlyRate, Sprite icon)
        {
            if (txtProductionName != null)
            {
                txtProductionName.text = productionName;
            }

            if (txtProductionRate != null)
            {
                txtProductionRate.text = hourlyRate.ToString() + "/h";
            }

            if (imgIcon != null)
            {
                imgIcon.sprite = icon;
            }
        }
    }
}