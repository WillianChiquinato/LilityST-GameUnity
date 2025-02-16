using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos_Manager2 : MonoBehaviour
{
    public static Dialogos_Manager2 dialogos_Manager;

    public DialogoTextoRobert linhaAtual;
    public Dialogo_LilityPqna dialogo_Trigger;

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
    [SerializeField] public Queue<DialogoTextoRobert> linhas;
    public string sceneName;
    private LevelTransicao transicao;

    public bool isDialogoAtivo = false;
    public float speedTexto = 0.2f;
    public Animator animator;
    public PlayerBebe_Moviment playerBebe;

    void Start()
    {
        animator = GetComponent<Animator>();
        linhas = new Queue<DialogoTextoRobert>();
        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        playerBebe = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        dialogo_Trigger = GameObject.FindFirstObjectByType<Dialogo_LilityPqna>();

        if (dialogos_Manager == null)
        {
            dialogos_Manager = this;
        }
    }

    public void StartDialogos(DialogosRobert dialogos)
    {
        isDialogoAtivo = true;

        animator.SetBool(animationstrings.isDialog, true);
        playerBebe.animacao.SetBool(animationstrings.canMove, false);

        linhas.Clear();

        foreach (DialogoTextoRobert dialogoTexto in dialogos.dialogoTextos)
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

    IEnumerator Sequencial(DialogoTextoRobert dialogoTexto)
    {
        dialogoArea.text = "";
        foreach (char letter in dialogoTexto.linhaTexto.ToCharArray())
        {
            dialogoArea.text += letter;
            yield return new WaitForSeconds(speedTexto);
        }
        isTextComplete = true;
    }

    IEnumerator Sequencial2(DialogoTextoRobert dialogoTexto)
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
        StartCoroutine(DelayTransition());
    }

    IEnumerator DelayTransition()
    {
        playerBebe.animacao.SetBool(animationstrings.canMove, false);

        yield return new WaitForSeconds(1);

        transicao.Transicao(sceneName);
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
