using Farms.UI;
using Farms.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private FarmSimulationManager farmManager;

        [Header("UI Prefabs")] [SerializeField]
        private GameObject buildPrefab;

        [SerializeField] private GameObject constructionBuildViewPrefab;
        [SerializeField] private GameObject constructionUpgradeViewPrefab;
        [SerializeField] private GameObject upgradeViewPrefab;
        [SerializeField] private GameObject mainViewPrefab;
        [SerializeField] private GameObject informationPrefab;

        public GameObject BuildPrefab => buildPrefab;
        public GameObject ConstructionBuildViewPrefab => constructionBuildViewPrefab;
        public GameObject ConstructionUpgradeViewPrefab => constructionUpgradeViewPrefab;
        public GameObject UpgradeViewPrefab => upgradeViewPrefab;
        public GameObject MainViewPrefab => mainViewPrefab;
        public GameObject InformationPrefab => informationPrefab;

        private GameObject _buildViewInstance;
        private GameObject _upgradeViewInstance;
        private GameObject _managementViewInstance;
        [ShowInInspector, ReadOnly] private MainView _mainView;
        public MainView MainView => _mainView;

        public UpgradeView UpgradeView => _managementViewInstance?.GetComponent<UpgradeView>();


        public void Initialize()
        {
            SetupViews();
        }

        public void UpdateManager()
        {
            UpdateSelectionViewPosition();
            UpdateCountdownPositions();
            UpdateInformationPositions();
        }

        public void ShowInformation(Plot plot)
        {
            if (plot == null || informationPrefab == null || farmManager.UiCanvas == null) return;
            if (plot.informationView != null) return;

            var obj = Instantiate(informationPrefab, farmManager.UiCanvas.transform);

            plot.informationView = obj.GetComponent<Farms.UI.InformationView>();
            UpdateInformation(plot);

            Vector3 worldPos;
            if (plot.constructionView != null && plot.constructionView.infoPoint != null)
            {
                worldPos = plot.constructionView.infoPoint.position;
            }
            else
            {
                worldPos = plot.constructionObject != null
                    ? plot.constructionObject.transform.position + Vector3.up * 2f
                    : Vector3.zero;
            }

            ShowInfoAtWorld(plot.informationView.gameObject, worldPos);
        }

        public void UpdateInformation(Plot plot)
        {
            if (plot?.informationView == null) return;

            var ratePerSecond = 1f / farmManager.GrowthSeconds;
            var baseRate = ratePerSecond * 3600f;
            var hourlyRate = Mathf.FloorToInt(baseRate * plot.LevelProfitMultiplier);

            // Nếu có multiplier x2 từ Boost
            if (plot.localProfitMultiplier > 1.1f)
                hourlyRate = Mathf.FloorToInt(hourlyRate * plot.localProfitMultiplier);

            var prodName = string.Empty;
            Sprite icon = null;
            if (plot.constructionData != null)
            {
                prodName = plot.constructionData.constructionName;
                icon = plot.constructionData.constructionIcon;
            }

            plot.informationView.UpdateProduction(prodName, hourlyRate, icon);
        }

        private void UpdateInformationPositions()
        {
            var plots = farmManager.PlotManager.Plots;
            for (var i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                if (plot.built && plot.informationView != null)
                {
                    Vector3 worldPos;
                    if (plot.constructionView != null && plot.constructionView.infoPoint != null)
                    {
                        worldPos = plot.constructionView.infoPoint.position;
                    }
                    else
                    {
                        worldPos = plot.constructionObject != null
                            ? plot.constructionObject.transform.position + Vector3.up * 2f
                            : Vector3.zero;
                    }

                    PositionInformationViewAtWorld(plot.informationView.gameObject, worldPos);
                }
            }
        }

        private void PositionInformationViewAtWorld(GameObject viewRoot, Vector3 worldPosition)
        {
            RectTransform canvasRect = farmManager.UiCanvas.transform as RectTransform;
            if (canvasRect == null) return;

            RectTransform targetRect = viewRoot.transform as RectTransform;
            if (targetRect == null) return;

            Vector3 screenPoint =
                farmManager.MainCamera.WorldToScreenPoint(worldPosition);
            if (screenPoint.z <= 0f)
            {
                viewRoot.SetActive(false);
                return;
            }

            viewRoot.SetActive(true);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null,
                    out Vector2 localPoint))
            {
                targetRect.anchoredPosition = localPoint;
            }
        }

        public void ShowCountdown(Plot plot)
        {
            if (plot == null || buildPrefab == null || farmManager.UiCanvas == null) return;
            if (plot.countdownView != null) return;

            var obj = Instantiate(buildPrefab, farmManager.UiCanvas.transform);

            plot.countdownView = obj.GetComponent<UI.BuildCountdownView>();
            if (plot.countdownView == null)
            {
                plot.countdownView = obj.AddComponent<UI.BuildCountdownView>();
            }

            UpdateCountdownForPlot(plot);

            var worldPos = plot.boxObject != null ? plot.boxObject.transform.position : Vector3.zero;
            PositionCountdownViewAtWorld(plot.countdownView.gameObject, worldPos);
            UpdateSelectionViewPosition();
        }

        public void HideCountdown(Plot plot)
        {
            if (plot?.countdownView == null) return;
            Destroy(plot.countdownView.gameObject);
            plot.countdownView = null;
        }

        private void UpdateCountdownPositions()
        {
            var plots = farmManager.PlotManager.Plots;
            for (int i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                if (plot.isUnlocking && plot.countdownView != null)
                {
                    UpdateCountdownForPlot(plot);

                    Vector3 worldPos;
                    if (plot.boxObject != null)
                    {
                        worldPos = plot.boxObject.transform.position;
                    }
                    else if (plot.constructionView != null && plot.constructionView.buildPoint != null)
                    {
                        worldPos = plot.constructionView.buildPoint.position;
                    }
                    else
                    {
                        worldPos = Vector3.zero;
                    }

                    PositionCountdownViewAtWorld(plot.countdownView.gameObject, worldPos);
                }
            }
        }

        private void PositionCountdownViewAtWorld(GameObject viewRoot, Vector3 worldPosition)
        {
            RectTransform canvasRect = farmManager.UiCanvas.transform as RectTransform;
            if (canvasRect == null) return;

            Transform frameTransform = FindChildByName(viewRoot.transform, "Frame");
            RectTransform targetRect = (frameTransform != null ? frameTransform : viewRoot.transform) as RectTransform;
            if (targetRect == null) return;

            Vector3 screenPoint =
                farmManager.MainCamera.WorldToScreenPoint(worldPosition);
            if (screenPoint.z <= 0f)
            {
                viewRoot.SetActive(false);
                return;
            }

            viewRoot.SetActive(true);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null,
                    out Vector2 localPoint))
            {
                targetRect.anchoredPosition = localPoint;
            }
        }

        private void ShowInfoAtWorld(GameObject viewRoot, Vector3 worldPosition)
        {
            RectTransform canvasRect = farmManager.UiCanvas.transform as RectTransform;
            if (canvasRect == null) return;

            RectTransform targetRect = viewRoot.transform as RectTransform;
            if (targetRect == null) return;

            Vector3 screenPoint =
                farmManager.MainCamera.WorldToScreenPoint(worldPosition + farmManager.SelectionViewWorldOffset);
            if (screenPoint.z <= 0f) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null,
                    out Vector2 localPoint))
            {
                targetRect.anchoredPosition = localPoint + farmManager.SelectionViewScreenOffset;
            }
        }

        private void UpdateCountdownForPlot(Plot plot)
        {
            if (plot.countdownView == null) return;
            var total = plot.constructionData != null
                ? plot.constructionData.unlockSeconds
                : farmManager.BuildUnlockSeconds;
            plot.countdownView.UpdateTimer(plot.unlockTimer, total);
        }

        private void SetupViews()
        {
            var uiCanvas = farmManager.UiCanvas;
            if (uiCanvas == null)
            {
                uiCanvas = FindObjectOfType<Canvas>();
            }

            if (uiCanvas == null) return;

            if (constructionBuildViewPrefab != null)
            {
                _buildViewInstance = Instantiate(constructionBuildViewPrefab, uiCanvas.transform);
                ConfigureOverlayCanvas(_buildViewInstance);
                WireNamedButton(_buildViewInstance, "Button", farmManager.PlotManager.BuildSelected);
                WireNamedButton(_buildViewInstance, "Close", ClearSelection);
                _buildViewInstance.SetActive(false);
            }

            if (constructionUpgradeViewPrefab != null)
            {
                _upgradeViewInstance = Instantiate(constructionUpgradeViewPrefab, uiCanvas.transform);
                ConfigureOverlayCanvas(_upgradeViewInstance);
                WireNamedButton(_upgradeViewInstance, "Upgrade", farmManager.PlotManager.UpgradeSelected);
                WireNamedButton(_upgradeViewInstance, "Close", ClearSelection);
                _upgradeViewInstance.SetActive(false);
            }

            if (upgradeViewPrefab != null)
            {
                _managementViewInstance = Instantiate(upgradeViewPrefab, uiCanvas.transform);
                ConfigureOverlayCanvas(_managementViewInstance);
                WireNamedButton(_managementViewInstance, "Close", HideManagementView);
                HideManagementView();
            }

            if (mainViewPrefab != null)
            {
                var mainViewInstance = Instantiate(mainViewPrefab, uiCanvas.transform);
                ConfigureOverlayCanvas(mainViewInstance);
                _mainView = mainViewInstance.GetComponent<MainView>();
                if (_mainView != null)
                {
                    _mainView.Setup(farmManager.UpgradeManager.OpenUpgradeView);
                }
            }
        }

        public void UpdateMainViewCurrency()
        {
            if (_mainView != null)
            {
                _mainView.UpdateCash(farmManager.Cash);
                _mainView.UpdateGem(farmManager.Gem);
            }
        }

        // public void ShowManagementView()
        // {
        //     Debug.Log("ShowManagementView");
        //     if (_managementViewInstance != null)
        //     {
        //         _managementViewInstance.SetActive(true);
        //     }
        // }

        public void HideManagementView()
        {
            if (_managementViewInstance != null) _managementViewInstance.SetActive(false);
        }

        private static void ConfigureOverlayCanvas(GameObject viewRoot)
        {
            Canvas canvas = viewRoot.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = null;
                canvas.sortingOrder = 100;
            }

            var rect = viewRoot.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.anchoredPosition = Vector2.zero;
            }
        }

        private static void EnsureCanvasForWorldSpace(GameObject obj)
        {
            if (obj == null) return;
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = obj.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 50;

            // UI trong World Space cần được scale lại vì default UI thường rất to (1 unit = 1 pixel)
            // Nếu prefab đã có canvas và scale sẵn thì ta có thể giữ nguyên,
            // nhưng nếu vừa AddComponent thì mặc định scale là 1,1,1 nên sẽ cực kỳ to.
            if (obj.transform.localScale == Vector3.one)
            {
                obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }

            GraphicRaycaster raycaster = obj.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                obj.AddComponent<GraphicRaycaster>();
            }
        }

        private static void WireNamedButton(GameObject viewRoot, string buttonName,
            UnityEngine.Events.UnityAction action)
        {
            Transform target = FindChildByName(viewRoot.transform, buttonName);
            if (target == null) return;
            Button button = target.GetComponent<Button>();
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private static Transform FindChildByName(Transform root, string name)
        {
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindChildByName(root.GetChild(i), name);
                if (found != null) return found;
            }

            return null;
        }

        public void UpdateSelectionContent()
        {
            var selectedPlot = farmManager.PlotManager.SelectedPlot;
            if (selectedPlot == null) return;

            if (!selectedPlot.built)
            {
                UpdateBuildViewContent(selectedPlot);
            }
            else
            {
                UpdateUpgradeViewContent(selectedPlot);
            }
        }

        private void UpdateBuildViewContent(Plot plot)
        {
            if (_buildViewInstance == null) return;

            var data = _buildViewInstance.GetComponent<ConstructionBuildViewItem>();
            data.SetupView(plot.constructionData);
        }

        private void UpdateUpgradeViewContent(Plot plot)
        {
            if (_upgradeViewInstance == null) return;
            var data = _upgradeViewInstance.GetComponent<ConstructionUpgradeView>();

            data.SetupView(plot.level, plot.constructionData.constructionName, plot.GetCurrentProfit(),
                plot.GetCurrentGrowthTime(), plot.GetUpgradePrice(), plot.level >= 10);
        }

        private static void SetTextIfExists(GameObject viewRoot, string objectName, string content)
        {
            if (viewRoot == null) return;
            Transform node = FindChildByName(viewRoot.transform, objectName);
            if (node == null) return;
            TMP_Text text = node.GetComponent<TMP_Text>();
            if (text == null) return;
            text.text = content;
        }

        private static void SetImageSpriteIfExists(GameObject viewRoot, string objectName, Sprite sprite)
        {
            if (viewRoot == null || sprite == null) return;
            Transform node = FindChildByName(viewRoot.transform, objectName);
            if (node == null) return;
            Image image = node.GetComponent<Image>();
            if (image == null) return;
            image.sprite = sprite;
        }

        private static void SetButtonLabel(GameObject viewRoot, string buttonName, string content)
        {
            if (viewRoot == null) return;
            Transform buttonNode = FindChildByName(viewRoot.transform, buttonName);
            if (buttonNode == null) return;
            TMP_Text text = buttonNode.GetComponentInChildren<TMP_Text>(true);
            if (text == null) return;
            text.text = content;
        }

        private static void SetButtonInteractable(GameObject viewRoot, string buttonName, bool interactable)
        {
            if (viewRoot == null) return;
            Transform buttonNode = FindChildByName(viewRoot.transform, buttonName);
            if (buttonNode == null) return;
            Button button = buttonNode.GetComponent<Button>();
            if (button == null) return;
            button.interactable = interactable;
        }

        public void ClearSelection()
        {
            farmManager.PlotManager.SelectedPlot = null;
            RefreshSelectionViews();
        }

        public void RefreshSelectionViews()
        {
            var selectedPlot = farmManager.PlotManager.SelectedPlot;
            if (_buildViewInstance != null)
            {
                _buildViewInstance.SetActive(selectedPlot is { built: false, isUnlocking: false });
            }

            if (_upgradeViewInstance != null)
            {
                _upgradeViewInstance.SetActive(selectedPlot is { built: true });
            }

            UpdateSelectionContent();
            UpdateSelectionViewPosition();
        }

        public void UpdateSelectionViewPosition()
        {
            var selectedPlot = farmManager.PlotManager.SelectedPlot;
            if (selectedPlot == null || farmManager.MainCamera == null || farmManager.UiCanvas == null) return;

            var anchorWorldPos = Vector3.zero;
            if (selectedPlot.isUnlocking)
            {
                anchorWorldPos = selectedPlot.boxObject.transform.position;
            }
            else
            {
                anchorWorldPos = selectedPlot.boxObject.transform.position;
            }

            if (_buildViewInstance != null && _buildViewInstance.activeSelf)
            {
                PositionViewAtWorld(_buildViewInstance, anchorWorldPos);
            }

            if (_upgradeViewInstance != null && _upgradeViewInstance.activeSelf)
            {
                PositionViewAtWorld(_upgradeViewInstance, anchorWorldPos);
            }
        }

        private void PositionViewAtWorld(GameObject viewRoot, Vector3 worldPosition)
        {
            RectTransform canvasRect = farmManager.UiCanvas.transform as RectTransform;
            if (canvasRect == null) return;

            Transform frameTransform = FindChildByName(viewRoot.transform, "Frame");
            RectTransform frameRect = frameTransform as RectTransform;
            if (frameRect == null) return;

            Vector3 screenPoint =
                farmManager.MainCamera.WorldToScreenPoint(worldPosition + farmManager.SelectionViewWorldOffset);
            if (screenPoint.z <= 0f) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null,
                    out Vector2 localPoint))
            {
                frameRect.anchoredPosition = localPoint + farmManager.SelectionViewScreenOffset;
            }
        }
    }
}