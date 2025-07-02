using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerCaracter
{
    public string nome;
    public Sprite icone;
}

[System.Serializable]
public class DialogoTexto
{
    public DialogoTriggerCaracter caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
    public bool isLility = false;
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

    public float tempoDeEspera;
    public bool targetEndDialogo = false;

    public delegate void DialogoFinalizado();
    public event DialogoFinalizado OnDialogoFinalizado;

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
        Debug.Log("Dialogo iniciado!");
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

            if (gameObject.CompareTag("Inimigos"))
            {
                targetBool = true;
                if (TimerDialogo >= TimerTargetDialogo)
                {
                    Debug.Log("Dialogo com inimigo iniciado!");
                    TriggerDialogo();
                    TimerDialogo = 0f;
                    return;
                }
            }
            else if (playerMoviment.entrar && this.gameObject.CompareTag("Cervo"))
            {
                StartCoroutine(EsperaLility());
            }
            else if (playerMoviment.entrar)
            {
                TriggerDialogo();
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

    IEnumerator EsperaLility()
    {
        yield return new WaitForSeconds(3f);

        TriggerDialogo();
        animator.SetBool(animationstrings.InicioDialogo, true);
    }

    public void NotificarDialogoFinalizado()
    {
        OnDialogoFinalizado?.Invoke();
        Debug.Log("Dialogo finalizado!");
        if (this.gameObject.CompareTag("Cervo"))
        {
            StartCoroutine(AbrirUIComDelay());
        }
    }

    private IEnumerator AbrirUIComDelay()
    {
        yield return new WaitForSeconds(1f);
        Dialogos_Manager.dialogos_Manager.UISavePoint.SetActive(true);
    }
}
