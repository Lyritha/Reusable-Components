using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : InputListener
{
    [SerializeField]
    private List<WeaponAsset> weapons = new();

    private GameObject currentWeapon;

    private void Awake()
    {
        AddSubscription(e => e.OnNumberSelected += OnNumberSelected, e => e.OnNumberSelected -= OnNumberSelected);
        OnNumberSelected(1);
    }

    private void OnNumberSelected(int number)
    {
        if (number > 0 && number <= weapons.Count)
        {
            Destroy(currentWeapon);

            WeaponAsset selectedWeapon = weapons[number - 1];
            currentWeapon = Instantiate(selectedWeapon.weaponPrefab, transform, false);
            // Handle weapon selection logic here
        }
    }
}
