using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class WeaponAsset : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
}
