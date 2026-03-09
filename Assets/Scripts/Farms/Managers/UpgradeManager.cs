using System.Collections.Generic;
using Farms.ScriptableObjects;
using Farms.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class UpgradeManager : MonoBehaviour
    {

        [BoxGroup("Data")] [SerializeField] protected List<UpgradeData> availableUpgrades;

        [BoxGroup("References")] [SerializeField]
        protected FarmSimulationManager farmManager;

        [Button]
        public void OpenUpgradeView()
        {
            Debug.LogError($"Opening upgrade view: {farmManager.UIManager.UpgradeView == null}");
            if (farmManager.UIManager.UpgradeView != null)
            {
                Debug.LogError("Opening upgrade view");
                farmManager.UIManager.UpgradeView.Setup(availableUpgrades, OnUpgradeClicked);
                farmManager.UIManager.UpgradeView.Show();
            }
        }

        private void OnUpgradeClicked(UpgradeItemView itemView, UpgradeData data)
        {  
            if (farmManager == null) return;
            Debug.LogError($"Opening upgrade view1: {farmManager.UIManager.UpgradeView == null}");

            ApplyUpgrade(data);

            availableUpgrades.Remove(data);

            farmManager.UIManager.UpgradeView.Setup(availableUpgrades, OnUpgradeClicked);
        }

        private void ApplyUpgrade(UpgradeData data)
        {
            switch (data.type)
            {
                case UpgradeType.GlobalProfit:
                    farmManager.AddGlobalProfit(data.multiplier - 1f);
                    break;
                case UpgradeType.ConstructionProfit:
                    farmManager.AddLocalProfit(data.constructionIndex, data.multiplier - 1f);
                    break;
                case UpgradeType.AddCustomer:
                    farmManager.AddTargetCustomer(data.addCount);
                    break;
            }
        }
    }
}