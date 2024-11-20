using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item_SlotUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image itemImagem;
    [SerializeField] private Image originalSprite;
    [SerializeField] private TextMeshProUGUI itemTexto;

    public Inventory_item item;

    public void UpdateInventory(Inventory_item _newItem)
    {
        item = _newItem;

        if (item != null)
        {
            itemImagem.sprite = item.itemData.Icon;

            if (item.stackSize >= 1)
            {
                itemTexto.text = item.stackSize.ToString();
            }
            else
            {
                itemTexto.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImagem = originalSprite;
        itemTexto.text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Isso é um item");
    }
}
