using UnityEngine;

public class PernasArcoLogic : MonoBehaviour
{
    public PlayerMoviment player;
    Animator anim;
    public Animator animTorax;
    public bool estaVirado;

    void Awake()
    {
        player = GetComponentInParent<PlayerMoviment>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        AtualizarAnimator(anim);
        AtualizarAnimator(animTorax);

        estaVirado = false;

        if (player.touching.IsGrouded)
        {
            if (player.moveInput.x != 0f)
            {
                if (player.moveInput.x >= 0.1f)
                {
                    if (player.bow.Direcao.x < -0.1f)
                    {
                        estaVirado = true;
                    }
                }
                else
                {
                    if (player.bow.Direcao.x > 0.1f)
                    {
                        estaVirado = true;
                    }
                }
            }

        }

        anim.SetBool("IsVire", estaVirado);
    }

    public void DisableBow()
    {
        player.bow.gameObject.SetActive(false);

        if (estaVirado)
        {
            if (player.moveInput.x > 0.1f)
            {
                player.IsRight = true;
            }
            else
            {
                player.IsRight = false;
            }
        }
        anim.SetBool("IsVire", false);
    }

    void AtualizarAnimator(Animator animator)
    {
        animator.SetBool("IsGrounded", player.touching.IsGrouded);
        animator.SetBool("IsMoving", player.IsMoving);
        animator.SetFloat("yVelocity", player.rb.linearVelocity.y);
        animator.SetBool("IsJumping", player.IsJumping);
        animator.SetFloat("MoveInput", player.moveInput.x);
        animator.SetBool("IsVire", estaVirado);
    }
}
