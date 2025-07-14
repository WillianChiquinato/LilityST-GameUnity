using UnityEngine;

public class PernasArcoLogic : MonoBehaviour
{
    public PlayerMoviment player;
    Animator anim;

    void Start()
    {
        player = GetComponentInParent<PlayerMoviment>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("IsGrounded", player.touching.IsGrouded);
        anim.SetBool("IsMoving", player.IsMoving);
        anim.SetFloat("yVelocity", player.rb.linearVelocity.y);
    }
}
