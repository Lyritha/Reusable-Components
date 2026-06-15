using TMPro;
using UnityEngine;

public class GameStatusDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text gameStatusText;

    private void Start()
    {
        EnemyManager.OnEnemyCountChanged += OnEnemyCountChanged;
    }

    private void OnEnemyCountChanged(int count)
    {
        gameStatusText.text = $"Objective: Kill all enemies\r\nEnemies left: {count}";
    }
}
