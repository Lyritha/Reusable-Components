using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCrafting : MonoBehaviour
{
    [SerializeField]
    private TMP_Text behaviourText;

    [SerializeField]
    private List<BulletBehaviour> baseBehaviour = new();

    private List<BulletBehaviour> behaviours = new();

    [SerializeField]
    private List<TypedTrigger<Rune>> slots = new() ;

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

    public void AddBulletBehavior(Rune rune)
    {
        behaviours.Add(rune.BulletBehaviour);
        UpdateUI();
    }

    public void RemoveBulletBehavior(Rune rune)
    {
        behaviours.Remove(rune.BulletBehaviour);
        UpdateUI();
    }

    private void UpdateUI()
    {
        string text = "";

        foreach (BulletBehaviour behaviour in behaviours)
        {
            List<FieldData> fields = behaviour.GetValues();
            foreach (FieldData field in fields)
            {
                text += $"{field.FieldName}: {field.Value} \n";
            }
        }

        behaviourText.text = text;
    }
}
