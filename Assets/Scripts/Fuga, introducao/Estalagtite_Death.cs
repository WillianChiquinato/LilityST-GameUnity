using UnityEngine;

public class Estalagtite_Death : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public CapsuleCollider2D capsuleCollider2D;

    private readonly string[] validTags = { "Player", "Enemy1", "Enemy2", "Enemy3", "Ground" };

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (System.Array.Exists(validTags, tag => tag == collisionInfo.collider.tag))
        {
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                animator.SetBool("Death", true);
                capsuleCollider2D.enabled = false;
            }
        }
    }
}
