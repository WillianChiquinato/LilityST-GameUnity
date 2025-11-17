using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : Item_drop
{
    [Header("Player's Drop")]
    [SerializeField] private List<ItemData> DropsInventoryPlayer;
    private Vector3 lastDeathPos;
    public List<ItemData> droppedItems = new List<ItemData>();
    public bool podeCriar = true;
    public LayerMask groundLayer;

    void Start()
    {
        LoadPossibleDrops();
    }

    public override void GenerateDrop()
    {
        if (DropsInventoryPlayer == null || DropsInventoryPlayer.Count == 0)
        {
            return;
        }
        droppedItems.Clear();

        foreach (ItemData item in DropsInventoryPlayer)
        {
            if (Random.Range(0, 100) <= item.dropChance)
            {
                DropItem(item);
                inventory_System.instance.RemoveItemToDeath(item, 1);
                droppedItems.Add(item);
            }
        }

        lastDeathPos = transform.position;

        if (podeCriar)
        {
            Vector3 origemRay = GameManager.instance.player.transform.position + Vector3.up * 0.5f;
            RaycastHit2D hit = Physics2D.Raycast(origemRay, Vector2.down, 100f, groundLayer);

            Debug.DrawRay(origemRay, Vector2.down * 5f, Color.green, 2f);

            if (hit.collider != null)
            {
                Vector3 posicaoChao = hit.point;
                posicaoChao.y += 0.62f;
                lastDeathPos = posicaoChao;
            }
            else
            {
                lastDeathPos = GameManager.instance.player.transform.position;
            }

            EstatuaSystem.Instance.CreateEstatua(
                lastDeathPos,
                GameManager.instance.XpPlayer > 0 ? GameManager.instance.XpPlayer : 0,
                droppedItems
            );

            podeCriar = false;
            inventory_System.instance.SaveInventory();
            SaveData.Instance.XPlayer += -GameManager.instance.XpPlayer;
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
            }
        }

        // Adiciona ColectItens
        foreach (var itemData in inventoryData.ColectItens)
        {
            ItemData item = inventory_System.instance.GetItemData(itemData.itemName, itemData.itemType);
            if (item != null)
            {
                DropsInventoryPlayer.Add(item);
            }
        }
    }

    public override void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelo = new Vector2(Random.Range(-5, 5), Random.Range(10, 15));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelo);
    }
}
