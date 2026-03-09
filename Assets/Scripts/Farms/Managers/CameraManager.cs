using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farms.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [FoldoutGroup("Camera Setup")] [SerializeField]
        protected Vector3 cameraPosition = new Vector3(0f, 18f, -10.5f);

        [FoldoutGroup("Camera Setup")] [SerializeField]
        protected Vector3 cameraEuler = new Vector3(62f, 0f, 0f);

        [FoldoutGroup("Camera Setup"), SerializeField]
        protected float cameraFov = 36f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                _mainCamera = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
        }

        private void Start()
        {
            SetupCamera();
        }

        private void SetupCamera()
        {
            _mainCamera.transform.position = cameraPosition;
            _mainCamera.transform.rotation = Quaternion.Euler(cameraEuler);
            _mainCamera.fieldOfView = cameraFov;
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = new Color(0.73f, 0.92f, 0.52f);
        }
    }
}