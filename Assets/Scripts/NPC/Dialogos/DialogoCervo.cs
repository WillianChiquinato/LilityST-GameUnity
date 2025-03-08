using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoCervo : MonoBehaviour
{
    public static DialogoCervo dialogos_Cervo;

    public DialogoTextoCervo linhaAtual;

    [Header("Dialogo1")]
    public string initialName;
    public Image iconeCaracter;
    public TextMeshProUGUI nomeCaracter;
    public TextMeshProUGUI dialogoArea;


    [Header("Situacoes Gerais")]
    public bool isTextComplete = false;
    public Queue<DialogoTextoCervo> linhas;

    public bool isDialogoAtivo = false;
    public float speedTexto = 0.2f;
    public Animator animator;
    public PlayerMoviment playerMoviment;

    public GameObject UISavePoint;

    void Start()
    {
        this.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
        linhas = new Queue<DialogoTextoCervo>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();

        if (dialogos_Cervo == null)
        {
            dialogos_Cervo = this;
        }
    }

    public void StartDialogos(DialogosCervo dialogos)
    {
        this.gameObject.SetActive(true);
        isDialogoAtivo = true;

        animator.SetBool(animationstrings.IsDialogFinish, false);
        animator.SetBool(animationstrings.isDialog, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, false);

        linhas.Clear();

        foreach (DialogoTextoCervo dialogoTexto in dialogos.dialogoTextos)
        {
            linhas.Enqueue(dialogoTexto);
        }

        DisplayNextLinha();
    }

    public void DisplayNextLinha()
    {
        isTextComplete = false;
        if (linhas.Count == 0)
        {
            EndDialogo();
            return;
        }

        linhaAtual = linhas.Dequeue();

        nomeCaracter.text = initialName;
        iconeCaracter.sprite = linhaAtual.caracter.icone;
        nomeCaracter.text = linhaAtual.caracter.nome;

        StopAllCoroutines();
        StartCoroutine(Sequencial(linhaAtual));
    }

    IEnumerator Sequencial(DialogoTextoCervo dialogoTexto)
    {
        dialogoArea.text = "";
        foreach (char letter in dialogoTexto.linhaTexto.ToCharArray())
        {
            dialogoArea.text += letter;
            yield return new WaitForSeconds(speedTexto);
        }
        isTextComplete = true;
    }

    public void EndDialogo()
    {
        StartCoroutine(EndDialogTimer());
    }

    IEnumerator EndDialogTimer()
    {
        isDialogoAtivo = false;
        isTextComplete = true;
        animator.SetBool(animationstrings.IsDialogFinish, true);

        yield return new WaitForSeconds(0.1f);
        animator.SetBool(animationstrings.isDialog, false);

        yield return new WaitForSeconds(1f);
        UISavePoint.SetActive(true);
    }

    public void buttonDialog()
    {
        if (isTextComplete)
        {
            DisplayNextLinha();
        }
        else
        {
            StopAllCoroutines();
            dialogoArea.text = linhaAtual.linhaTexto;
            isTextComplete = true;
        }
    }
}
