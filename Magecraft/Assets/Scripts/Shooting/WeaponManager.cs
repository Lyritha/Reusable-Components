using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> weapons = new();

    private GameObject currentWeapon;

    private void Awake()
    {
        ICharacterInput input = gameObject.GetComponentInParent<ICharacterInput>();

        if (input != null)
        {
            input.NumberSelectEvent += OnNumberSelected;
        }

        OnNumberSelected(1);
    }

    private void OnNumberSelected(int number)
    {
        if (number > 0 && number <= weapons.Count)
        {
            Destroy(currentWeapon);

            Weapon selectedWeapon = weapons[number - 1];
            currentWeapon = Instantiate(selectedWeapon.weaponPrefab, transform, false);
            // Handle weapon selection logic here
        }
    }
}
