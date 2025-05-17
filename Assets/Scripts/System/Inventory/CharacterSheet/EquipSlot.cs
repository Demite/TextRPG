using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image panelImage;
    [SerializeField] private Image BrokenPanel;
    [SerializeField] private TMP_Text slotText;
    [SerializeField] private Image iconImage;

    [Header("Slot Colors")]
    [SerializeField] private Color baseColor = new Color32(255, 255, 255, 100);
    [SerializeField] private Color equippedColor = new Color32(0, 0, 0, 100);
    [SerializeField] private Color brokenColor = new Color32(226, 69, 69, 159);

    private ItemBase itemInSlot;
    private bool isBroken;
    private bool isEquipped;

    /// <summary>
    /// Call this to equip an item into the slot.
    /// </summary>
    public void Equip(ItemBase item)
    {
        itemInSlot = item;
        isBroken = false;
        isEquipped = item != null;
        UpdateVisuals();
    }

    /// <summary>
    /// Call this to remove whatever¡¯s in the slot.
    /// </summary>
    public void Unequip()
    {
        itemInSlot = null;
        isBroken = false;
        isEquipped = false;
        UpdateVisuals();
    }

    private void Update()
    {
        if (itemInSlot != null && !isBroken && itemInSlot.ItemDurability <= 0)
        {
            isBroken = true;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Drives the panel color & icon state.
    /// </summary>
    private void UpdateVisuals()
    {
        if (itemInSlot != null && isEquipped)
        {
            panelImage.color = isBroken ? brokenColor : equippedColor;
            iconImage.sprite = itemInSlot.Icon;
            iconImage.enabled = true;
        }
        else
        {
            panelImage.color = baseColor;
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}
