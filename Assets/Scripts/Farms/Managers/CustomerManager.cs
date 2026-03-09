using Farms;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class CustomerManager : MonoBehaviour
    {
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private static readonly int IsCarryMove = Animator.StringToHash("IsCarryMove");
        private static readonly int IsEmpty = Animator.StringToHash("IsEmpty");
        [SerializeField] private FarmSimulationManager farmManager;
        [BoxGroup("Prefabs")] [SerializeField] protected GameObject customerPrefab;
        [ShowInInspector, ReadOnly] private readonly List<Customer> _customers = new List<Customer>();
        private readonly Queue<Customer> _waitingQueue = new Queue<Customer>();

        private float _customerSpawnCooldown;
        private int _targetCustomerCount;

        public int WaitingCount => _waitingQueue.Count;

        public void Initialize(int initialCount)
        {
            _targetCustomerCount = Mathf.Max(1, initialCount);
            for (var i = 0; i < _targetCustomerCount; i++)
            {
                SpawnCustomer();
            }
        }

        public void UpdateManager()
        {
            SimulateCustomers();
            MaintainCustomerCount();
        }

        private void SimulateCustomers()
        {
            for (var i = _customers.Count - 1; i >= 0; i--)
            {
                var customer = _customers[i];
                if (customer.gameObject == null)
                {
                    _customers.RemoveAt(i);
                    continue;
                }

                var currentPos = customer.gameObject.transform.position;
                var targetPos = customer.target;

                targetPos.y = 0f;

                customer.gameObject.transform.position = Vector3.MoveTowards(
                    currentPos,
                    targetPos,
                    farmManager.CustomerSpeed * Time.deltaTime);

                if (targetPos != currentPos)
                {
                    var direction = (targetPos - currentPos).normalized;
                    if (direction != Vector3.zero)
                    {
                        customer.gameObject.transform.rotation = Quaternion.LookRotation(direction);
                    }
                }

                if (customer.animator != null)
                {
                    var dist = Vector3.Distance(currentPos, customer.gameObject.transform.position);
                    var speed = dist / Time.deltaTime;

                    var isMoving = speed > 0.01f;
                    var hasItems = false;
                    var view = customer.gameObject.GetComponent<CustomerView>();
                    if (view != null && view.tomatoTransForms != null)
                    {
                        foreach (var pt in view.tomatoTransForms)
                        {
                            if (pt != null && pt.childCount > 0)
                            {
                                hasItems = true;
                                break;
                            }
                        }
                    }

                    customer.animator.SetBool(IsMove, isMoving);
                    customer.animator.SetBool(IsEmpty, !hasItems);
                    customer.animator.SetBool(IsCarryMove, isMoving && hasItems);
                }

                if (Vector3.Distance(customer.gameObject.transform.position, targetPos) < 0.15f)
                {
                    if (!customer.leaving && customer.state is CustomerState.AtCurrency or CustomerState.AtDelivery)
                    {
                        var camPos = Camera.main.transform.position;
                        camPos.y = customer.gameObject.transform.position.y;
                        var dirToCam = (camPos - customer.gameObject.transform.position).normalized;
                        if (dirToCam != Vector3.zero)
                        {
                            customer.gameObject.transform.rotation = Quaternion.Slerp(
                                customer.gameObject.transform.rotation, Quaternion.LookRotation(dirToCam),
                                Time.deltaTime * 5f);
                        }
                    }

                    if (customer.leaving)
                    {
                        if (farmManager.DeliveryEndPoint != null &&
                            targetPos != farmManager.DeliveryEndPoint.position)
                        {
                            customer.target = farmManager.DeliveryEndPoint.position;
                        }
                        else
                        {
                            var view = customer.gameObject.GetComponent<CustomerView>();
                            if (view != null && view.tomatoTransForms != null)
                            {
                                foreach (var pt in view.tomatoTransForms)
                                {
                                    if (pt == null) continue;
                                    for (int j = pt.childCount - 1; j >= 0; j--)
                                    {
                                        Destroy(pt.GetChild(j).gameObject);
                                    }
                                }
                            }

                            Destroy(customer.gameObject);
                            _customers.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private void MaintainCustomerCount()
        {
            _customerSpawnCooldown -= Time.deltaTime;
            if (_customerSpawnCooldown > 0f) return;

            if (_customers.Count < _targetCustomerCount)
            {
                SpawnCustomer();
                _customerSpawnCooldown = 1.25f;
            }
        }

        public void RefreshQueueTargets()
        {
            var queued = _waitingQueue.ToArray();
            for (var i = 0; i < queued.Length; i++)
            {
                if (i < 4) // Nếu nằm trong 4 vị trí đầu của hàng đợi (có thể tương ứng với 4 dock)
                {
                    Transform targetPoint = null;
                    if (farmManager.DockCurrencyPoints != null && i < farmManager.DockCurrencyPoints.Length)
                    {
                        targetPoint = farmManager.DockCurrencyPoints[i];
                        queued[i].dockIndex = i; // Lưu lại index của dock
                    }

                    if (targetPoint != null)
                    {
                        queued[i].target = targetPoint.position + farmManager.CustomerDeliveryOffset;
                    }
                    else
                    {
                        queued[i].target = farmManager.StallPoint.position + farmManager.QueueStartOffset +
                                           new Vector3(i * farmManager.QueueSpacing, 0f, 0f);
                    }

                    queued[i].state = CustomerState.AtCurrency;
                }
                else
                {
                    queued[i].target = farmManager.StallPoint.position + farmManager.QueueStartOffset +
                                       new Vector3(i * farmManager.QueueSpacing, 0f, 0f);
                    queued[i].state = CustomerState.Waiting;
                }
            }
        }

        public void SpawnCustomer()
        {
            GameObject obj;
            var spawnPos = farmManager.CustomerSpawnPoint.position;
            if (customerPrefab != null)
            {
                obj = Instantiate(customerPrefab, spawnPos, Quaternion.identity);
                obj.name = "Customer";
            }
            else
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.name = "Customer";
                obj.transform.position = spawnPos;
            }

            obj.transform.SetParent(farmManager.FarmRoot);

            var runtime = new Customer
            {
                gameObject = obj,
                animator = obj.GetComponent<CustomerView>().animator,
                leaving = false,
                state = CustomerState.Waiting
            };

            _customers.Add(runtime);
            _waitingQueue.Enqueue(runtime);
            RefreshQueueTargets();
        }

        public void AddCustomers(int amount)
        {
            _targetCustomerCount += Mathf.Max(0, amount);
            _targetCustomerCount = Mathf.Max(_targetCustomerCount, 1);
        }

        public Customer DequeueSpecificCustomer(Customer customer)
        {
            if (customer == null) return null;

            // We need to remove from the queue. Since Queue doesn't support random removal, we rebuild it.
            var newQueue = new Queue<Customer>();
            Customer found = null;
            while (_waitingQueue.Count > 0)
            {
                var item = _waitingQueue.Dequeue();
                if (item == customer && found == null)
                {
                    found = item;
                }
                else
                {
                    newQueue.Enqueue(item);
                }
            }

            // Re-assign the rebuilt queue
            foreach (var item in newQueue)
            {
                _waitingQueue.Enqueue(item);
            }

            if (found != null)
            {
                if (farmManager.DockDeliveryPoints is { Length: > 0 })
                {
                    var deliveryIndex = (found.dockIndex >= 0 &&
                                         found.dockIndex < farmManager.DockDeliveryPoints.Length)
                        ? found.dockIndex
                        : 0;

                    found.target = farmManager.DockDeliveryPoints[deliveryIndex].position +
                                   farmManager.CustomerDeliveryOffset;
                    found.state = CustomerState.AtDelivery;
                }

                return found;
            }

            return null;
        }

        public Customer DequeueCustomer()
        {
            if (_waitingQueue.Count > 0)
            {
                var customer = _waitingQueue.Dequeue();
                if (farmManager.DockDeliveryPoints is { Length: > 0 })
                {
                    var deliveryIndex = (customer.dockIndex >= 0 &&
                                         customer.dockIndex < farmManager.DockDeliveryPoints.Length)
                        ? customer.dockIndex
                        : 0;

                    customer.target = farmManager.DockDeliveryPoints[deliveryIndex].position +
                                      farmManager.CustomerDeliveryOffset;
                    customer.state = CustomerState.AtDelivery;
                }

                return customer;
            }

            return null;
        }

        public Customer PeekCustomer()
        {
            return _waitingQueue.Count > 0 ? _waitingQueue.Peek() : null;
        }

        public List<Customer> GetAvailableCustomersAtDock()
        {
            var results = new List<Customer>();
            foreach (var customer in _waitingQueue)
            {
                if (customer.state == CustomerState.AtCurrency && !customer.workerAssigned)
                {
                    results.Add(customer);
                }
            }

            return results;
        }

        public int GetCustomerCount()
        {
            return _customers.Count;
        }
    }
}