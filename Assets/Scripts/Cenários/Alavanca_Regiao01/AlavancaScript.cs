using UnityEngine;

public class AlavancaScript : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Animator animator;
    public Tile_AlavancaScript[] tile_AlavancaScript;
    public bool TilesBool = false;
    public float timerDuration;
    public float timerTiles = 0f;

    void Start()
    {
        tile_AlavancaScript = FindObjectsByType<Tile_AlavancaScript>(FindObjectsSortMode.None);

        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (TilesBool)
        {
            timerTiles -= Time.deltaTime;

            foreach (var objeto in tile_AlavancaScript)
            {
                if (objeto.playerEncosto)
                {
                    objeto.StartCoroutine(objeto.TileTimer());
                }
                else
                {
                    objeto.animator.SetBool("Ativado", true);
                    objeto.boxCollider2D.enabled = true;
                }
            }

            if (timerTiles <= 0f)
            {
                TilesBool = false;
                ResetarTiles();
            }
        }
        else
        {
            timerTiles = timerDuration;
            ResetarTiles();
        }
    }

    void ResetarTiles()
    {
        foreach (var objeto in tile_AlavancaScript)
        {
            objeto.animator.SetBool("Ativado", false);
            objeto.boxCollider2D.enabled = false;
            objeto.playerEncosto = false;
        }

        animator.SetBool("Ativado", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerMoviment.entrar)
        {
            animator.SetBool("Ativado", true);
            TilesBool = true;
        }
    }
}
