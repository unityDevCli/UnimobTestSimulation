using TMPro;
using UnityEngine;

namespace Farms
{
    public class GemMainView : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txtGem;

        public void UpdateGem(float gem)
        {
            txtGem.text = gem.ToString("N0");
        }
    }
}