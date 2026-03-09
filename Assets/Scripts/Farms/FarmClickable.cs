using UnityEngine;

namespace Farms
{
    public class FarmClickable : MonoBehaviour
    {
        public int plotIndex;
        public FarmClickableType clickableType;
    }

    public enum FarmClickableType
    {
        Box,
        Plant
    }
}