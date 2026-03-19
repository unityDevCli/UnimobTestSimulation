using System;
using Farms.Core;
using TMPro;
using UnityEngine;

namespace Farms.UI
{
    public class GoldUI : MonoBehaviour
    {
        [SerializeField] protected TMP_Text goldText;

        private void OnEnable()
        {
            GameEvents.OnGoldChanged += UpdateMoney;
        }

        private void OnDisable()
        {
            GameEvents.OnGoldChanged -= UpdateMoney;
        }

        private void UpdateMoney(float gold)
        {
            goldText.text = gold.ToString("F0");
        }
    }
}