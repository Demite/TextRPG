using UnityEngine;
using UnityEngine.UI;
public enum ItemType
{
    Weapon,
    Armor,
    Jewelry,
    Quest,
    Consumable,
    Junk,
    Resource
}
public enum ItemSlot
{
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Neck,
    Ring1,
    Ring2,
    Weapon1,
    Weapon2,
    Shield
}
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class ItemBase : ScriptableObject
{
    [Header("Base Item Data")]
    [SerializeField] protected string itemName;
    public string ItemName => itemName;

    [SerializeField] protected Sprite icon;
    public Sprite Icon => icon;
    public ItemAttributes attributes;

    [SerializeField] protected int id;
    public int ID => id;
    public string DescriptionShort { get; set; }
    public string DescriptionLong { get; set; }
    public int MaxStack = 10;
    public int CurrentStack = 1;
    public float Weight { get; set; }
    public float Value { get; set; }
    public ItemType itemType { get; set; }


    // Equippable Item Attributes
    public float ItemDurability { get; set; }
    public float ItemMaxDurability { get; set; }
}
