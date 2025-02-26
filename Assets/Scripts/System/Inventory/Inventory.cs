using UnityEngine;

public class Inventory
{
    public int inventorySize = 20;
    public InventorySlot[] items;

    public Inventory()
    {
        items = new InventorySlot[inventorySize];
    }
    public Inventory(int size)
    {
        inventorySize = size;
        items = new InventorySlot[inventorySize];
    }

    public void AddItem(ItemBase item, int amount)
    {
        int remaining = amount;

        // First, try to add to any existing slots with the same item that have room.
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].item == item)
            {
                // Calculate how many items can still fit in this slot.
                int room = item.MaxStack - items[i].amount;
                if (room > 0)
                {
                    int toAdd = Mathf.Min(remaining, room);
                    items[i].amount += toAdd;
                    remaining -= toAdd;

                    // If all items have been added, we're done.
                    if (remaining <= 0)
                        return;
                }
            }
        }

        // Next, if there is any remaining amount, add them in new empty slots.
        for (int i = 0; i < items.Length; i++)
        {
            if (remaining <= 0)
                break;

            if (items[i] == null)
            {
                int toAdd = Mathf.Min(remaining, item.MaxStack);
                items[i] = new InventorySlot(item, toAdd);
                remaining -= toAdd;
            }
        }

        // Optionally, if remaining > 0, it means the inventory is full.
        if (remaining > 0)
        {
            Debug.LogWarning("Not enough room in inventory for the entire item stack.");
            // TODO: Optionally, drop the remaining items on the ground.
        }
    }
    /// <summary>
    /// Removes a specified amount of an item from the inventory.
    /// Returns true if the removal was successful; false if not enough items were available.
    /// </summary>
    public bool RemoveItem(ItemBase item, int amount)
    {
        int remaining = amount;

        // First, calculate the total available amount for this item.
        int totalAvailable = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].item == item)
            {
                totalAvailable += items[i].amount;
            }
        }

        if (totalAvailable < amount)
        {
            Debug.LogWarning("Not enough items in inventory to remove.");
            return false;
        }

        // Remove the items from slots until the desired amount is removed.
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].item == item)
            {
                if (items[i].amount <= remaining)
                {
                    remaining -= items[i].amount;
                    items[i] = null; // Clear the slot if all items in this slot are removed.
                }
                else
                {
                    items[i].amount -= remaining;
                    remaining = 0;
                }

                // Once we've removed the required amount, exit early.
                if (remaining <= 0)
                    break;
            }
        }

        return true;
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
