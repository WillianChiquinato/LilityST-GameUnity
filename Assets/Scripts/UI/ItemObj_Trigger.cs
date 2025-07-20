using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObj_Trigger : MonoBehaviour
{
    public ItemObject itemObject;

    void Awake()
    {
        itemObject = GetComponentInParent<ItemObject>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMoviment>() != null && GameManager.instance.player.entrar)
        {
            itemObject.PickUpItem();
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
