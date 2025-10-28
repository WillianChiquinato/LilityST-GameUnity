using System.Collections;
using UnityEngine;

public class vidroScript : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer[] cornerSpriteRenderer;
    public BoxCollider2D boxCollider2D;
    public bool GeneralActived = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        foreach (var vidro in cornerSpriteRenderer)
        {
            vidro.enabled = false;
        }
    }

    void Update()
    {
        if (GeneralActived)
        {
            // Verifica a posição relativa ao player e ao inimigo
            bool isInFrontOfPlayer = player.position.x < transform.position.x;

            if (isInFrontOfPlayer)
            {
                spriteRenderer.sortingOrder = 20;
            }
            else
            {
                spriteRenderer.sortingOrder = -1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (GeneralActived)
        {
            if (colisor.gameObject.CompareTag("Player"))
            {
                StartCoroutine(VidroTimer());
            }
        }
    }

    IEnumerator VidroTimer()
    {
        animator.SetBool("Vidro", true);

        yield return new WaitForSeconds(1.3f);

        foreach (var vidro in cornerSpriteRenderer)
        {
            vidro.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        this.spriteRenderer.enabled = false;
    }


}
