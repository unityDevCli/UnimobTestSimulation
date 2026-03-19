using Farms.Enums;

namespace Farms.Entities
{
    public class WorkerAgent
    {
        public int workerId;
        public WorkerStateType stateType;
        public Plant targetPlant;
        public HarvestBundle carryingBundle;

        public bool HasCarryingGo => carryingBundle is { fruitAmount: > 0 };
    }
}