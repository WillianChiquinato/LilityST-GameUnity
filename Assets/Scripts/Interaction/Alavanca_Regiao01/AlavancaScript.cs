using UnityEngine;

public class AlavancaScript : Alavancas
{
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerMoviment.entrar)
        {
            animator.SetBool("Ativado", true);
            TilesBool = true;
            this.boxCollider.enabled = false;
        }
    }
}
