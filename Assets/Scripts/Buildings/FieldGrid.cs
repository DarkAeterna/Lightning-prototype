using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class FieldGrid : MonoBehaviour
    {
        [Header("Grid settings")]
        [SerializeField]
        private float _cellSize = 1.0f;

        [SerializeField, Min(1)] private int _width = 10;
        [SerializeField, Min(1)] private int _height = 10;
        
        private Building[,] _field;

        private void Awake()
        {
            _field = new Building[_height, _width];
        }

        public bool TrySpawn(Building buildingPrefab, Vector2Int position)
        {
            if (buildingPrefab == null)
            {
                return false;
            }

            Vector3 localPosition = TranslateCellToLocalCenter(position);
            Quaternion rotation = Quaternion.identity;

            Building building = Instantiate(buildingPrefab, transform);
            building.transform.localPosition = localPosition;
            building.transform.localRotation = rotation;

            _field[position.y, position.x] = building;
            return true;
        }

        public bool IsInside(Vector3 worldPoint)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPoint);

            float halfWidth  = _width * _cellSize * 0.5f;
            float halfHeight = _height * _cellSize * 0.5f;

            return localPosition.x >= -halfWidth &&
                   localPosition.x <  halfWidth &&
                   localPosition.y >= -halfHeight &&
                   localPosition.y <  halfHeight;
        }
        
        public bool TryGetCellFromWorldPosition(Vector3 worldPosition, out Vector2Int cell)
        {
            cell = default;

            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

            float halfWidth  = _width * _cellSize * 0.5f;
            float halfHeight = _height * _cellSize * 0.5f;

            float shiftedX = localPosition.x + halfWidth;
            float shiftedY = localPosition.y + halfHeight;

            int x = Mathf.FloorToInt(shiftedX / _cellSize);
            int y = Mathf.FloorToInt(shiftedY / _cellSize);

            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return false;
            }

            cell = new Vector2Int(x, y);
            return true;
        }

        private Vector3 TranslateCellToLocalCenter(Vector2Int cell)
        {
            float halfWidth  = _width * _cellSize * 0.5f;
            float halfHeight = _height * _cellSize * 0.5f;

            float x = (cell.x + 0.5f) * _cellSize - halfWidth;
            float y = (cell.y + 0.5f) * _cellSize - halfHeight;

            return new Vector3(x, y, 0.0f);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            if (_width <= 0 || _height <= 0 || _cellSize <= 0.0f)
            {
                return;
            }

            float halfWidth  = _width * _cellSize * 0.5f;
            float halfHeight = _height * _cellSize * 0.5f;

            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;

            Vector3 bottomLeft  = new Vector3(-halfWidth, -halfHeight, 0.0f);
            Vector3 bottomRight = new Vector3( halfWidth, -halfHeight, 0.0f);
            Vector3 topRight    = new Vector3( halfWidth,  halfHeight, 0.0f);
            Vector3 topLeft     = new Vector3(-halfWidth,  halfHeight, 0.0f);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);

            Gizmos.matrix = previousMatrix;
        }
    }
}