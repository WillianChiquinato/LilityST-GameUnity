using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos_Manager : MonoBehaviour
{
    public static Dialogos_Manager dialogos_Manager;

    public DialogoTexto linhaAtual;

    public bool isUpDialog;

    [Header("Dialogo1")]
    public string initialName1;
    public Image iconeCaracter;
    public TextMeshProUGUI nomeCaracter;
    public TextMeshProUGUI dialogoArea;

    [Header("Dialogo2")]
    public string initialName2;
    public Image iconeCaracter2;
    public TextMeshProUGUI nomeCaracter2;
    public TextMeshProUGUI dialogoArea2;

    public bool isTextComplete = false;

    [Header("Situacoes Gerais")]
    [SerializeField] public Queue<DialogoTexto> linhas;

    public bool isDialogoAtivo = false;
    public float speedTexto = 0.2f;
    public Animator animator;
    public PlayerMoviment playerMoviment;

    void Start()
    {
        animator = GetComponent<Animator>();
        linhas = new Queue<DialogoTexto>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();

        if (dialogos_Manager == null)
        {
            dialogos_Manager = this;
        }
    }

    public void StartDialogos(Dialogos dialogos)
    {
        isDialogoAtivo = true;

        animator.SetBool(animationstrings.isDialog, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, false);

        linhas.Clear();

        foreach (DialogoTexto dialogoTexto in dialogos.dialogoTextos)
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
        isUpDialog = linhaAtual.caracter.isUpDialog;

        nomeCaracter.text = initialName1;
        nomeCaracter2.text = initialName2;

        if (isUpDialog)
        {
            iconeCaracter.sprite = linhaAtual.caracter.icone;
            nomeCaracter.text = linhaAtual.caracter.nome;
            StopAllCoroutines();
            StartCoroutine(Sequencial(linhaAtual));

            dialogoArea2.text = "...";
        }
        else
        {
            iconeCaracter2.sprite = linhaAtual.caracter.icone;
            nomeCaracter2.text = linhaAtual.caracter.nome;
            StopAllCoroutines();
            StartCoroutine(Sequencial2(linhaAtual));

            dialogoArea.text = "...";
        }
    }

    IEnumerator Sequencial(DialogoTexto dialogoTexto)
    {
        dialogoArea.text = "";
        foreach (char letter in dialogoTexto.linhaTexto.ToCharArray())
        {
            dialogoArea.text += letter;
            yield return new WaitForSeconds(speedTexto);
        }
        isTextComplete = true;
    }

    IEnumerator Sequencial2(DialogoTexto dialogoTexto)
    {
        dialogoArea2.text = "";
        foreach (char letter in dialogoTexto.linhaTexto.ToCharArray())
        {
            dialogoArea2.text += letter;
            yield return new WaitForSeconds(speedTexto);
        }
        isTextComplete = true;
    }

    public void EndDialogo()
    {
        isDialogoAtivo = false;
        isTextComplete = true;
        animator.SetBool(animationstrings.IsDialogFinish, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, true);
    }

    public void buttonDialog()
    {
        if (isTextComplete)
        {
            DisplayNextLinha();
        }
        else
        {
            if (isUpDialog)
            {
                StopAllCoroutines();
                dialogoArea.text = linhaAtual.linhaTexto;
                isTextComplete = true;
            }
            else
            {
                StopAllCoroutines();
                dialogoArea2.text = linhaAtual.linhaTexto;
                isTextComplete = true;
            }
        }
    }
}
