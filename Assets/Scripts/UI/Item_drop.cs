using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropEntry
{
    [Range(0, 100)]
    public float dropChance = 100;

    public ItemData itemData;
    public GameObject objectPrefab;
}

public class Item_drop : MonoBehaviour
{
    [SerializeField] private int amountOfItems = 1;

    [SerializeField] private List<DropEntry> possibleDrops = new();

    public GameObject dropPrefab;

    private readonly List<DropEntry> dropList = new();

    public virtual void GenerateDrop()
    {
        dropList.Clear();

        foreach (var drop in possibleDrops)
        {
            if (Random.Range(0f, 100f) <= drop.dropChance)
            {
                dropList.Add(drop);
            }
        }

        int dropsToGenerate = Mathf.Min(amountOfItems, dropList.Count);

        for (int i = 0; i < dropsToGenerate; i++)
        {
            int randomIndex = Random.Range(0, dropList.Count);

            DropEntry selectedDrop = dropList[randomIndex];

            dropList.RemoveAt(randomIndex);

            if (selectedDrop.itemData != null)
            {
                DropItemData(selectedDrop.itemData);
            }
            else if (selectedDrop.objectPrefab != null)
            {
                DropObjectItem(selectedDrop.objectPrefab);
            }
        }
    }

    public virtual void DropItemData(ItemData itemData)
    {
        GameObject newDrop = Instantiate(
            dropPrefab,
            transform.position,
            Quaternion.identity
        );

        Vector2 randomVelocity = new Vector2(
            Random.Range(-5f, 5f),
            Random.Range(10f, 15f)
        );

        ItemObject itemObject = newDrop.GetComponent<ItemObject>();

        if (itemObject != null)
        {
            itemObject.SetupItem(itemData, randomVelocity);
        }
    }

    public virtual void DropObjectItem(GameObject objectPrefab)
    {
        GameObject spawnedObject = Instantiate(
            objectPrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                Random.Range(-3f, 3f),
                Random.Range(5f, 7f)
            );
        }
    }
}