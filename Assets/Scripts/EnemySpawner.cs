
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float enemySpawnRate = 2.0f;
    [SerializeField] private float SpawnPosX = 5.0f;
    [SerializeField] private float minSpawnPosY = -0.75f;
    [SerializeField] private float maxSpawnPosY = 6.0f;
    [SerializeField] private GameObject enemyPrefab;
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0, enemySpawnRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, new Vector3(SpawnPosX, Random.Range(minSpawnPosY, maxSpawnPosY), 0), enemyPrefab.transform.rotation);
    }
}
