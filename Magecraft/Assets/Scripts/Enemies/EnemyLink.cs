using UnityEngine;

public class EnemyLink : MonoBehaviour
{
    private void Start()
    {
        if (EnemyManager.Instance != null) EnemyManager.Instance.AddEnemy();
    }

    public void Notify()
    {
        if (EnemyManager.Instance != null)  EnemyManager.Instance.RemoveEnemy();
    }
}
