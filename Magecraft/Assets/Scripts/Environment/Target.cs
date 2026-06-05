using TMPro;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [SerializeField]
    private TMP_Text ScoreText;
    [SerializeField]
    private float targetSize = 1f;

    private int score;

    public void TakeDamage(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        CalculateScore(hitPoint);
    }

    private void CalculateScore(Vector3 hitPoint)
    {
        float distance = Vector3.Distance(transform.position, hitPoint);


        score = distance <= targetSize * 0.25f ? 100 :
                distance <= targetSize * 0.5f ? 50 :
                distance <= targetSize * 0.75f ? 25 : 10;

        ScoreText.text = $"Score: {score}";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetSize);
    }
}
