using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_drop : MonoBehaviour
{
    [SerializeField] private int amountOfItens;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> droplist = new List<ItemData>();


    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
            {
                droplist.Add(possibleDrop[i]);
            }
        }

        int dropsToGenerate = Mathf.Min(amountOfItens, droplist.Count);

        for (int i = 0; i < dropsToGenerate; i++)
        {
            if (droplist.Count > 0)
            {
                int randomIndex = Random.Range(0, droplist.Count);
                ItemData randomIten = droplist[randomIndex];

                droplist.RemoveAt(randomIndex);
                DropItem(randomIten);
            }
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelo = new Vector2(Random.Range(-5, 5), Random.Range(10, 15));

        newDrop.GetComponent<itemObject>().SetupItem(_itemData, randomVelo);
    }
}
