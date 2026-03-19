using Farms.Core;
using Farms.Entities;
using Farms.Enums;

namespace Farms.Systems
{
    public class CustomerSystem
    {
        private readonly int _baseCustomerLimit;

        public CustomerSystem(int baseCustomerLimit)
        {
            _baseCustomerLimit = baseCustomerLimit;
        }

        public int GetMaxCustomers(FarmUpgradeState state)
        {
            return _baseCustomerLimit + state.extraCustomerCount;
        }

        public bool CanSpawn(FarmRuntime runtime)
        {
            return runtime.customers.Count < GetMaxCustomers(runtime.upgradeState);
        }

        public CustomerAgent SpawnCustomer(FarmRuntime runtime)
        {
            var customer = new CustomerAgent()
            {
                customerId = runtime.nextCustomerId++,
                stateType = CustomerStateType.MovingToQueue
            };
            runtime.customers.Enqueue(customer);
            return customer;
        }

        public CustomerAgent PeekCurrentCustomer(FarmRuntime runtime)
        {
            return runtime.customers.Count > 0 ? runtime.customers.Peek() : null;
        }

        public void CompleteCurrentCustomer(FarmRuntime runtime)
        {
            if (runtime.customers.Count > 0)
                runtime.customers.Dequeue();
        }
    }
}