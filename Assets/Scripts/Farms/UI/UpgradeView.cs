using System;
using System.Collections.Generic;
using Farms.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class UpgradeView : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        protected Transform container;

        [BoxGroup("References")] [SerializeField]
        protected UpgradeItemView itemPrefab;

        [BoxGroup("References")] [SerializeField]
        protected Button btnClose;

        public void Setup(List<UpgradeData> upgrades, Action<UpgradeItemView, UpgradeData> onUpgradeClicked)
        {
            Debug.LogError("Setup");
            // Clear old items
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            // Create new items
            foreach (var upgradeData in upgrades)
            {
                var itemView = Instantiate(itemPrefab, container);
                itemView.SetupView(upgradeData, onUpgradeClicked);
            }

            if (btnClose != null)
            {
                btnClose.onClick.RemoveAllListeners();
                btnClose.onClick.AddListener(() => gameObject.SetActive(false));
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
