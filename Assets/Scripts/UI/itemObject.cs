using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private ItemData itemData;
    public PlayerMoviment playerMoviment;


    void Awake()
    {
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
    }

    private void SetupVisual()
    {
        if (itemData == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = itemData.Icon;
        gameObject.name = "Item - " + itemData.ItemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisual();
    }

    public void PickUpItem()
    {
        if (inventory_System.instance.LimiteItens())
        {
            return;
        }
        Debug.Log("Pegou " + itemData.ItemName);
        inventory_System.instance.AddItem(itemData);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMoviment>() != null && playerMoviment.entrar)
        {
            PickUpItem();
        }
    }
}
