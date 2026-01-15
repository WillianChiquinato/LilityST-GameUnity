using UnityEngine;

public class AlavancaScript : Alavancas
{
    public Damage damageOpenAlavanca;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        damageOpenAlavanca = GetComponent<Damage>();
    }

    protected override void Update()
    {
        base.Update();

        if (!damageOpenAlavanca.IsAlive)
        {
            animator.SetBool("Ativado", true);
            TilesBool = true;
            this.boxCollider.enabled = false;
            //Sound here.
        }

        if (alavancaReset)
        {
            damageOpenAlavanca.Reset();
        }
    }
}
