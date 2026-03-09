using System.Collections.Generic;
using Farms;
using Farms.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Farms.Managers
{
    public class FarmSimulationManager : MonoBehaviour
    {
        [FoldoutGroup("Managers")] [SerializeField]
        protected CustomerManager customerManager;

        public CustomerManager CustomerManager => customerManager;

        [FoldoutGroup("Managers")] [SerializeField]
        protected PlotManager plotManager;

        public PlotManager PlotManager => plotManager;

        [FoldoutGroup("Managers")] [SerializeField]
        protected WorkerManager workerManager;

        public WorkerManager WorkerManager => workerManager;

        [FoldoutGroup("Managers")] [SerializeField]
        protected WorldManager worldManager;

        public WorldManager WorldManager => worldManager;

        [FoldoutGroup("Managers")] [SerializeField]
        protected UIManager uiManager;

        public UIManager UIManager => uiManager;

        [FoldoutGroup("Managers")] [SerializeField]
        protected UpgradeManager upgradeManager;

        public UpgradeManager UpgradeManager => upgradeManager;

        [FoldoutGroup("Prefabs")] [SerializeField]
        protected GameObject constructionPrefab;

        public GameObject ConstructionPrefab => constructionPrefab;

        [FoldoutGroup("Prefabs")] [SerializeField]
        protected GameObject plantPrefab;

        public GameObject PlantPrefab => plantPrefab;

        [FoldoutGroup("Prefabs")] [SerializeField]
        protected GameObject effBuildDonePrefab;

        public GameObject EffBuildDonePrefab => effBuildDonePrefab;

        [FoldoutGroup("Prefabs")] [SerializeField]
        protected GameObject effPayPrefab;

        public GameObject EffPayPrefab => effPayPrefab;

        [FoldoutGroup("Prefabs")] [SerializeField]
        protected GameObject[] materialPrefabs; // 0: Clay, 1: Steel, 2: Wheat, 3: Wood

        public GameObject GetMaterialPrefab(PlotMaterialType type)
        {
            var index = (int)type;
            if (materialPrefabs != null && index >= 0 && index < materialPrefabs.Length)
                return materialPrefabs[index];
            return null;
        }

        [FoldoutGroup("UI")] [SerializeField] protected Canvas uiCanvas;

        public Canvas UiCanvas => uiCanvas;

        [FoldoutGroup("UI")] [SerializeField] protected bool showDebugOnGui;

        [FoldoutGroup("UI")] [SerializeField] protected Vector3 selectionViewWorldOffset = new Vector3(0f, 1.5f, 0f);

        public Vector3 SelectionViewWorldOffset => selectionViewWorldOffset;

        [FoldoutGroup("UI")] [SerializeField] protected Vector2 selectionViewScreenOffset = new Vector2(0f, 60f);

        public Vector2 SelectionViewScreenOffset => selectionViewScreenOffset;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected ScriptableObjects.ConstructionData[] constructionConfigs;

        public ScriptableObjects.ConstructionData[] ConstructionConfigs => constructionConfigs;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float baseUpgradePrice = 100f;

        public float BaseUpgradePrice => baseUpgradePrice;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float baseGrowthSeconds = 5f;

        public float GrowthSeconds => baseGrowthSeconds;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float baseFruitPrice = 10f;

        public float BaseFruitPrice => baseFruitPrice;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float workerSpeed = 3.5f;

        public float WorkerSpeed => workerSpeed;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float customerSpeed = 2.2f;

        public float CustomerSpeed => customerSpeed;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected int initialCustomerTarget = 1;

        [FoldoutGroup("Simulation")] [SerializeField]
        protected float buildUnlockSeconds = 5f;

        public float BuildUnlockSeconds => buildUnlockSeconds;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3[] plotPositions =
        {
            new Vector3(-3f, 0f, 4f),
            new Vector3(3f, 0f, 4f),
            new Vector3(-3f, 0f, -2f),
            new Vector3(3f, 0f, -2f)
        };

        public Vector3[] PlotPositions => plotPositions;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3[] centerLanePositions =
        {
            new Vector3(0f, 0f, 4f),
            new Vector3(0f, 0f, -2f)
        };

        public Vector3[] CenterLanePositions => centerLanePositions;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 stallPosition = new Vector3(0f, 0.55f, 9.2f);

        public Vector3 StallPosition => stallPosition;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 workerSpawnOffset = new Vector3(0f, 0f, 1.8f);

        public Vector3 WorkerSpawnOffset => workerSpawnOffset;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 customerSpawnOffset = new Vector3(7.6f, 0f, 0f);

        public Vector3 CustomerSpawnOffset => customerSpawnOffset;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 customerExitOffset = new Vector3(7.6f, 0f, -5f);

        public Vector3 CustomerExitOffset => customerExitOffset;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 queueStartOffset = new Vector3(2.3f, 0f, 0f);

        public Vector3 QueueStartOffset => queueStartOffset;

        [FoldoutGroup("Layout")] [SerializeField]
        private float queueSpacing = 1.1f;

        public float QueueSpacing => queueSpacing;

        [FoldoutGroup("Layout")] [SerializeField]
        private Vector3 customerDeliveryOffset = new Vector3(0f, 0f, 1.2f);

        public Vector3 CustomerDeliveryOffset => customerDeliveryOffset;

        [FoldoutGroup("References")] [SerializeField]
        private Transform farmRoot;

        public Transform FarmRoot => farmRoot;

        [FoldoutGroup("References")] [SerializeField]
        private Transform stallPoint;

        public Transform StallPoint
        {
            get => stallPoint;
            set => stallPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform customerSpawnPoint;

        public Transform CustomerSpawnPoint
        {
            get => customerSpawnPoint;
            set => customerSpawnPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform customerExitPoint;

        public Transform CustomerExitPoint
        {
            get => customerExitPoint;
            set => customerExitPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform customerStartPoint;

        public Transform CustomerStartPoint
        {
            get => customerStartPoint;
            set => customerStartPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform customerEndPoint;

        public Transform CustomerEndPoint
        {
            get => customerEndPoint;
            set => customerEndPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform deliveryEndPoint;

        public Transform DeliveryEndPoint
        {
            get => deliveryEndPoint;
            set => deliveryEndPoint = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform[] dockCurrencyPoints;

        public Transform[] DockCurrencyPoints
        {
            get => dockCurrencyPoints;
            set => dockCurrencyPoints = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private Transform[] dockDeliveryPoints;

        public Transform[] DockDeliveryPoints
        {
            get => dockDeliveryPoints;
            set => dockDeliveryPoints = value;
        }

        [FoldoutGroup("References")] [SerializeField]
        private MarketView marketView;

        public MarketView MarketView
        {
            get => marketView;
            set => marketView = value;
        }

        public float GlobalProfitMultiplier => _globalProfitMultiplier;
        public Camera MainCamera => _mainCamera;

        private Camera _mainCamera;
        private Plot _selectedPlot;

        private Plot _workerTargetPlot;
        private float _carriedValue;

        private float _cash;
        private float _gem;
        private float _globalProfitMultiplier = 1f;
        private int _targetCustomerCount;
        private float _customerSpawnCooldown;

        private GameObject _buildViewInstance;
        private GameObject _upgradeViewInstance;
        private GameObject _managementViewInstance;

        private void Awake()
        {
            EnsureManagers();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                var camObj = new GameObject("Main Camera");
                _mainCamera = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            if (farmRoot == null)
            {
                var root = new GameObject("FarmRuntimeRoot");
                farmRoot = root.transform;
            }

            if (marketView == null)
            {
                marketView = FindObjectOfType<MarketView>();
            }

            _cash = 0;
            _gem = 0;

            worldManager.Initialize();
            uiManager.Initialize();
            customerManager.Initialize(initialCustomerTarget);

            uiManager.RefreshSelectionViews();
            uiManager.UpdateMainViewCurrency();
        }

        private void EnsureManagers()
        {
            if (customerManager == null) customerManager = GetOrAddComponent<CustomerManager>();
            if (plotManager == null) plotManager = GetOrAddComponent<PlotManager>();
            if (workerManager == null) workerManager = GetOrAddComponent<WorkerManager>();
            if (worldManager == null) worldManager = GetOrAddComponent<WorldManager>();
            if (uiManager == null) uiManager = GetOrAddComponent<UIManager>();
            if (upgradeManager == null) upgradeManager = GetOrAddComponent<UpgradeManager>();

            SetFarmManager(customerManager);
            SetFarmManager(plotManager);
            SetFarmManager(workerManager);
            SetFarmManager(worldManager);
            SetFarmManager(uiManager);
            SetFarmManager(upgradeManager);
        }

        private T GetOrAddComponent<T>() where T : MonoBehaviour
        {
            T component = GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        private void SetFarmManager(MonoBehaviour manager)
        {
            if (manager == null) return;

            var type = manager.GetType();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public;
            var field = type.GetField("farmManager", flags);

            if (field == null && type.BaseType != null)
            {
                field = type.BaseType.GetField("farmManager", flags);
            }

            if (field != null)
            {
                field.SetValue(manager, this);
            }
            else
            {
                Debug.LogError(
                    $"[FarmSimulationManager] Could not find field 'farmManager' in {manager.GetType().Name}");
            }
        }

        private void Update()
        {
            plotManager.HandleClickSelection();
            plotManager.UpdateManager();
            customerManager.UpdateManager();
            workerManager.UpdateManager();
            uiManager.UpdateManager();
        }

        public float Cash
        {
            get => _cash;
            set
            {
                _cash = value;
                uiManager.UpdateMainViewCurrency();
            }
        }

        public float Gem
        {
            get => _gem;
            set
            {
                _gem = value;
                uiManager.UpdateMainViewCurrency();
            }
        }

        private double _cashDouble;

        public double CashDouble
        {
            get => _cashDouble;
            set => _cashDouble = value;
        }

        public void AddGlobalProfit(float multiplier)
        {
            _globalProfitMultiplier += multiplier;
        }

        public void AddLocalProfit(int plotIndex, float multiplier)
        {
            if (plotIndex >= 0 && plotIndex < plotManager.Plots.Count)
            {
                plotManager.Plots[plotIndex].localProfitMultiplier += multiplier;
            }
        }

        public void AddTargetCustomer(int count)
        {
            customerManager.AddCustomers(count);
        }
    }
}