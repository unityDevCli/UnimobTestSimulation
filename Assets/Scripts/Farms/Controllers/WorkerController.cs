using System.Linq;
using Farms.Entities;
using Farms.Enums;
using Farms.Managers;

namespace Farms.Controllers
{
    public class WorkerController
    {
        private readonly GameManager _gameManager;
        private readonly WorkerAgent _workerAgent;

        public WorkerController(GameManager gameManager, WorkerAgent workerAgent)
        {
            _gameManager = gameManager;
            _workerAgent = workerAgent;
        }

        public void Tick()
        {
            switch (_workerAgent.stateType)
            {
                case WorkerStateType.Idle:
                    UpdateIdle();
                    break;
                case WorkerStateType.MovingToPlant:
                    UpdateMoveToPlant();
                    break;
                case WorkerStateType.Harvesting:
                    UpdateHarvesting();
                    break;
                case WorkerStateType.MovingToCounter:
                    UpdateMoveToCounter();
                    break;
                case WorkerStateType.Selling:
                    UpdateSelling();
                    break;
            }
        }

        private void UpdateIdle()
        {
            var targetPlant = _gameManager.RunTime.plants.FirstOrDefault(x => x.isReadyToHarvest);
            if (targetPlant == null) return;
            _workerAgent.targetPlant = targetPlant;
            _workerAgent.stateType = WorkerStateType.MovingToPlant;
        }

        private void UpdateMoveToPlant()
        {
            //TODO: Check NavmeshAgent to plant
            _workerAgent.stateType = WorkerStateType.Harvesting;
        }

        private void UpdateHarvesting()
        {
            if (_workerAgent.targetPlant == null)
            {
                _workerAgent.stateType = WorkerStateType.Idle;
                return;
            }

            var bundle =
                _gameManager.HarvestSystem.Harvest(_workerAgent.targetPlant, _gameManager.RunTime.upgradeState);
            if (bundle == null)
            {
                _workerAgent.stateType = WorkerStateType.Idle;
                return;
            }

            _workerAgent.carryingBundle = bundle;
            _workerAgent.targetPlant = null;
            _workerAgent.stateType = WorkerStateType.MovingToCounter;
        }

        private void UpdateMoveToCounter()
        {
            if (!_workerAgent.HasCarryingGo)
            {
                _workerAgent.stateType = WorkerStateType.Idle;
                return;
            }

            if (_gameManager.CustomerSystem.PeekCurrentCustomer(_gameManager.RunTime) == null) return;
            _workerAgent.stateType = WorkerStateType.Selling;
        }

        private void UpdateSelling()
        {
            var customer = _gameManager.CustomerSystem.PeekCurrentCustomer(_gameManager.RunTime);
            if (customer == null || !_workerAgent.HasCarryingGo)
            {
                _workerAgent.stateType = WorkerStateType.Idle;
                return;
            }

            var gold = _gameManager.DeliverySystem.Sell(_workerAgent.carryingBundle);
            _gameManager.EconomySystem.AddGold(gold);
            _gameManager.CustomerSystem.CompleteCurrentCustomer(_gameManager.RunTime);

            _workerAgent.carryingBundle = null;
            _workerAgent.stateType = WorkerStateType.Idle;
        }
    }
}