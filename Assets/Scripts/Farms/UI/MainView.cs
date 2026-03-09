using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class MainView : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        protected Button btnUpgrade;

        [BoxGroup("References")] [SerializeField]
        protected GoldMainView goldView;

        [BoxGroup("References")] [SerializeField]
        protected GemMainView gemView;

        public void Setup(Action onUpgradeClick)
        {
            if (btnUpgrade != null)
            {
                btnUpgrade.onClick.RemoveAllListeners();
                btnUpgrade.onClick.AddListener(() => onUpgradeClick?.Invoke());
            }
        }

        public void UpdateCash(float cash)
        {
            if (goldView != null)
            {
                goldView.UpdateGold(cash);
            }
        }

        public void UpdateGem(float gem)
        {
            if (gemView != null)
            {
                gemView.UpdateGem(gem);
            }
        }
    }
}
