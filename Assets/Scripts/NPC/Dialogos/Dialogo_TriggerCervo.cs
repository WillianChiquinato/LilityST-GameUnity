using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerCaracterCervo
{
    public string nome;
    public Sprite icone;
}

[System.Serializable]
public class DialogoTextoCervo
{
    public DialogoTriggerCaracterCervo caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
}

[System.Serializable]
public class DialogosCervo
{
    public List<DialogoTextoCervo> dialogoTextos = new List<DialogoTextoCervo>();
}

public class Dialogo_TriggerCervo : MonoBehaviour
{
    public DialogosCervo dialogos;
    public PlayerMoviment playerMoviment;
    public Input_Conversa input_Conversa;
    public Animator animator;

    public GameObject dialogosAnim;
    public float TimerDialogo;
    public bool targetBool = false;

    private void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (TimerDialogo >= 1f && targetBool)
        {
            TimerDialogo += Time.deltaTime;
        }
    }

    public void TriggerDialogo()
    {
        DialogoCervo.dialogos_Cervo.StartDialogos(dialogos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerMoviment.entrar)
            {
                StartCoroutine(EsperaLility());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMoviment>().StartCoroutine(AnimacaoSair());
        }
    }

    public IEnumerator EsperaLility()
    {
        yield return new WaitForSeconds(4f);

        TriggerDialogo();
        animator.SetBool(animationstrings.InicioDialogo, true);
    }

    IEnumerator AnimacaoSair()
    {
        if (input_Conversa != null)
        {
            input_Conversa.animator.SetBool("Dialogos", true);
        }
        yield return new WaitForSeconds(0.6f);

        if (dialogosAnim != null)
        {
            dialogosAnim.SetActive(false);
        }
        else
        {
            Debug.Log("Sem indicador");
        }
    }
}