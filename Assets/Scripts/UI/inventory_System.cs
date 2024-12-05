using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory_System : MonoBehaviour
{
    public static inventory_System instance;

    public List<ItemData> startEquipament;

    public List<Inventory_item> inventory;
    public Dictionary<ItemData, Inventory_item> inventoryDicionary;

    public List<Inventory_item> docs;
    public Dictionary<ItemData, Inventory_item> docsDicionary;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform documentosSlotParent;


    [SerializeField] private Item_SlotUI[] inventoryItemSlot;
    [SerializeField] private Item_SlotUI[] documentosItemSlot;


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

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<Item_SlotUI>();
        documentosItemSlot = documentosSlotParent.GetComponentsInChildren<Item_SlotUI>();

        for (int i = 0; i < startEquipament.Count; i++)
        {
            AddItem(startEquipament[i]);
        }
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



        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateInventory(inventory[i]);
        }

        for (int i = 0; i < docs.Count; i++)
        {
            documentosItemSlot[i].UpdateInventory(docs[i]);
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

    public List<Inventory_item> GetInventoryList() => inventory;
}
