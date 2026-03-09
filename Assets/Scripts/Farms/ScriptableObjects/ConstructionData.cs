using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ConstructionData", menuName = "_Farm/ConstructionData", order = 0)]
    public class ConstructionData : ScriptableObject
    {
        public int index;
        public string constructionName;
        [PreviewField] public Sprite constructionIcon;
        [BoxGroup("References")] [SerializeField] public PlotMaterialType materialType;
        public int price;
        public float unlockSeconds;
        [BoxGroup("Prefabs")] public GameObject constructionPrefab;
    }
}