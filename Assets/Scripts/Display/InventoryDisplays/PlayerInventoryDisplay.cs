using UnityEngine;
using TMPro;
public class PlayerInventoryDisplay : MonoBehaviour
{
    public GameObject ItemPrefab;
    public TMP_Text CountAndNameText;
    public TMP_Text WeightText;
    public TMP_Text CostText;

    Inventory Inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Inventory = Game_Manager.Instance.player.Inventory;
        CountAndNameText = ItemPrefab.transform.GetChild(0).GetComponent<TMP_Text>();
        WeightText = ItemPrefab.transform.GetChild(1).GetComponent<TMP_Text>();
        CostText = ItemPrefab.transform.GetChild(2).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            WeaponBase sword = new WeaponBase(name: "Sword", min: 5, max: 10, description: "A sword", descriptionLog: "A sword", weight: 2, value: 10, itemType: ItemType.Weapon, weaponType: WeaponStyle.OneHanded, weaponClass: WeaponClass.Sword);
            WeaponBase axe = new WeaponBase(name: "Axe", min: 7, max: 12, description: "An axe", descriptionLog: "An axe", weight: 3, value: 15, itemType: ItemType.Weapon, weaponType: WeaponStyle.OneHanded, weaponClass: WeaponClass.Axe);
            Inventory.AddItem(axe, 1);
            Inventory.AddItem(sword, 1);
        }
    }
}
