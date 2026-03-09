using UnityEngine;

namespace Farms
{
    public enum CustomerState
    {
        Waiting,
        AtCurrency,
        AtDelivery,
        Leaving
    }

    public class Customer
    {
        public GameObject gameObject;
        public Animator animator;
        public Vector3 target;
        public bool leaving;
        public CustomerState state = CustomerState.Waiting;
        public int dockIndex = -1;
        public bool workerAssigned = false;
    }
}