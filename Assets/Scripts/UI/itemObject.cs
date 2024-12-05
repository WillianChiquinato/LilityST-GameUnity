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
<<<<<<< HEAD
        rb.linearVelocity = _velocity;
=======
        rb.velocity = _velocity;
>>>>>>> 22fa71694fc4d3eb86e284a7a5c186e2275aeb23

        SetupVisual();
    }

    public void PickUpItem()
    {
        Debug.Log("Pegou " + itemData.ItemName);
        inventory_System.instance.AddItem(itemData);
        Destroy(this.gameObject);
    }
}
