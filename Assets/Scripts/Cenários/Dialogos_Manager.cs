using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos_Manager : MonoBehaviour
{
    public static Dialogos_Manager dialogos_Manager;

    public Image iconeCaracter;
    public TextMeshProUGUI nomeCaracter;
    public TextMeshProUGUI dialogoArea;

    //Igual listas, a unica diferença é que ele remove e adiciona sequencialmente
    [SerializeField]
    public Queue<DialogoTexto> linhas;

    public bool isDialogoAtivo = false;
    public float speedTexto = 0.2f;
    public Animator animator;
    public PlayerMoviment playerMoviment;

    void Start()
    {
        animator = GetComponent<Animator>();
        linhas = new Queue<DialogoTexto>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();

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
        if (linhas.Count == 0)
        {
            EndDialogo();
            return;
        }

        DialogoTexto linhaAtual = linhas.Dequeue();

        iconeCaracter.sprite = linhaAtual.caracter.icone;
        nomeCaracter.text = linhaAtual.caracter.nome;

        StopAllCoroutines();

        StartCoroutine(Sequencial(linhaAtual));
    }

    IEnumerator Sequencial(DialogoTexto dialogoTexto)
    {
        dialogoArea.text = "";
        foreach (char letter in dialogoTexto.linhaTexto.ToCharArray())
        {
            dialogoArea.text += letter;
            yield return new WaitForSeconds(speedTexto);
        }
    }

    public void EndDialogo()
    {
        //Final aqui das animações e reset.
        isDialogoAtivo = false;
        animator.SetBool(animationstrings.IsDialogFinish, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, true);
    }
}
