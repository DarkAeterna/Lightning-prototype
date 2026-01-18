using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class BuildingGiver : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup _group;
        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private Building[] _buildings;
        
        [SerializeField] private BuildingSpawner _buildingSpawner;

        public event Action<Building> BuildingSelected;
    
        private readonly List<Button> _spawnedButtons = new List<Button>();
        
        private Building _currentBuilding;
        
        public Building CurrentBuilding => _currentBuilding;

        private void Awake()
        {
            Show();
        }

        private void OnEnable()
        {
            BuildingSelected += Reload;
            _buildingSpawner.Spawned += ResetSelection;
        }

        private void OnDisable()
        {
            BuildingSelected -= Reload;
            _buildingSpawner.Spawned -= ResetSelection;
        }

        [ContextMenu("Show")]
        public void Show()
        {
            Building[] buildingsToShow = Choose(Random.Range(1, 4 + 1));
            gameObject.SetActive(true);

            ClearButtons();

            for (int i = 0; i < buildingsToShow.Length; i++)
            {
                Building building = buildingsToShow[i];

                Button buttonInstance = Instantiate(_buttonPrefab, _group.transform);
                _spawnedButtons.Add(buttonInstance);

                Text label = buttonInstance.GetComponentInChildren<Text>(true);
            
                if (label != null)
                {
                    label.text = GetBuildingDisplayName(building);
                }

                buttonInstance.onClick.AddListener(() =>
                {
                    BuildingSelected?.Invoke(building);
                });
            }
        }
    
        private void ClearButtons()
        {
            for (int i = 0; i < _spawnedButtons.Count; i++)
            {
                Button button = _spawnedButtons[i];
            
                if (button == null)
                {
                    continue;
                }

                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            _spawnedButtons.Clear();
        }

        private Building[] Choose(int count)
        {
            Building[] buildings = new Building[count];

            for (int i = 0; i < buildings.Length; i++)
            {
                buildings[i] =  _buildings[Random.Range(0, _buildings.Length)];
            }
        
            return buildings;
        }
        
        private void ResetSelection()
        {
            _currentBuilding = null;
        }

        private void Reload(Building building)
        {
            _currentBuilding = building;
            Show();
        }
    
        private string GetBuildingDisplayName(Building building)
        {
            if (building == null)
            {
                return "Unknown";
            }
        
            return building.name;
        }
    }
}
