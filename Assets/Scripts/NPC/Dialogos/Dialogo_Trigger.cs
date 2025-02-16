using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerCaracter
{
    public string nome;
    public Sprite icone;
    public bool isUpDialog;
}

[System.Serializable]
public class DialogoTexto
{
    public DialogoTriggerCaracter caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
}

[System.Serializable]
public class Dialogos
{
    public List<DialogoTexto> dialogoTextos = new List<DialogoTexto>();
}

public class Dialogo_Trigger : MonoBehaviour
{
    public Dialogos dialogos;
    public PlayerMoviment playerMoviment;
    public Input_Conversa input_Conversa;
    public Animator animator;

    public GameObject dialogosAnim;
    public float TimerTargetDialogo;
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
        Dialogos_Manager.dialogos_Manager.StartDialogos(dialogos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (dialogosAnim != null)
            {
                dialogosAnim.SetActive(true);
            }
            else
            {
                Debug.Log("Sem indicador");
            }

            if (this.gameObject.CompareTag("Inimigos"))
            {
                targetBool = true;
                if (TimerDialogo >= TimerTargetDialogo)
                {
                    TriggerDialogo();
                    TimerDialogo = 0f;
                    return;
                }
            }
            else if (playerMoviment.entrar)
            {
                TriggerDialogo();
                animator.SetBool(animationstrings.InicioDialogo, true);
            }
        }

        if (!Dialogos_Manager.dialogos_Manager.isDialogoAtivo && !this.gameObject.CompareTag("Inimigos"))
        {
            animator.SetBool(animationstrings.InicioDialogo, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMoviment>().StartCoroutine(AnimacaoSair());
        }
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
