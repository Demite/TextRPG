using UnityEngine;
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
public abstract class ItemBase
{
    public string ItemName { get; set; }
    public string DescriptionShort { get; set; }
    public string DescriptionLong { get; set; }
    public int MaxStack = 10;
    public int currentStack = 1;
    public float Weight { get; set; }
    public float value { get; set; }
    public ItemType itemType { get; set; }

    public ItemBase(string name, string description,string descriptionLog, float weight, float value, ItemType itemType)
    {
        ItemName = name;
        DescriptionShort = description;
        DescriptionLong = descriptionLog;
        Weight = weight;
        this.value = value;
        this.itemType = itemType;
    }

}
