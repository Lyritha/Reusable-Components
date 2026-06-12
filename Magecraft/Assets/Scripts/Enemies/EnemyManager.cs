using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    public UnityEvent OnAllEnemiesDefeated;

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
            int randomEnemyCount = Random.Range(1, 4);
            spawnedEnemies += randomEnemyCount;

            spawner.Spawn(randomEnemyCount);
        }
    }

    public void RemoveEnemy()
    {
        spawnedEnemies -= 1;

        if (spawnedEnemies <= 0)
        {
            OnAllEnemiesDefeated?.Invoke();

            Debug.Log("won  ");
        }
    }
}
