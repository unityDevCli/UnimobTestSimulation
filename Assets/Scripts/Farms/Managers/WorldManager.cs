using Farms;
using Farms.ScriptableObjects;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private FarmSimulationManager farmManager;

        [BoxGroup("Setup World")] [SerializeField]
        protected GameObject boxPrefab;

        [BoxGroup("Setup World")] [SerializeField]
        protected GameObject constructionPrefab;

        [BoxGroup("Setup World")] [SerializeField]
        protected GameObject marketPrefab;

        [BoxGroup("Setup World")] [SerializeField]
        protected GameObject centerBoxPrefab;

        [BoxGroup("References")] [SerializeField]
        protected ConstructionData clay;

        [BoxGroup("References")] [SerializeField]
        protected ConstructionData steel;

        [BoxGroup("References")] [SerializeField]
        protected ConstructionData wheat;

        [BoxGroup("References")] [SerializeField]
        protected ConstructionData wood;

        public void Initialize()
        {
            EnsureRuntimePoints();
            CreateLaneBlocks();
            CreatePlots();
        }

        private void EnsureRuntimePoints()
        {
            if (farmManager.StallPoint == null)
            {
                var obj = Instantiate(marketPrefab, farmManager.FarmRoot.transform);
                obj.name = "StallPoint";
                obj.transform.position = farmManager.StallPosition;
                farmManager.StallPoint = obj.transform;

                var marketRef = obj.GetComponent<MarketView>();
                if (marketRef == null) marketRef = farmManager.MarketView;

                if (marketRef != null)
                {
                    farmManager.MarketView = marketRef;
                    farmManager.CustomerStartPoint = marketRef.customerStart;
                    farmManager.CustomerEndPoint = marketRef.customerEnd;
                    farmManager.DeliveryEndPoint = marketRef.deliveryEnd;
                    farmManager.DockCurrencyPoints = marketRef.currency;
                    farmManager.DockDeliveryPoints = marketRef.delivery;
                }
            }

            if (farmManager.CustomerSpawnPoint == null)
            {
                if (farmManager.CustomerStartPoint != null)
                {
                    farmManager.CustomerSpawnPoint = farmManager.CustomerStartPoint;
                }
                else
                {
                    var obj = new GameObject("CustomerSpawn");
                    obj.transform.SetParent(farmManager.FarmRoot);
                    obj.transform.position = farmManager.StallPoint.position + farmManager.CustomerSpawnOffset;
                    farmManager.CustomerSpawnPoint = obj.transform;
                }
            }

            if (farmManager.CustomerExitPoint == null)
            {
                if (farmManager.CustomerEndPoint != null)
                {
                    farmManager.CustomerExitPoint = farmManager.CustomerEndPoint;
                }
                else
                {
                    var obj = new GameObject("CustomerExit");
                    obj.transform.SetParent(farmManager.FarmRoot);
                    obj.transform.position = farmManager.StallPoint.position + farmManager.CustomerExitOffset;
                    farmManager.CustomerExitPoint = obj.transform;
                }
            }
        }

        private Transform FindChildRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name.ToLower() == name.ToLower()) return child;
                Transform found = FindChildRecursive(child, name);
                if (found != null) return found;
            }

            return null;
        }

        private void CreateLaneBlocks()
        {
            for (var i = 0; i < farmManager.CenterLanePositions.Length; i++)
            {
                var lane = Instantiate(centerBoxPrefab, farmManager.FarmRoot);
                lane.name = "CenterLane_" + i;
                lane.transform.SetParent(farmManager.FarmRoot);
                lane.transform.position = farmManager.CenterLanePositions[i] + new Vector3(0f, 0.65f, 0f);
                lane.transform.localScale = new Vector3(1.25f, 1.3f, 3.2f);
            }
        }

        private void CreatePlots()
        {
            farmManager.PlotManager.Plots.Clear();
            var configs = new List<ConstructionData>();
            if (farmManager.ConstructionConfigs is { Length: > 0 })
            {
                configs.AddRange(farmManager.ConstructionConfigs);
            }
            else
            {
                if (clay != null) configs.Add(clay);
                if (steel != null) configs.Add(steel);
                if (wheat != null) configs.Add(wheat);
                if (wood != null) configs.Add(wood);
            }

            if (configs.Count > 1)
            {
                for (int i = 0; i < configs.Count; i++)
                {
                    int randomIndex = Random.Range(i, configs.Count);
                    (configs[i], configs[randomIndex]) = (configs[randomIndex], configs[i]);
                }
            }

            for (var i = 0; i < farmManager.PlotPositions.Length; i++)
            {
                var center = farmManager.PlotPositions[i];

                ConstructionData config = null;
                if (configs.Count > 0)
                {
                    config = configs[i % configs.Count];
                }

                var box = InstantiateOrCreate(boxPrefab, PrimitiveType.Cube, "Box_" + i,
                    center + new Vector3(0f, 0.6f, 0f), new Vector3(1.4f, 1.4f, 1.4f));
                box.transform.SetParent(farmManager.FarmRoot);
                var boxClick = box.GetComponent<FarmClickable>();
                if (boxClick == null) boxClick = box.AddComponent<FarmClickable>();
                boxClick.plotIndex = i;
                boxClick.clickableType = FarmClickableType.Box;

                var constructionPrefabToUse = (config != null && config.constructionPrefab != null)
                    ? config.constructionPrefab
                    : constructionPrefab;
                var construction = InstantiateOrCreate(constructionPrefabToUse, PrimitiveType.Cube, "Construction_" + i,
                    center + new Vector3(0f, 0.6f, 0f), new Vector3(1.4f, 1.4f, 1.4f));
                construction.transform.SetParent(farmManager.FarmRoot);
                construction.SetActive(false);
                var constructionClick = construction.GetComponent<FarmClickable>();
                if (constructionClick == null) constructionClick = construction.AddComponent<FarmClickable>();
                constructionClick.plotIndex = i;
                constructionClick.clickableType = FarmClickableType.Plant;

                var plot = new Plot
                {
                    index = i,
                    constructionData = config,
                    boxObject = box,
                    boxAnimation = box.GetComponent<BoxView>().boxAnimation,
                    countdownView = null,
                    constructionObject = construction,
                    constructionView = construction.GetComponent<ConstructionView>(),
                    built = false,
                    isUnlocking = false,
                    unlockTimer = 0f,
                    level = 1,
                    localProfitMultiplier = 1f,
                    readyFruits = 0,
                    growTimer = 0f,
                    farmManager = farmManager
                };

                farmManager.PlotManager.AddPlot(plot);
            }
        }

        private GameObject InstantiateOrCreate(GameObject prefab, PrimitiveType fallback, string name, Vector3 position,
            Vector3 scale)
        {
            GameObject obj;
            if (prefab != null)
            {
                obj = Instantiate(prefab, position, Quaternion.identity);
                obj.name = name;
            }
            else
            {
                obj = GameObject.CreatePrimitive(fallback);
                obj.name = name;
                obj.transform.position = position;
            }

            obj.transform.localScale = scale;
            if (obj.GetComponent<Collider>() == null)
            {
                obj.AddComponent<BoxCollider>();
            }

            return obj;
        }
    }
}