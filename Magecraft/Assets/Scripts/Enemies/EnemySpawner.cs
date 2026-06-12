using System.Collections;
using UnityEngine;

[Fold]
[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : SingletonGroup<EnemySpawner>
{
    [Header("Spawn Settings")]
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private float enemyRadius = 0.5f;
    [SerializeField] private int maxAttemptsPerEnemy = 20;
    [SerializeField] private float retryDelay = 0.2f;

    private BoxCollider col;

    private void OnValidate()
    {
        if (TryGetComponent(out col)) col.isTrigger = true;
    }

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out col);
    }

    public void Spawn(int count) => StartCoroutine(SpawnRoutine(count));

    private IEnumerator SpawnRoutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool found = TryFindValidSpawnPoint(out Vector3 pos);
            while (!found)
            {
                yield return new WaitForSeconds(retryDelay);
                found = TryFindValidSpawnPoint(out pos);
            }

            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }

    private bool TryFindValidSpawnPoint(out Vector3 result)
    {
        result = Vector3.zero;

        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++)
        {
            Vector3 pos = GetRandomPointInCollider(col);
            pos = GetGroundPoint(pos);

            if (IsPositionFree(pos))
            {
                result = pos;
                return true;
            }
        }

        return false;
    }

    private bool IsPositionFree(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, enemyRadius);
        foreach (Collider collider in colliders) if (collider.TryGetComponent(out EnemyController _)) return false;
        return true;
    }

    private Vector3 GetRandomPointInCollider(BoxCollider c)
    {
        Vector3 halfsize = new(c.size.x * 0.5f, c.size.y * 0.5f, c.size.z * 0.5f);
        Vector3 local = new Vector3(Random.Range(-halfsize.x, halfsize.x), halfsize.y, Random.Range(-halfsize.z, halfsize.z)) + c.center;
        return c.transform.TransformPoint(local);
    }

    private Vector3 GetGroundPoint(Vector3 pos)
    {
        Vector3 rayStart = pos;
        Vector3 rayDir = -col.transform.up;

        if (Physics.Raycast(rayStart, rayDir, out RaycastHit hit, col.size.y * 2f)) return hit.point;

        return pos;
    }
}
