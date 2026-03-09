using Farms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class WorkerManager : MonoBehaviour
    {
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private static readonly int IsEmpty = Animator.StringToHash("IsEmpty");
        private static readonly int IsCarryMove = Animator.StringToHash("IsCarryMove");
        [SerializeField] private FarmSimulationManager farmManager;
        [BoxGroup("Prefabs")] [SerializeField] protected GameObject workerPrefab;
        [BoxGroup("Prefabs")] [SerializeField] protected GameObject tomatoPrefab;

        [ShowInInspector, ReadOnly]
        private class WorkerInstance
        {
            public GameObject obj;
            public Animator animator;
            public WorkerState state = WorkerState.Idle;
            public Plot targetPlot;
            public float carriedValue;
            public List<Transform> carriedMaterials = new List<Transform>();
            public Transform[] handPoints;
            public Customer assignedCustomer;
            public float harvestTimer;
        }

        [ShowInInspector, ReadOnly]   private List<WorkerInstance> _workers = new List<WorkerInstance>();
        private float _spawnCooldown;


        public void UpdateManager()
        {
            SimulateWorkers();
        }

        private void SimulateWorkers()
        {
            if (farmManager == null || farmManager.PlotManager == null) return;

            _spawnCooldown -= Time.deltaTime;
            if (_spawnCooldown <= 0f)
            {
                var plots = farmManager.PlotManager.Plots;
                for (var j = 0; j < plots.Count; j++)
                {
                    var plot = plots[j];
                    if (!plot.built || plot.readyFruits < 3) continue;

                    var assigned = false;
                    foreach (var w in _workers)
                    {
                        if (w.targetPlot == plot)
                        {
                            assigned = true;
                            break;
                        }
                    }

                    if (!assigned)
                    {
                        // Check if "full" (more workers than customers)
                        int workerCount = _workers.Count;
                        int customerCount = farmManager.CustomerManager.GetCustomerCount(); 
                        if (workerCount < customerCount)
                        {
                            CreateWorkerForPlot(plot);
                            _spawnCooldown = 1.0f;
                            break;
                        }
                    }
                }
            }

            for (int i = _workers.Count - 1; i >= 0; i--)
            {
                var worker = _workers[i];
                if (worker.obj == null)
                {
                    if (worker.assignedCustomer != null)
                    {
                        worker.assignedCustomer.workerAssigned = false;
                        worker.assignedCustomer = null;
                    }

                    _workers.RemoveAt(i);
                    continue;
                }

                if (worker.state == WorkerState.GoToPlant)
                {
                    if (worker.targetPlot.constructionView != null &&
                        worker.targetPlot.constructionView.deliveryPoint != null)
                    {
                        var target = worker.targetPlot.constructionView.deliveryPoint.position;

                        MoveWorkerTo(worker, target);
                        if (Vector3.Distance(worker.obj.transform.position, target) < 0.15f)
                        {
                            if (worker.targetPlot != null)
                            {
                                worker.targetPlot.state = PlotState.Harvesting;
                                // readyFruits = 0 and growTimer = 0 will be moved into HarvestFromTargetPlot or after delay
                                
                                HarvestFromTargetPlot(worker);
                                worker.harvestTimer = 3 * 0.15f + 0.5f; // count * delay + duration
                            }

                            UpdateIdleAnimation(worker); // Ensure idle at plant
                            worker.state = WorkerState.Harvesting;
                        }
                    }
                }
                else if (worker.state == WorkerState.Harvesting)
                {
                    worker.harvestTimer -= Time.deltaTime;
                    UpdateIdleAnimation(worker); // Ensure idle when harvesting
                    if (worker.harvestTimer <= 0f)
                    {
                        if (worker.targetPlot != null)
                            worker.targetPlot.state = PlotState.Growing;

                        worker.state = WorkerState.GoToStall;
                    }
                }
                else if (worker.state == WorkerState.GoToStall)
                {
                    if (worker.assignedCustomer == null)
                    {
                        var available = farmManager.CustomerManager.GetAvailableCustomersAtDock();
                        if (available.Count > 0)
                        {
                            worker.assignedCustomer = available[0];
                            worker.assignedCustomer.workerAssigned = true;
                        }
                    }

                    Vector3 target;
                    if (worker.assignedCustomer != null && farmManager.DockDeliveryPoints != null &&
                        worker.assignedCustomer.dockIndex >= 0 &&
                        worker.assignedCustomer.dockIndex < farmManager.DockDeliveryPoints.Length)
                    {
                        target = farmManager.DockDeliveryPoints[worker.assignedCustomer.dockIndex].position;
                    }
                    else
                    {
                        target = farmManager.DockDeliveryPoints is { Length: > 0 }
                            ? farmManager.DockDeliveryPoints[0].position
                            : farmManager.StallPoint.position + new Vector3(0.4f, 0f, 0f);
                    }

                    MoveWorkerTo(worker, target);
                    if (Vector3.Distance(worker.obj.transform.position, target) < 0.15f)
                    {
                        worker.state = WorkerState.WaitingForCustomer;
                    }
                }
                else if (worker.state == WorkerState.WaitingForCustomer)
                {
                    if (worker.assignedCustomer is { state: CustomerState.AtCurrency })
                    {
                        SellToCustomer(worker);
                        worker.state = WorkerState.Leaving;
                    }
                    else if (worker.assignedCustomer == null || worker.assignedCustomer.gameObject == null)
                    {
                        var available = farmManager.CustomerManager.GetAvailableCustomersAtDock();
                        if (available.Count > 0)
                        {
                            worker.assignedCustomer = available[0];
                            worker.assignedCustomer.workerAssigned = true;
                        }
                        else
                        {
                            MoveWorkerTo(worker, worker.obj.transform.position);
                        }
                    }
                    else
                    {
                        MoveWorkerTo(worker, worker.obj.transform.position);
                    }
                }
                else if (worker.state == WorkerState.Leaving)
                {
                    var target = farmManager.DeliveryEndPoint != null
                        ? farmManager.DeliveryEndPoint.position
                        : farmManager.StallPoint.position;
                    MoveWorkerTo(worker, target);
                    if (Vector3.Distance(worker.obj.transform.position, target) < 0.2f)
                    {
                        Destroy(worker.obj);
                        _workers.RemoveAt(i);
                    }
                }
            }
        }

        private void MoveWorkerTo(WorkerInstance worker, Vector3 target)
        {
            Vector3 currentPos = worker.obj.transform.position;
            Vector3 targetPosWithY = new Vector3(target.x, currentPos.y, target.z);
            worker.obj.transform.position = Vector3.MoveTowards(currentPos, targetPosWithY,
                farmManager.WorkerSpeed * Time.deltaTime);

            if (targetPosWithY != currentPos)
            {
                Vector3 direction = (targetPosWithY - currentPos).normalized;
                if (direction != Vector3.zero)
                {
                    worker.obj.transform.rotation = Quaternion.LookRotation(direction);
                }
            }

            if (worker.animator != null)
            {
                float dist = Vector3.Distance(currentPos, worker.obj.transform.position);
                float speed = Time.deltaTime > 0 ? dist / Time.deltaTime : 0;

                bool isMoving = speed > 0.01f;
                bool isEmpty = worker.carriedMaterials.Count == 0;
                worker.animator.SetBool(IsMove, isMoving);
                worker.animator.SetBool(IsEmpty, isEmpty);
                worker.animator.SetBool(IsCarryMove, isMoving && !isEmpty);
            }
        }

        private void HarvestFromTargetPlot(WorkerInstance worker)
        {
            if (worker.targetPlot is not { built: true } || worker.targetPlot.readyFruits < 3)
            {
                worker.carriedValue = 0f;
                return;
            }

            worker.targetPlot.readyFruits = 0;
            worker.targetPlot.growTimer = 0f;

            var count = 0;
            while (worker.targetPlot.spawnedFruits.Count > 0)
            {
                var index = 0;
                var fruitObj = worker.targetPlot.spawnedFruits[index];
                worker.targetPlot.spawnedFruits.RemoveAt(index);

                if (fruitObj != null)
                {
                    Transform targetHand = null;
                    if (worker.handPoints is { Length: > 0 })
                    {
                        targetHand = worker.handPoints[worker.carriedMaterials.Count % worker.handPoints.Length];
                    }
                    else if (worker.obj != null)
                    {
                        targetHand = worker.obj.transform;
                    }

                    var delay = count * 0.15f; 
                    worker.obj.GetComponent<MonoBehaviour>().StartCoroutine(DelayedMove(fruitObj, targetHand, delay, 0.5f, 1.5f));

                    worker.carriedMaterials.Add(fruitObj.transform);
                    count++;
                }
            }

            var fruitValue = farmManager.BaseFruitPrice *
                             worker.targetPlot.TotalProfitMultiplier(farmManager.GlobalProfitMultiplier);
            worker.carriedValue = fruitValue * 3; // Value for 3 fruits
            
        }

        private IEnumerator DelayedMove(GameObject obj, Transform target, float delay, float duration, float height)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null && target != null)
            {
                obj.SendMessage("MoveTo",
                    new object[] { target, duration, height, null },
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        private void SellToCustomer(WorkerInstance worker)
        {
            if (worker.carriedValue <= 0f)
            {
                worker.carriedValue = 0f;
                foreach (var m in worker.carriedMaterials)
                    if (m != null)
                        Destroy(m.gameObject);
                worker.carriedMaterials.Clear();
                if (worker.assignedCustomer != null)
                {
                    worker.assignedCustomer.workerAssigned = false;
                    worker.assignedCustomer = null;
                }

                return;
            }

            var customer = farmManager.CustomerManager.DequeueSpecificCustomer(worker.assignedCustomer);
            if (customer != null)
            {
                var customerView = customer.gameObject.GetComponent<CustomerView>();
                if (customerView != null && customerView.tomatoTransForms is { Length: > 0 })
                {
                    for (int i = 0; i < worker.carriedMaterials.Count; i++)
                    {
                        var materialTrans = worker.carriedMaterials[i];
                        if (materialTrans == null) continue;

                        var targetPoint = customerView.tomatoTransForms[i % customerView.tomatoTransForms.Length];

                        // Bay lần lượt sang đầu customer
                        float delay = i * 0.15f;
                        worker.obj.GetComponent<MonoBehaviour>()
                            .StartCoroutine(DelayedMove(materialTrans.gameObject, targetPoint, delay, 0.5f, 1.0f));
                    }
                }
                else
                {
                    foreach (var m in worker.carriedMaterials)
                        if (m != null)
                            Destroy(m.gameObject);
                }

                worker.carriedMaterials.Clear();
                farmManager.Cash += worker.carriedValue;

                // Spawn Pay effect
                if (farmManager.EffPayPrefab != null)
                {
                    Instantiate(farmManager.EffPayPrefab, customer.gameObject.transform.position, Quaternion.identity);
                }

                worker.carriedValue = 0f;

                customer.leaving = true;
                customer.target = farmManager.CustomerExitPoint.position;
                farmManager.CustomerManager.RefreshQueueTargets();
                farmManager.CustomerManager.SpawnCustomer();
            }
            else
            {
                foreach (var m in worker.carriedMaterials.Where(m => m != null))
                    Destroy(m.gameObject);
                worker.carriedMaterials.Clear();
                worker.carriedValue = 0f;
            }
        }

        private void CreateWorkerForPlot(Plot plot)
        {
            var spawnPos = farmManager.DeliveryEndPoint != null
                ? farmManager.DeliveryEndPoint.position
                : farmManager.StallPoint.position + farmManager.WorkerSpawnOffset;
            GameObject newWorker;
            if (workerPrefab != null)
            {
                newWorker = Instantiate(workerPrefab, spawnPos, Quaternion.identity);
                newWorker.name = "Delivery_" + plot.index;
            }
            else
            {
                newWorker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                newWorker.name = "Delivery_" + plot.index;
                newWorker.transform.position = spawnPos;
            }

            newWorker.transform.SetParent(farmManager.FarmRoot);

            var workerView = newWorker.GetComponent<CustomerView>();
            Transform[] hands = null;
            if (workerView != null && workerView.tomatoTransForms is { Length: > 0 })
            {
                hands = workerView.tomatoTransForms;
            }
            else
            {
                var singleHand = FindHandPoint(newWorker.transform);
                if (singleHand != null) hands = new[] { singleHand };
            }

            var instance = new WorkerInstance
            {
                obj = newWorker,
                animator = workerView != null ? workerView.animator : newWorker.GetComponent<Animator>(),
                state = WorkerState.GoToPlant,
                targetPlot = plot,
                handPoints = hands
            };

            _workers.Add(instance);
        }

        private Transform FindHandPoint(Transform parent)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>())
            {
                if (child.name.ToLower().Contains("hand")) return child;
            }

            return null;
        }

        private bool HasAnyBuiltPlot()
        {
            if (farmManager == null || farmManager.PlotManager == null) return false;
            foreach (var plot in farmManager.PlotManager.Plots)
            {
                if (plot.built) return true;
            }

            return false;
        }

        private void UpdateIdleAnimation(WorkerInstance worker)
        {
            if (worker.animator != null)
            {
                worker.animator.SetBool(IsMove, false);
                worker.animator.SetBool(IsEmpty, worker.carriedMaterials.Count == 0);
                worker.animator.SetBool(IsCarryMove, false);
            }
        }

        private void UpdateIdleAnimation()
        {
            // Not used in multi-worker setup as they are destroyed or moving
        }

        private void SimulateWorker()
        {
            // Obsolete, replaced by SimulateWorkers
        }

        private void CreateWorker()
        {
            // Obsolete, replaced by CreateWorkerForPlot
        }
    }
}