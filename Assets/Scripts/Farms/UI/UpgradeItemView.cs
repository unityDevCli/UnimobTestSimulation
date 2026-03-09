using System;
using Farms.ScriptableObjects;
using Farms.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class UpgradeItemView : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtItemName;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtDes;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtPrice;

        [BoxGroup("References")] [SerializeField]
        protected Image imgIcon;

        [BoxGroup("References")] [SerializeField]
        protected Button btnUpgrade;

        private UpgradeData _data;
        private Action<UpgradeItemView, UpgradeData> _onUpgradeClicked;

        public void SetupView(UpgradeData data, Action<UpgradeItemView, UpgradeData> onUpgradeClicked)
        {
            _data = data;
            _onUpgradeClicked = onUpgradeClicked;

            txtItemName.text = data.upgradeName;
            txtDes.text = data.type == UpgradeType.AddCustomer
                ? $"+{data.addCount} Customer"
                : $"x{data.multiplier}" + (data.type == UpgradeType.GlobalProfit
                    ? " Global Profit"
                    : $" Profit to Construction {data.constructionIndex}");
            txtPrice.text = data.price.ToString("0").ToKMBTA();
            if (imgIcon != null && data.icon != null)
            {
                imgIcon.sprite = data.icon;
            }

            btnUpgrade.onClick.RemoveAllListeners();
            btnUpgrade.onClick.AddListener(OnUpgradeButtonClicked);
        }

        private void OnUpgradeButtonClicked()
        {
            _onUpgradeClicked?.Invoke(this, _data);
        }
    }
}