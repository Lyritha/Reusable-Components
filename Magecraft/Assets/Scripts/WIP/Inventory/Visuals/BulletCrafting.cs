using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCrafting : MonoBehaviour
{
    [SerializeField]
    private TMP_Text behaviourText;

    [SerializeField]
    private Bullet resultBullet;

    [SerializeField]
    private List<TypedTrigger<Rune>> slots = new();

    private void OnEnable()
    {
        foreach (TypedTrigger<Rune> trigger in slots)
        {
            trigger.OnTriggerEntered += AddBulletBehavior;
            trigger.OnTriggerExited += RemoveBulletBehavior;
        }
    }

    private void OnDisable()
    {
        foreach (TypedTrigger<Rune> trigger in slots)
        {
            trigger.OnTriggerEntered -= AddBulletBehavior;
            trigger.OnTriggerExited -= RemoveBulletBehavior;
        }
    }

    private void Awake()
    {
        resultBullet = new();
        UpdateUI();
    }

    public void AddBulletBehavior(Rune rune)
    {
        resultBullet.AddModifier(rune.Modifier);
        UpdateUI();
    }

    public void RemoveBulletBehavior(Rune rune)
    {
        resultBullet.RemoveModifier(rune.Modifier);
        UpdateUI();
    }



    private void UpdateUI()
    {
        string text = resultBullet.GetBulletInfo();
        behaviourText.text = text;
    }
}
