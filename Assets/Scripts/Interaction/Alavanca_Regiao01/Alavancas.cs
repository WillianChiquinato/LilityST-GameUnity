using UnityEngine;

public class Alavancas : MonoBehaviour
{
    [Header("Timers e Variaveis")]
    public bool TilesBool = false;
    public float timerDuration;
    public float timerTiles = 0f;
    public Vector3 offset;

    [Header("References")]
    public PlayerMoviment playerMoviment;
    public BoxCollider2D boxCollider;
    public Animator animator;
    public Tile_AlavancaScript[] tile_AlavancaScript;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
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
        this.boxCollider.enabled = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.Log("Entrou");
    }

    protected virtual void OnTriggerExit2D(Collider2D other) 
    {
        Debug.Log("Saiu");
    }

    protected virtual void OnTriggerStay2D(Collider2D other) 
    {
        Debug.Log("Ficou");
    }
}
