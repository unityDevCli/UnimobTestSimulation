using Farms.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class PlantUpgradePanelUI : MonoBehaviour
    {
        [SerializeField] protected Button levelUpgradeButton;
        [SerializeField] protected Button profitUpgradeButton;

        private PlantController _controller;
        private int _selectedPlantId;

        public void Init(PlantController controller)
        {
            _controller = controller;
            levelUpgradeButton.onClick.AddListener(OnLevelUpgrade);
            profitUpgradeButton.onClick.AddListener(OnProfitUpgrade);
        }

        public void Open(int plantId)
        {
            _selectedPlantId = plantId;
            gameObject.SetActive(true);
        }

        private void OnProfitUpgrade()
        {
            _controller.UpgradeSinglePlantProfit(_selectedPlantId, 1.2f);
        }

        private void OnLevelUpgrade()
        {
            _controller.UpgradePlant(_selectedPlantId);
        }
    }
}