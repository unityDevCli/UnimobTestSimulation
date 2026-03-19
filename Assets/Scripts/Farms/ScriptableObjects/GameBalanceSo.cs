using UnityEngine;

namespace Farms.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameBalance", menuName = "_FarmSimulation/GameBalance", order = 0)]
    public class GameBalanceSo : ScriptableObject
    {
        public int baseCustomerLimit = 1;
        public float levelProfitStep = 0.1f;
    }
}