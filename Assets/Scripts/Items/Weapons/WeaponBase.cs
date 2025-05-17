using UnityEngine;

public enum WeaponStyle
{
    OneHanded,
    TwoHanded,
    Ranged
}

public enum WeaponClass
{
    Sword,
    Axe,
    Mace,
    Bow,
    Crossbow,
    Staff,
    Unarmed,
    Wand
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapons/Weapon")]
public class WeaponBase : ItemBase
{
    [Header("Weapon Specific Data")]
    public WeaponStyle weaponType;
    public WeaponClass weaponClass;
    public ItemSlot weaponSlot;

    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private int range;

    public int MinDamage => minDamage;
    public int MaxDamage => maxDamage;
    public int Range => range;

    private void Reset()
    {
        itemType = ItemType.Weapon;
        weaponType = WeaponStyle.OneHanded;
        weaponClass = WeaponClass.Sword;
        weaponSlot = ItemSlot.Weapon1;
        minDamage = 1;
        maxDamage = 10;
        range = 1;
    }
}
