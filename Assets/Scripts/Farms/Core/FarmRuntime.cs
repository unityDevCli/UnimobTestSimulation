using System.Collections.Generic;
using Farms.Entities;
using UnityEngine;

namespace Farms.Core
{
    public class FarmRuntime : MonoBehaviour
    {
        public List<ConstructionBox> constructionBoxes = new();
        public List<Plant> plants = new();
        public List<WorkerAgent> workers = new();
        public Queue<CustomerAgent> customers = new();
        public FarmUpgradeState upgradeState = new();
        public int nextCustomerId = 1;
    }
}