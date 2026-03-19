using System;
using Farms.Core;
using Farms.ScriptableObjects;
using Farms.Systems;
using UnityEngine;

namespace Farms.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] protected PlantConfigSo plantConfig;
        [SerializeField] protected GameBalanceSo gameBalance;

        public FarmRuntime RunTime { get; private set; }
        public EconomySystem EconomySystem { get; private set; }
        public BuildSystem BuildSystem { get; private set; }
        public HarvestSystem HarvestSystem { get; private set; }
        public UpgradeSystem UpgradeSystem { get; private set; }
        public DeliverySystem DeliverySystem { get; private set; }
        public CustomerSystem CustomerSystem { get; private set; }
        public ProfitCalculator ProfitCalculator { get; private set; }

        public PlantConfigSo PlantConfig => plantConfig;
        public GameBalanceSo GameBalance => gameBalance;

        private void Awake()
        {
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            ProfitCalculator = new ProfitCalculator(gameBalance.levelProfitStep);
            EconomySystem = new EconomySystem();
            BuildSystem = new BuildSystem();
            HarvestSystem = new HarvestSystem(ProfitCalculator);
            UpgradeSystem = new UpgradeSystem();
            DeliverySystem = new DeliverySystem();
            CustomerSystem = new CustomerSystem(gameBalance.baseCustomerLimit);
        }

        private void Update()
        {
            TickPlants();
            TickCustomerSpawn();
        }

        private void TickPlants()
        {
            foreach (var runTimePlant in RunTime.plants)
            {
                runTimePlant.Tick(Time.deltaTime);
            }
        }

        private void TickCustomerSpawn()
        {
            if (CustomerSystem.CanSpawn(RunTime))
            {
                CustomerSystem.SpawnCustomer(RunTime);
            }
        }
    }
}