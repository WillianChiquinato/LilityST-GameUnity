using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObj_Trigger : MonoBehaviour
{
    public itemObject itemObject;
    PlayerMoviment playerMoviment;

    void Awake()
    {
        itemObject = GetComponentInParent<itemObject>();
        playerMoviment = FindObjectOfType<PlayerMoviment>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMoviment>() != null && playerMoviment.entrar)
        {
            itemObject.PickUpItem();
        }
    }
}
