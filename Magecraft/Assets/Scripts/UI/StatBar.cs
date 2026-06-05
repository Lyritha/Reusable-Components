using System;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField]
    private Slider statSlider;

    public void UpdateUI(int currentHealth, int maxHealth)
    {
        statSlider.maxValue = maxHealth;
        statSlider.value = currentHealth;
    }
}
