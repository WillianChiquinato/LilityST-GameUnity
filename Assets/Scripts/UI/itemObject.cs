using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class itemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TextMeshPro texto;

    [SerializeField] public ItemData itemData;
    public PlayerMoviment playerMoviment;


    void Awake()
    {
        texto = GetComponentInChildren<TextMeshPro>();

        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        GetComponent<SpriteRenderer>().sprite = itemData.Icon;
        gameObject.name = "Item - " + itemData.ItemName;
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
        rb.linearVelocity = _velocity;

        SetupVisual();
    }

    public void PickUpItem()
    {
        Debug.Log("Pegou " + itemData.ItemName);
        inventory_System.instance.AddItem(itemData);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            texto.text = "Pegar " + itemData.ItemName; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            texto.text = "";
        }
    }
}
