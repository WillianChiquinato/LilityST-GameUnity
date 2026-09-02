using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public GameObject[] spawnedEnemies;
    public float spawnInterval = 1f;
    [Min(0)] public int maxEnemiesToSpawn = 2;
    public ArenaController arenaController;

    private bool isSpawnLoopRunning;
    private bool wasSpawnAuthorized;
    private int spawnedEnemiesCount;

    void Awake()
    {
        arenaController = GetComponent<ArenaController>();

        if (spawnedEnemies == null || spawnedEnemies.Length < maxEnemiesToSpawn)
            System.Array.Resize(ref spawnedEnemies, maxEnemiesToSpawn);
    }

    void Update()
    {
        if (arenaController == null)
            return;

        if (!arenaController.AuthorizeSpawn)
        {
            if (isSpawnLoopRunning)
                StopSpawnLoop();

            if (wasSpawnAuthorized)
                spawnedEnemiesCount = 0;

            wasSpawnAuthorized = false;
            return;
        }

        wasSpawnAuthorized = true;

        if (spawnedEnemiesCount >= maxEnemiesToSpawn)
        {
            StopSpawnLoop();
            arenaController.NotifySpawningFinished();
            return;
        }

        if (!isSpawnLoopRunning)
        {
            InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
            isSpawnLoopRunning = true;
        }
    }

    private void OnDisable()
    {
        StopSpawnLoop();
    }

    private void StopSpawnLoop()
    {
        if (IsInvoking(nameof(SpawnEnemy)))
            CancelInvoke(nameof(SpawnEnemy));

        isSpawnLoopRunning = false;
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
            return;

        if (spawnedEnemiesCount >= maxEnemiesToSpawn)
        {
            StopSpawnLoop();
            return;
        }

        int spawnPointIndex = spawnedEnemiesCount % spawnPoints.Length;
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);

        GameObject spawnedEnemy = Instantiate(
            enemyPrefabs[randomEnemyIndex],
            spawnPoints[spawnPointIndex].position,
            Quaternion.identity);

        spawnedEnemies[spawnedEnemiesCount] = spawnedEnemy;
        spawnedEnemiesCount++;

        if (arenaController != null)
        {
            spawnedEnemy.transform.SetParent(arenaController.transform);
            arenaController.RegisterArenaEnemy(spawnedEnemy);
        }

        if (spawnedEnemiesCount >= maxEnemiesToSpawn)
        {
            StopSpawnLoop();
            arenaController.NotifySpawningFinished();
        }
    }
}
