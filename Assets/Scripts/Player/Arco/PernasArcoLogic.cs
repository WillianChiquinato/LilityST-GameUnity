using UnityEngine;

public class PernasArcoLogic : MonoBehaviour
{
    public PlayerMoviment player;
    Animator anim;
    public bool estaVirado;

    void Awake()
    {
        player = GetComponentInParent<PlayerMoviment>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("IsGrounded", player.touching.IsGrouded);
        anim.SetBool("IsMoving", player.IsMoving);
        anim.SetFloat("yVelocity", player.rb.linearVelocity.y);
        anim.SetBool("IsJumping", player.IsJumping);
        anim.SetFloat("MoveInput", player.moveInput.x);
        anim.SetBool("IsVire", estaVirado);

        estaVirado = false;

        if (player.touching.IsGrouded)
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
}
