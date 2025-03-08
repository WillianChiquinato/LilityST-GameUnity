using UnityEngine;

public class FumacaController : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Animator animator;
    public bool fallEffectAnimation = false;

    void Start()
    {
        playerMoviment = GetComponentInParent<PlayerMoviment>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("IsGround", playerMoviment.touching.IsGrouded);
        animator.SetBool("IsMoving", playerMoviment.IsMoving);

        if (playerMoviment.touching.IsGrouded)
        {
            if (!fallEffectAnimation) // Apenas ativa se ainda não tiver ativado
            {
                fallEffectAnimation = true;
                animator.SetTrigger("IsGroundTrigger");
            }
        }
        else
        {
            fallEffectAnimation = false; // Reseta apenas quando o personagem sair do chão
            animator.ResetTrigger("IsGroundTrigger");
        }
    }
}
