using Buildings;
using UnityEngine;

namespace UI
{
    public class FieldGridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FieldGrid _model;

        [Header("Placement spacing (view setting)")]
        [SerializeField, Min(0.0f)] private float _extraSeparation = 0.02f;

        [Header("Preview visuals")]
        [SerializeField, Range(0.0f, 1.0f)] private float _previewAlpha = 0.55f;
        [SerializeField] private Color _validTint = new Color(0.35f, 1.0f, 0.35f, 1.0f);
        [SerializeField] private Color _invalidTint = new Color(1.0f, 0.35f, 0.35f, 1.0f);

        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _previewSortingOrder = 500;

        [SerializeField, Min(0.0f)] private float _previewZ = 0.0f;

        private Building _currentPrefab;
        private GameObject _previewInstance;
        private SpriteRenderer[] _previewRenderers;
        private bool _hasPreviewPoint;
        private Vector3 _previewPoint;
        private float _previewRotationZ;

        public float GetExtraSeparation()
        {
            return _extraSeparation;
        }

        private void Awake()
        {
            if (_model == null)
            {
                _model = GetComponent<FieldGrid>();
            }
        }

        private void LateUpdate()
        {
            RenderPreview();
        }

        public void SetPreviewBuilding(Building buildingPrefab)
        {
            if (ReferenceEquals(_currentPrefab, buildingPrefab))
            {
                return;
            }

            _currentPrefab = buildingPrefab;
            RecreatePreview();
        }

        public void ClearPreviewBuilding()
        {
            _currentPrefab = null;
            DestroyPreview();
        }

        public void SetPreviewPose(Vector3 worldPosition, float rotationZ)
        {
            _previewPoint = worldPosition;
            _previewPoint.z = _previewZ;

            _previewRotationZ = rotationZ;
            _hasPreviewPoint = true;
        }

        public void ClearPreviewPose()
        {
            _hasPreviewPoint = false;
        }

        private void RenderPreview()
        {
            if (_model == null || _currentPrefab == null || !_hasPreviewPoint)
            {
                if (_previewInstance != null)
                {
                    _previewInstance.SetActive(false);
                }

                return;
            }

            if (_previewInstance == null)
            {
                RecreatePreview();
            }

            if (_previewInstance == null)
            {
                return;
            }

            bool canPlace = _model.CanPlace(_currentPrefab, _previewPoint, _previewRotationZ, _extraSeparation);

            _previewInstance.SetActive(true);
            _previewInstance.transform.position = _previewPoint;
            _previewInstance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _previewRotationZ);

            ApplyTint(canPlace ? _validTint : _invalidTint);
        }

        private void RecreatePreview()
        {
            DestroyPreview();

            if (_currentPrefab == null)
            {
                return;
            }

            _previewInstance = Instantiate(_currentPrefab.gameObject);
            _previewInstance.name = $"{_currentPrefab.name}_Preview";
            _previewInstance.transform.SetParent(transform, true);

            DisablePreviewPhysicsAndBehaviours(_previewInstance);

            _previewRenderers = _previewInstance.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < _previewRenderers.Length; i++)
            {
                _previewRenderers[i].sortingLayerName = _sortingLayerName;
                _previewRenderers[i].sortingOrder = _previewSortingOrder;
            }

            _previewInstance.SetActive(false);
        }

        private void DestroyPreview()
        {
            if (_previewInstance != null)
            {
                Destroy(_previewInstance);
            }

            _previewInstance = null;
            _previewRenderers = null;
        }

        private void DisablePreviewPhysicsAndBehaviours(GameObject root)
        {
            Collider2D[] colliders = root.GetComponentsInChildren<Collider2D>(true);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            Rigidbody2D[] bodies = root.GetComponentsInChildren<Rigidbody2D>(true);
            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].simulated = false;
            }

            MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].enabled = false;
            }
        }

        private void ApplyTint(Color baseColor)
        {
            if (_previewRenderers == null)
            {
                return;
            }

            Color color = baseColor;
            color.a = _previewAlpha;

            for (int i = 0; i < _previewRenderers.Length; i++)
            {
                if (_previewRenderers[i] == null)
                {
                    continue;
                }

                _previewRenderers[i].color = color;
            }
        }
    }
}