using UnityEngine;
using System.Collections.Generic;

public class SelectedItemToUse : MonoBehaviour
{
    public Item_SlotUI item_SlotUI;

    void Start()
    {
        item_SlotUI = GetComponent<Item_SlotUI>();
    }

    void Update()
    {
        if (item_SlotUI.item.itemData != null)
        {
            inventory_System.instance.selectedItemToUse = new List<Inventory_item> { item_SlotUI.item };
        }
    }
}
