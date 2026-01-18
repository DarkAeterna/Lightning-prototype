
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRate = 2.0f;
    [SerializeField] private float _spawnPosX = 5.0f;
    [SerializeField] private float _minSpawnPosY = -0.75f;
    [SerializeField] private float _maxSpawnPosY = 6.0f;
    [SerializeField] private GameObject [] _enemyPrefab;
    [SerializeField] public int _waveCount = 1;
    [SerializeField] public bool _isEnemyPhase;
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnEnemy()
    {
        if(_enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is empty");
        }
        for(int i = 0; i < _waveCount; i++)
        {
            int _randomIndex = Random.Range(0, _enemyPrefab.Length);
            Vector3 _randomSpawnPos = new Vector3(_spawnPosX, Random.Range(_minSpawnPosY, _maxSpawnPosY), 0);
            Instantiate(_enemyPrefab[_randomIndex], _randomSpawnPos, Quaternion.identity);
        }
    }
    private IEnumerator SpawnRoutine()
    {
        while(_isEnemyPhase)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }
}
