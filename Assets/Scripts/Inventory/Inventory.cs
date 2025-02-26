using UnityEngine;

public class Inventory
{
    public int inventorySize = 20;
    public InventorySlot[] items;
    
    public Inventory()
    {
        items = new InventorySlot[inventorySize];
        //AddItem(new ItemBase("Sword", "A sharp sword", 2, 10), 1);
    }
    public void AddItem(ItemBase item, int amount)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventorySlot(item, amount);
                return;
            }
        }
    }
}
public class InventorySlot
{
    public ItemBase item;
    public int amount;
    public InventorySlot(ItemBase item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
