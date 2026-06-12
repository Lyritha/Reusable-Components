using UnityEngine;

public class EnemyLink : MonoBehaviour
{
    public void Notify()
    {
        if (EnemyManager.Instance != null)  EnemyManager.Instance.RemoveEnemy();
    }
}
