using UnityEngine;

public class PressurePlate : Alavancas
{
    public LayerMask layerMask;
    public bool isPressed = false;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            animator.SetBool("IsPressed", true);
            isPressed = true;
            TilesBool = true;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            animator.SetBool("IsPressed", false);
            isPressed = false;
            TilesBool = false;
            this.boxCollider.enabled = true;
        }
    }
}
