using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObj_Trigger : MonoBehaviour
{
    public itemObject itemObject;

    void Awake()
    {
        itemObject = GetComponentInParent<itemObject>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMoviment>() != null && GameManager.instance.player.entrar)
        {
            itemObject.PickUpItem();
        }
    }
}
