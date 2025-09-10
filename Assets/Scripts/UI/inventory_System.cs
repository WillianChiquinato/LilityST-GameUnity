using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class inventory_System : MonoBehaviour
{
    public static inventory_System instance;

    public List<ItemData> startEquipament;

    public List<Inventory_item> inventory;
    public Dictionary<ItemData, Inventory_item> inventoryDicionary;

    public List<Inventory_item> docs;
    public Dictionary<ItemData, Inventory_item> docsDicionary;

    public List<Inventory_item> coletaveis;
    public Dictionary<ItemData, Inventory_item> collectDicionary;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform documentosSlotParent;

    [SerializeField] private Transform coletaveisSlotParent;


    [SerializeField] private Item_SlotUI[] inventoryItemSlot;
    [SerializeField] private Item_SlotUI[] documentosItemSlot;
    [SerializeField] private Item_SlotUI[] coletaveisItemSlot;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inventory = new List<Inventory_item>();
        inventoryDicionary = new Dictionary<ItemData, Inventory_item>();

        docs = new List<Inventory_item>();
        docsDicionary = new Dictionary<ItemData, Inventory_item>();

        coletaveis = new List<Inventory_item>();
        collectDicionary = new Dictionary<ItemData, Inventory_item>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<Item_SlotUI>();
        documentosItemSlot = documentosSlotParent.GetComponentsInChildren<Item_SlotUI>();
        coletaveisItemSlot = coletaveisSlotParent.GetComponentsInChildren<Item_SlotUI>();

        int currentSlot = GameManager.currentSaveSlot;

        if (!SaveManager.SaveExists(currentSlot))
        {
            foreach (var item in startEquipament)
            {
                AddItem(item);
            }

            SaveInventory();
            Debug.Log("Inventário inicializado com os itens de startEquipament.");
        }
        else
        {
            var loadedData = SaveManager.Load(GameManager.currentSaveSlot);
            if (loadedData != null)
            {
                SaveData.Instance.inventoryData = loadedData.inventoryData;
            }

            if (SaveData.Instance != null && SaveData.Instance.inventoryData != null)
            {
                LoadInventory();
            }
            else
            {
                Debug.LogWarning("Não foi possível carregar o inventário. SaveData vazio.");
            }
        }
    }

    public bool IsInventoryJSONFileEmpty(string path)
    {
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);
            return string.IsNullOrWhiteSpace(fileContent);
        }
        else
        {
            Debug.LogError("O arquivo JSON não foi encontrado.");
            return true;
        }
    }

    [System.Serializable]
    public class InventoryItemSaveData
    {
        public string itemName;
        public itensType itemType;
        public int stackSize;
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventoryItemSaveData> MaterialsItens = new List<InventoryItemSaveData>();
        public List<InventoryItemSaveData> DocumentsItens = new List<InventoryItemSaveData>();
        public List<InventoryItemSaveData> ColectItens = new List<InventoryItemSaveData>();
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < documentosItemSlot.Length; i++)
        {
            documentosItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < coletaveisItemSlot.Length; i++)
        {
            coletaveisItemSlot[i].CleanUpSlot();
        }



        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateInventory(inventory[i]);
        }

        for (int i = 0; i < docs.Count; i++)
        {
            documentosItemSlot[i].UpdateInventory(docs[i]);
        }
        for (int i = 0; i < coletaveis.Count; i++)
        {
            coletaveisItemSlot[i].UpdateInventory(coletaveis[i]);
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itensType == itensType.Documentos)
        {
            AddToDocuments(_item);
        }
        else if (_item.itensType == itensType.Materiais)
        {
            AddToInventory(_item);
        }
        else if (_item.itensType == itensType.Coletaveis)
        {
            AddToCollect(_item);
        }

        UpdateInventory();
    }

    public void AddToInventory(ItemData _item)
    {
        if (inventoryDicionary.TryGetValue(_item, out Inventory_item value))
        {
            value.AddStack();
        }
        else
        {
            Inventory_item newItem = new Inventory_item(_item);
            inventory.Add(newItem);
            inventoryDicionary.Add(_item, newItem);
        }
    }

    public void AddToDocuments(ItemData _item)
    {
        if (docsDicionary.TryGetValue(_item, out Inventory_item value))
        {
            value.AddStack();
        }
        else
        {
            Inventory_item newItem = new Inventory_item(_item);
            docs.Add(newItem);
            docsDicionary.Add(_item, newItem);
        }
    }

    public void AddToCollect(ItemData _item)
    {
        if (collectDicionary.TryGetValue(_item, out Inventory_item value))
        {
            value.AddStack();
        }
        else
        {
            Inventory_item newItem = new Inventory_item(_item);
            coletaveis.Add(newItem);
            collectDicionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDicionary.TryGetValue(_item, out Inventory_item value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDicionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }

        if (docsDicionary.TryGetValue(_item, out Inventory_item docsValue))
        {
            if (docsValue.stackSize <= 1)
            {
                inventory.Remove(docsValue);
                inventoryDicionary.Remove(_item);
            }
            else
            {
                docsValue.RemoveStack();
            }
        }

        if (collectDicionary.TryGetValue(_item, out Inventory_item collectvalue))
        {
            collectvalue.RemoveStack();

            if (collectvalue.stackSize <= 0)
            {
                collectvalue.ResetStack();
            }
        }

        UpdateInventory();
    }

    public bool LimiteItens()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("Atingiu o limite");
            return false;
        }

        return true;
    }

    public void SaveInventory()
    {
        SaveData.Instance.inventoryData = new InventorySaveData();

        foreach (var item in inventory)
        {
            SaveData.Instance.inventoryData.MaterialsItens.Add(new InventoryItemSaveData
            {
                itemName = item.itemData.name,
                itemType = item.itemData.itensType,
                stackSize = item.stackSize
            });
        }

        foreach (var doc in docs)
        {
            SaveData.Instance.inventoryData.DocumentsItens.Add(new InventoryItemSaveData
            {
                itemName = doc.itemData.name,
                itemType = doc.itemData.itensType,
                stackSize = doc.stackSize
            });
        }

        foreach (var colet in coletaveis)
        {
            SaveData.Instance.inventoryData.ColectItens.Add(new InventoryItemSaveData
            {
                itemName = colet.itemData.name,
                itemType = colet.itemData.itensType,
                stackSize = colet.stackSize
            });
        }

        SaveManager.Save(SaveData.Instance, GameManager.currentSaveSlot);
    }


    public void LoadInventory()
    {
        InventorySaveData data = SaveData.Instance.inventoryData;

        inventory.Clear();
        docs.Clear();
        coletaveis.Clear();

        foreach (var item in data.MaterialsItens)
        {
            ItemData refItem = GetItemData(item.itemName, item.itemType);
            for (int i = 0; i < item.stackSize; i++)
                AddToInventory(refItem);
        }

        foreach (var item in data.DocumentsItens)
        {
            ItemData refItem = GetItemData(item.itemName, item.itemType);
            for (int i = 0; i < item.stackSize; i++)
                AddToDocuments(refItem);
        }

        foreach (var item in data.ColectItens)
        {
            ItemData refItem = GetItemData(item.itemName, item.itemType);
            for (int i = 0; i < item.stackSize; i++)
                AddToCollect(refItem);
        }

        UpdateInventory();
    }


    public ItemData GetItemData(string itemName, itensType type)
    {
        Debug.Log("Tentando carregar: Itens/" + itemName);
        ItemData item = Resources.Load<ItemData>("Itens/" + itemName);
        if (item == null)
        {
            Debug.LogError("Item não encontrado: " + itemName);
        }
        return item;
    }

    public List<Inventory_item> GetInventoryList() => inventory;
}
