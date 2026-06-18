using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private int minEnemies = 1;
    [SerializeField]
    private int maxEnemies = 10;

    // Static hooks
    public static event Action OnAllEnemiesDefeated;
    public static event Action<int> OnEnemyCountChanged;

    private EnemySpawner[] spawners;

    private int spawnedEnemies = 0;

    private void OnEnable() => EnemySpawner.AllInstancesReady += TriggerWave;
    private void OnDisable() => EnemySpawner.AllInstancesReady -= TriggerWave;


    [ContextMenu("start")]
    public void TriggerWave()
    {
        spawners = EnemySpawner.AllInstances;

        foreach (EnemySpawner spawner in spawners)
        {
            int randomEnemyCount = Random.Range(minEnemies, maxEnemies);
            spawnedEnemies += randomEnemyCount;

            spawner.Spawn(randomEnemyCount);
            OnEnemyCountChanged?.Invoke(spawnedEnemies);
        }
    }

    public void RemoveEnemy()
    {
        spawnedEnemies -= 1;

        OnEnemyCountChanged?.Invoke(spawnedEnemies);
        if (spawnedEnemies <= 0) OnAllEnemiesDefeated?.Invoke();
    }

    public int EnemyCount => spawnedEnemies;
}
