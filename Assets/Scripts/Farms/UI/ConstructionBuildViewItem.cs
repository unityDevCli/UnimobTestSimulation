using Farms.ScriptableObjects;
using Farms.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class ConstructionBuildViewItem : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtConstructionName;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtPrice;

        [BoxGroup("References")] [SerializeField]
        protected Image imgConstruction;

        public void SetupView(ConstructionData constructionData)
        {
            txtConstructionName.text = constructionData.constructionName;
            txtPrice.text = StringUtils.ConvertMoneyAndAddText(constructionData.price);
            imgConstruction.sprite = constructionData.constructionIcon;
        }
    }
}