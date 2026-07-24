using UnityEngine;

public class ItemFragment_Trigger : MonoBehaviour
{
    public ItemFragment itemFragment;

    void Awake()
    {
        itemFragment = GetComponentInParent<ItemFragment>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMoviment>() != null && GameManager.instance.player.entrar)
        {
            itemFragment.PickUpItem();

            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
