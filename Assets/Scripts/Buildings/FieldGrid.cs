using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class FieldGrid : MonoBehaviour
    {
        [Header("Field bounds")]
        [Tooltip("Коллайдер зоны, внутри которой можно строить. IsTrigger может быть true.")]
        [SerializeField] private Collider2D _areaCollider;

        [Header("Overlap")]
        [Tooltip("Слои, в которых находятся коллайдеры уже построенных зданий.")]
        [SerializeField] private LayerMask _buildingsMask = ~0;

        private readonly List<Building> _placedBuildings = new List<Building>();

        private void Awake()
        {
            if (_areaCollider == null)
            {
                _areaCollider = GetComponent<Collider2D>();
            }
        }

        public bool CanPlace(Building buildingPrefab, Vector3 worldPosition, float rotationZ, float extraSeparation)
        {
            if (buildingPrefab == null)
            {
                return false;
            }

            if (_areaCollider == null)
            {
                return false;
            }

            if (!TryGetPrefabBox(buildingPrefab, out BoxCollider2D prefabBox))
            {
                return false;
            }

            if (!IsBoxInsideArea(prefabBox, worldPosition, rotationZ))
            {
                return false;
            }

            if (IsOverlapping(prefabBox, worldPosition, rotationZ, extraSeparation))
            {
                return false;
            }

            return true;
        }

        public bool TryPlace(Building buildingPrefab, Vector3 worldPosition, float rotationZ, float extraSeparation, out Building placedBuilding)
        {
            placedBuilding = null;

            if (!CanPlace(buildingPrefab, worldPosition, rotationZ, extraSeparation))
            {
                return false;
            }

            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

            Building instance = Instantiate(buildingPrefab, transform);
            instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, instance.transform.position.z);
            instance.transform.rotation = rotation;

            _placedBuildings.Add(instance);
            placedBuilding = instance;

            return true;
        }

        public IReadOnlyList<Building> GetPlacedBuildings()
        {
            return _placedBuildings;
        }

        private bool TryGetPrefabBox(Building buildingPrefab, out BoxCollider2D box)
        {
            box = buildingPrefab.GetComponentInChildren<BoxCollider2D>(true);
            return box != null;
        }

        private bool IsOverlapping(BoxCollider2D prefabBox, Vector3 worldPosition, float rotationZ, float extraSeparation)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

            Vector2 scaledSize = GetScaledSize(prefabBox);
            Vector2 sizeWithSeparation = scaledSize + Vector2.one * extraSeparation * 2.0f;

            Vector2 center = GetWorldCenter(prefabBox, worldPosition, rotation);

            // Важно: OverlapBox вернет и коллайдеры "preview", если они включены.
            // Мы preview будем делать без коллайдеров — тогда всё ок.
            Collider2D hit = Physics2D.OverlapBox(center, sizeWithSeparation, rotationZ, _buildingsMask);
            return hit != null;
        }

        private bool IsBoxInsideArea(BoxCollider2D prefabBox, Vector3 worldPosition, float rotationZ)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

            Vector2 scaledSize = GetScaledSize(prefabBox);
            Vector2 half = scaledSize * 0.5f;

            Vector2 center = GetWorldCenter(prefabBox, worldPosition, rotation);

            Vector2[] corners =
            {
                center + (Vector2)(rotation * new Vector3(-half.x, -half.y, 0.0f)),
                center + (Vector2)(rotation * new Vector3( half.x, -half.y, 0.0f)),
                center + (Vector2)(rotation * new Vector3( half.x,  half.y, 0.0f)),
                center + (Vector2)(rotation * new Vector3(-half.x,  half.y, 0.0f))
            };

            for (int i = 0; i < corners.Length; i++)
            {
                if (!_areaCollider.OverlapPoint(corners[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private Vector2 GetScaledSize(BoxCollider2D box)
        {
            Vector3 lossyScale = box.transform.lossyScale;

            return new Vector2(
                box.size.x * Mathf.Abs(lossyScale.x),
                box.size.y * Mathf.Abs(lossyScale.y)
            );
        }

        private Vector2 GetWorldCenter(BoxCollider2D prefabBox, Vector3 worldPosition, Quaternion rotation)
        {
            // prefabBox.offset задан в локальных координатах объекта с коллайдером.
            // Для проверки размещения мы предполагаем, что корень префаба совпадает с pivot размещения.
            // Поэтому offset просто поворачиваем и прибавляем к worldPosition.
            Vector2 offset = prefabBox.offset;
            Vector2 rotatedOffset = (Vector2)(rotation * (Vector3)offset);

            return new Vector2(worldPosition.x, worldPosition.y) + rotatedOffset;
        }
    }
}