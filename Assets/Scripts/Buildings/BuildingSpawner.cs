using System;
using Logic;
using UI;
using UnityEngine;

namespace Buildings
{
    public class BuildingSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private FieldGrid _fieldModel;
        [SerializeField] private FieldGridView _fieldView;
        [SerializeField] private BuildingGiver _buildingGiver;

        [Header("Placement plane")]
        [SerializeField] private float _planeZ = 0.0f;

        [Header("Rotation")]
        [SerializeField] private KeyCode _rotateKey = KeyCode.R;
        [SerializeField] private float _rotationStepDegrees = 90.0f;

        private Camera _camera;
        private float _currentRotationZ;

        private bool _wasMousePressed;

        public event Action Spawned;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputSystem.MouseClicked += OnMouseClicked;
        }

        private void OnDisable()
        {
            _inputSystem.MouseClicked -= OnMouseClicked;
        }

        private void Update()
        {
            // Поворот preview (без изменения InputSystem)
            if (Input.GetKeyDown(_rotateKey))
            {
                _currentRotationZ += _rotationStepDegrees;
                _currentRotationZ = NormalizeAngle(_currentRotationZ);
            }

            // Обновляем preview каждый кадр, даже если мышь не нажата:
            Building building = _buildingGiver.CurrentBuilding;
            _fieldView.SetPreviewBuilding(building);

            if (TryGetMouseWorldPointOnPlane(_planeZ, out Vector3 worldPoint))
            {
                _fieldView.SetPreviewPose(worldPoint, _currentRotationZ);
            }
            else
            {
                _fieldView.ClearPreviewPose();
            }

            // Сброс антиспама: как только кнопку отпустили — можно снова ставить
            bool isPressed = Input.GetMouseButton(0);
            if (!isPressed)
            {
                _wasMousePressed = false;
            }
        }

        private void OnMouseClicked(Vector3 mousePosition)
        {
            // InputSystem вызывает событие каждый кадр удержания, поэтому:
            // спавним только при первом кадре нажатия.
            if (_wasMousePressed)
            {
                return;
            }

            _wasMousePressed = true;

            Building building = _buildingGiver.CurrentBuilding;
            if (building == null)
            {
                return;
            }

            if (!TryGetMouseWorldPointOnPlane(_planeZ, out Vector3 worldPoint))
            {
                return;
            }

            float separation = _fieldView != null ? _fieldView.GetExtraSeparation() : 0.0f;

            if (_fieldModel.TryPlace(building, worldPoint, _currentRotationZ, separation, out _))
            {
                Spawned?.Invoke();
            }
        }

        private bool TryGetMouseWorldPointOnPlane(float planeZ, out Vector3 worldPoint)
        {
            worldPoint = default;

            Plane plane = new Plane(Vector3.forward, new Vector3(0.0f, 0.0f, planeZ));
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out float enter))
            {
                return false;
            }

            worldPoint = ray.GetPoint(enter);
            worldPoint.z = planeZ;
            return true;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle >= 360.0f)
            {
                angle -= 360.0f;
            }

            while (angle < 0.0f)
            {
                angle += 360.0f;
            }

            return angle;
        }
    }
}