using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : Item_drop
{
    [Header("Player's Drop")]
    [SerializeField] private float chanceToropItens;

    public override void GenerateDrop()
    {
        inventory_System inventory = inventory_System.instance;

        List<Inventory_item> materialsLoose = new List<Inventory_item>();


        foreach (Inventory_item item in inventory.GetInventoryList())
        {
            if (Random.Range(0, 100) <= chanceToropItens)
            {
                DropItem(item.itemData);
                materialsLoose.Add(item);
            }
        }

        for (int i = 0; i < materialsLoose.Count; i++)
        {
            inventory.RemoveItem(materialsLoose[i].itemData);
        }
    }
}
