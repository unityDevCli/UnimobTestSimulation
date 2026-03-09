using System.Globalization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class ConstructionUpgradeView : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtCurrentLevel;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtProduction;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtCurrentProfit;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtTimer;

        [BoxGroup("References")] [SerializeField]
        protected TMP_Text txtPrice;

        [BoxGroup("References")] [SerializeField]
        protected Slider sliderLevelProgress;

        [BoxGroup("References")] [SerializeField]
        protected GameObject maxObject;

        [BoxGroup("References")] [SerializeField]
        protected GameObject normalObject;

        private int _maxLevel = 10;
        private bool isMaxLevel;

        public void SetupView(int currentLevel, string production, float currentProfit, float timer, float price,
            bool isMax)
        {
            txtCurrentLevel.text = $"Level {currentLevel}";
            txtProduction.text = production;
            txtCurrentProfit.text = currentProfit.ToString(CultureInfo.InvariantCulture);
            txtTimer.text = timer.ToString("0.00") + "s";
            txtPrice.text = price.ToString(CultureInfo.InvariantCulture);
            sliderLevelProgress.maxValue = _maxLevel;
            sliderLevelProgress.value = currentLevel;
            isMaxLevel = isMax;
            maxObject.SetActive(isMax);
            normalObject.SetActive(!isMax);
        }
    }
}