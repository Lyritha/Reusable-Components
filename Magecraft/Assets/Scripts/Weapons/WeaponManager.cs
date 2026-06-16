using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : InputListener
{
    [SerializeField]
    private List<WeaponAsset> weapons = new();

    private GameObject currentWeapon;
    private int currentNumber;

    private void Awake()
    {
        AddSubscription(e => e.NumberSelected.OnEvent += OnNumberSelected, e => e.NumberSelected.OnEvent -= OnNumberSelected);
        AddSubscription(e => e.Scroll.OnEvent += OnScroll, e => e.Scroll.OnEvent -= OnScroll);

        OnNumberSelected(1);
    }

    private void OnScroll(int dir)
    {
        int newNumber = currentNumber + dir;
        if (newNumber > weapons.Count) newNumber -= weapons.Count;
        else if (newNumber < 0) newNumber = weapons.Count;

        currentNumber = newNumber;


        SwitchWeapon();
    }

    private void OnNumberSelected(int number)
    {
        if (number <= weapons.Count) currentNumber = number;
        SwitchWeapon();
    }

    private void SwitchWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
            Destroy(currentWeapon);
        }

        if (currentNumber > 0)
        {
            WeaponAsset selectedWeapon = weapons[currentNumber - 1];
            currentWeapon = Instantiate(selectedWeapon.weaponPrefab, transform, false);
        }
    }
}
