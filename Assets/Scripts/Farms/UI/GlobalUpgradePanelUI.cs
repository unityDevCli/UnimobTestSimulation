using Farms.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class GlobalUpgradePanelUI : MonoBehaviour
    {
        [SerializeField] private Button allPlantProfitButton;
        [SerializeField] private Button addCustomerButton;

        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            allPlantProfitButton.onClick.AddListener(OnUpgradeAllPlantProfit);
            addCustomerButton.onClick.AddListener(OnAddCustomer);
        }

        private void OnUpgradeAllPlantProfit()
        {
            _gameManager.UpgradeSystem.UpgradeAllPlantProfit(
                _gameManager.RunTime.upgradeState, 2f);
        }

        private void OnAddCustomer()
        {
            _gameManager.UpgradeSystem.AddCustomers(
                _gameManager.RunTime.upgradeState, 2);
        }
    }
}