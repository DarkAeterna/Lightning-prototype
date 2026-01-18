
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRate = 2.0f;
    [SerializeField] private float _spawnPositionX = 5.0f;
    [SerializeField] private float _minSpawnPositionY = -0.75f;
    [SerializeField] private float _maxSpawnPositionY = 6.0f;
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] public int _waveCounter = 1;
    [SerializeField] public bool _isEnemyPhase;
    void Start()
    {
        StartCoroutine(SpawnNext());
    }

    private void Spawn()
    {
        if(_enemyPrefabs == null)
        {
            Debug.LogError("Enemy prefab is empty");
        }

        for(int i = 0; i < _waveCounter; i++)
        {
            int randomIndex = Random.Range(0, _enemyPrefabs.Length);
            Vector3 _randomSpawnPos = new Vector3(_spawnPositionX, Random.Range(_minSpawnPositionY, _maxSpawnPositionY), 0);
            Instantiate(_enemyPrefabs[randomIndex], _randomSpawnPos, Quaternion.identity);
        }
    }
    private IEnumerator SpawnNext()
    {
        while(_isEnemyPhase)
        {
            Spawn();
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }
}
