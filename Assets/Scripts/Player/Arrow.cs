using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    PlayerMoviment playerMoviment;
    CapsuleCollider2D ColliderArrow;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        ColliderArrow = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(playerMoviment.transform.localScale.x == 1)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Inimigos"))
        {
            animator.SetTrigger(animationstrings.Impacto);
            rb.bodyType = RigidbodyType2D.Static;
            ColliderArrow.enabled = false;
            Destroy(this.gameObject, 0.4f);
        }
    }
}
