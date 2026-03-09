using Farms;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class PlotManager : MonoBehaviour
    {
        [SerializeField] private FarmSimulationManager farmManager;
        [BoxGroup("Prefabs")] [SerializeField] protected GameObject plantPrefab;
        [ShowInInspector, ReadOnly] private readonly List<Plot> _plots = new List<Plot>();
        public List<Plot> Plots => _plots;

        private Plot _selectedPlot;

        public Plot SelectedPlot
        {
            get => _selectedPlot;
            set => _selectedPlot = value;
        }

        public void AddPlot(Plot plot)
        {
            _plots.Add(plot);
        }

        public void UpdateManager()
        {
            SimulatePlots();
        }

        private void SimulatePlots()
        {
            for (var i = 0; i < _plots.Count; i++)
            {
                var plot = _plots[i];
                if (plot.isUnlocking && !plot.built)
                {
                    plot.unlockTimer -= Time.deltaTime;
                    if (plot.unlockTimer <= 0f)
                    {
                        plot.unlockTimer = 0f;
                        plot.isUnlocking = false;
                        plot.built = true;
                        // plot.readyFruits = 3;
                        plot.growTimer = 0f;
                        plot.state = PlotState.Ready;
                        if (plot.boxObject != null) plot.boxObject.SetActive(false);
                        if (plot.countdownView != null) farmManager.UIManager.HideCountdown(plot);
                        if (plot.constructionObject != null) plot.constructionObject.SetActive(true);
                        
                        // Spawn effect
                        if (farmManager.EffBuildDonePrefab != null)
                        {
                            Instantiate(farmManager.EffBuildDonePrefab, plot.constructionObject.transform.position,
                                Quaternion.identity);
                        }

                        farmManager.UIManager.ShowInformation(plot);
                        if (_selectedPlot == plot)
                        {
                            farmManager.UIManager.ClearSelection();
                        }
                    }

                    if (_selectedPlot == plot)
                    {
                        farmManager.UIManager.UpdateSelectionContent();
                    }
                }

                if (!plot.built) continue;

                if (plot.state != PlotState.Harvesting)
                {
                    plot.state = plot.readyFruits >= 3 ? PlotState.Ready : PlotState.Growing;

                    var growthTime = plot.GetCurrentGrowthTime();

                    if (plot.state == PlotState.Growing)
                    {
                        plot.growTimer += Time.deltaTime;

                        var fruitSpawnInterval = growthTime / 3f;

                        int expectedFruits = Mathf.FloorToInt(plot.growTimer / fruitSpawnInterval) + 1;
                        expectedFruits = Mathf.Min(expectedFruits, 3);

                        if (plot.spawnedFruits.Count < expectedFruits)
                        {
                            SpawnFruitAtPoint(plot, plot.spawnedFruits.Count);
                        }

                        if (plot.spawnedFruits.Count > 0)
                        {
                            var currentGrowingFruit = plot.spawnedFruits[^1];
                            if (currentGrowingFruit != null)
                            {
                                float currentFruitTimer =
                                    plot.growTimer - (plot.spawnedFruits.Count - 1) * fruitSpawnInterval;
                                float progress = Mathf.Clamp01(currentFruitTimer / fruitSpawnInterval);
                                currentGrowingFruit.transform.localScale = Vector3.one * progress;
                            }
                        }

                        for (int j = 0; j < plot.spawnedFruits.Count - 1; j++)
                        {
                            if (plot.spawnedFruits[j] != null)
                            {
                                plot.spawnedFruits[j].transform.localScale = Vector3.one;
                            }
                        }

                        if (plot.growTimer >= growthTime)
                        {
                            plot.growTimer = 0f;
                            plot.readyFruits = 3;
                            plot.state = PlotState.Ready;

                            foreach (var fruit in plot.spawnedFruits)
                            {
                                if (fruit != null) fruit.transform.localScale = Vector3.one;
                            }
                        }
                    }
                    else
                    {
                        plot.growTimer = 0f;
                    }
                }
            }
        }

        public void BuildSelected()
        {
            if (_selectedPlot == null || _selectedPlot.built || _selectedPlot.isUnlocking) return;

            var unlockDuration = farmManager.BuildUnlockSeconds;
            if (_selectedPlot.constructionData != null)
            {
                unlockDuration = _selectedPlot.constructionData.unlockSeconds;
            }

            _selectedPlot.isUnlocking = true;
            _selectedPlot.state = PlotState.Unlocking;
            _selectedPlot.unlockTimer = Mathf.Max(0.5f, unlockDuration);
            if (_selectedPlot.boxAnimation != null) _selectedPlot.boxAnimation.Play("BoxOpen");
            farmManager.UIManager.ShowCountdown(_selectedPlot);
            farmManager.UIManager.RefreshSelectionViews();
        }

        public void UpgradeSelected()
        {
            if (_selectedPlot is not { built: true }) return;
            if (_selectedPlot.level >= 10) return;

            _selectedPlot.level += 1;
            farmManager.UIManager.UpdateInformation(_selectedPlot);
            farmManager.UIManager.RefreshSelectionViews();
        }

        public void BoostSelectedPlantX2()
        {
            if (_selectedPlot is not { built: true }) return;
            _selectedPlot.localProfitMultiplier *= 2f;
        }

        private void SpawnFruitAtPoint(Plot plot, int fruitIndex)
        {
            if (plot.constructionView != null && plot.constructionView.fruitPoints != null &&
                fruitIndex < plot.constructionView.fruitPoints.Length)
            {
                var point = plot.constructionView.fruitPoints[fruitIndex];
                if (point != null)
                {
                    var fruit = Instantiate(plantPrefab, point.position, point.rotation, point);
                    fruit.transform.localScale = Vector3.zero;
                    plot.spawnedFruits.Add(fruit);
                }
            }
        }

        public void HandleClickSelection()
        {
            if (!Input.GetMouseButtonDown(0) || farmManager.MainCamera == null) return;

            var ray = farmManager.MainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 200f)) return;

            var clickable = hit.collider.GetComponentInParent<FarmClickable>();
            if (clickable == null || clickable.plotIndex < 0 || clickable.plotIndex >= _plots.Count) return;

            _selectedPlot = _plots[clickable.plotIndex];
            farmManager.UIManager.RefreshSelectionViews();
        }
    }
}