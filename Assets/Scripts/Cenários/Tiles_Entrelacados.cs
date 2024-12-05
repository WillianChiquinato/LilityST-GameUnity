using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles_Entrelacados : MonoBehaviour
{
    public float Tempo_Spawn;
    public float Tempo_Animacao;
    public bool Tempo_Spawn_Bool = false;

    public bool Tempo_Animacao_Ativado;
    public bool Tempo_Reset;

    public GameObject Tiles;
    BoxCollider2D boxCollider2D;
    Animator animator;
    Animator animatorFimTile;


    void Start()
    {
        Tiles.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
        boxCollider2D = Tiles.GetComponent<BoxCollider2D>();
        animatorFimTile = Tiles.GetComponent<Animator>();

        boxCollider2D.enabled = false;
    }

    void Update()
    {
        if (Tempo_Spawn_Bool == true)
        {
            Tempo_Spawn += Time.deltaTime;
            Tiles.gameObject.SetActive(true);
            Tempo_Reset = true;
            animator.SetBool(animationstrings.Ativador_Ativo, true);
        }
        else if (Tempo_Animacao_Ativado == false && Tempo_Reset == true)
        {
            StartCoroutine(TimerTile());
        }

        if (Tempo_Spawn >= Tempo_Animacao)
        {
            boxCollider2D.enabled = true;
            Tempo_Spawn = 0f;
            Tempo_Spawn_Bool = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Parry"))
        {
            Tempo_Spawn_Bool = true;
        }
    }

    public IEnumerator TimerTile()
    {
        animatorFimTile.SetBool(animationstrings.Fim_TimeTile, true);
        animator.SetBool(animationstrings.Ativador_Ativo, false);
        Tempo_Reset = false;

        yield return new WaitForSeconds(0.4f);

        boxCollider2D.enabled = false;

        yield return new WaitForSeconds(0.6f);

        Tiles.gameObject.SetActive(false);
    }
}
