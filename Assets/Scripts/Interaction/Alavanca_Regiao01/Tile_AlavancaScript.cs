using System.Collections;
using UnityEngine;

public class Tile_AlavancaScript : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public bool playerEncosto = false;
    public Animator animator;
    public BoxCollider2D boxCollider2D;
    public GameObject oneWayStart;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        boxCollider2D.enabled = false;
    }

    void Update()
    {

    }

    //Caso troque de ideia, excluir linhas abaixo.
    private void OnCollisionStay2D(Collision2D Collision)
    {
        if (Collision.gameObject.CompareTag("Player") && playerMoviment.touching.IsGrouded)
        {
            //Em cima
            playerEncosto = true;
        }
    }

    public IEnumerator TileTimer()
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("Ativado", false);

        yield return new WaitForSeconds(0.1f);

        boxCollider2D.enabled = false;
    }
}
