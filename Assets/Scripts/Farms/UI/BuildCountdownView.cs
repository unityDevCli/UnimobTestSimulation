using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.UI
{
    public class BuildCountdownView : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtTimer;
        [SerializeField] private Slider sliderProgress;

        public void UpdateTimer(float secondsLeft, float totalSeconds)
        {
            if (txtTimer != null)
            {
                txtTimer.text = Mathf.CeilToInt(secondsLeft).ToString() + "s";
            }

            if (sliderProgress != null)
            {
                sliderProgress.value = secondsLeft / totalSeconds;
            }
        }
    }
}
