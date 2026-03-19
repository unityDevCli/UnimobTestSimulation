using Farms.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.Views
{
    public class PlantView : MonoBehaviour
    {
        [SerializeField] protected Button upgradeButton;
        [SerializeField] protected TMP_Text levelText;
        [SerializeField] protected TMP_Text stateText;

        private PlantController _controller;
        private int _plantId;

        public void Init(int plantId, PlantController controller)
        {
            _plantId = plantId;
            _controller = controller;
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        public void Refresh(int level, bool ready)
        {
            levelText.text = level.ToString();
            stateText.text = ready ? "Ready" : "Growing";
        }

        private void OnUpgradeClicked()
        {
            _controller.UpgradePlant(_plantId);
        }
    }
}