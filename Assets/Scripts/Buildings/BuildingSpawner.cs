using System;
using Logic;
using UnityEngine;

namespace Buildings
{
    public class BuildingSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private FieldGrid _fieldGrid;
        [SerializeField] private BuildingGiver _buildingGiver;

        private Camera _camera;

        public event Action Spawned;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputSystem.MouseClicked += Spawn;
        }

        private void OnDisable()
        {
            _inputSystem.MouseClicked -= Spawn;
        }

        private void Spawn(Vector3 mousePosition)
        {
            Building building = _buildingGiver.CurrentBuilding;

            if (TryGetMouseWorldPointOnPlane(_fieldGrid.transform.position.z, out Vector3 worldPoint) == false)
            {
                return;
            }

            bool isMouseInsideField = _fieldGrid.IsInside(worldPoint);

            if (isMouseInsideField == false)
            {
                return;
            }

            bool canGetCell = _fieldGrid.TryGetCellFromWorldPosition(worldPoint, out Vector2Int cell);

            if (canGetCell == false)
            {
                return;
            }

            _fieldGrid.TrySpawn(building, cell);
            Spawned?.Invoke();
        }

        private bool TryGetMouseWorldPointOnPlane(float planeZ, out Vector3 worldPoint)
        {
            worldPoint = default;

            Plane plane = new Plane(Vector3.forward, new Vector3(0.0f, 0.0f, planeZ));
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float enter) == false)
            {
                return false;
            }

            worldPoint = ray.GetPoint(enter);
            return true;
        }
    }
}