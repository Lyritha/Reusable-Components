using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        EnemyManager.OnAllEnemiesDefeated += Win;

        // for now only handle one player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.gameObject.TryGetComponent(out HealthSystem health))
        {
            health.OnDepleted.AddListener(Lose);
        }

    }

    private void OnDestroy()
    {
        EnemyManager.OnAllEnemiesDefeated -= Win;

        // for now only handle one player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.gameObject.TryGetComponent(out HealthSystem health))
        {
            health.OnDepleted.RemoveListener(Lose);
        }
    }

    public void Win() => SceneManager.LoadScene(3);
    public void Lose() => SceneManager.LoadScene(2);
}
