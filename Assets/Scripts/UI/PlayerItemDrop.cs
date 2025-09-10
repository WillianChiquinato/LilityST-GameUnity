using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerItemDrop : Item_drop
{
    [Header("Player's Drop")]
    [SerializeField] private float chanceToropItens;
    [SerializeField] private List<ItemData> DropsInventoryPlayer;

    void Start()
    {
        LoadPossibleDrops();
    }

    public override void GenerateDrop()
    {
        if (DropsInventoryPlayer == null || DropsInventoryPlayer.Count == 0)
        {
            Debug.LogWarning("[DROP] Nenhum possível drop carregado!");
            return;
        }

        foreach (ItemData item in DropsInventoryPlayer)
        {
            if (Random.Range(0, 100) <= chanceToropItens)
            {
                DropItem(item);
                Debug.Log($"[DROP] Item dropado: {item.ItemName}");
            }
        }
    }

    public void LoadPossibleDrops()
    {
        var inventoryData = SaveData.Instance.inventoryData;

        DropsInventoryPlayer.Clear();

        // Adiciona MaterialsItens
        foreach (var itemData in inventoryData.MaterialsItens)
        {
            ItemData item = inventory_System.instance.GetItemData(itemData.itemName, itemData.itemType);
            if (item != null)
            {
                DropsInventoryPlayer.Add(item);
                Debug.Log($"[DROP] Adicionando possível drop: {itemData.itemName}");
            }
        }

        // Adiciona DocumentsItens
        foreach (var itemData in inventoryData.DocumentsItens)
        {
            ItemData item = inventory_System.instance.GetItemData(itemData.itemName, itemData.itemType);
            if (item != null)
            {
                DropsInventoryPlayer.Add(item);
                Debug.Log($"[DROP] Adicionando possível drop: {itemData.itemName}");
            }
        }

        // Adiciona ColectItens
        foreach (var itemData in inventoryData.ColectItens)
        {
            ItemData item = inventory_System.instance.GetItemData(itemData.itemName, itemData.itemType);
            if (item != null)
            {
                DropsInventoryPlayer.Add(item);
                Debug.Log($"[DROP] Adicionando possível drop: {itemData.itemName}");
            }
        }

        Debug.Log($"[DROP] Total de possíveis drops carregados: {DropsInventoryPlayer.Count}");
    }
}
