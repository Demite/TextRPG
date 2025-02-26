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
public class WeaponBase : ItemBase
{
    public WeaponStyle weaponType { get; set; }
    public WeaponClass weaponClass { get; set; }
    public int minDamage { get; set; }
    public int maxDamage { get; set; }
    private int range { get; set; }
    public WeaponBase(string name, int min, int max, string description, string descriptionLog, float weight, float value, ItemType itemType, WeaponStyle weaponType, WeaponClass weaponClass) : base(name, description, descriptionLog, weight, value, itemType)
    {
        this.weaponType = weaponType;
        this.weaponClass = weaponClass;
        minDamage = min;
        maxDamage = max;
    }
}
