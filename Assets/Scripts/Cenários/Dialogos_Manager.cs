using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos_Manager : MonoBehaviour
{
    public static Dialogos_Manager dialogos_Manager;

    public DialogoTexto linhaAtual;
    public Dialogo_Trigger dialogo_Trigger;
    public GameObject ImagemRotate;

    public bool isLeft;
    public Image iconeCaracter;
    public TextMeshProUGUI nomeCaracter;
    public TextMeshProUGUI dialogoArea;
    public RectTransform rectTransform;
    public bool isTextComplete = false;

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
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        dialogo_Trigger = GameObject.FindFirstObjectByType<Dialogo_Trigger>();

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

        iconeCaracter.sprite = linhaAtual.caracter.icone;
        nomeCaracter.text = linhaAtual.caracter.nome;
        isLeft = linhaAtual.caracter.isLeft;

        Debug.Log($"isLeft: {isLeft}");

        if (isLeft)
        {
            //Area de NPC.
            ImagemRotate.transform.localScale = new Vector3(-5, 5, 5);
            rectTransform.offsetMin = new Vector2(764, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(-493, rectTransform.offsetMax.y);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 428);
            Debug.Log("Oi");
        }
        else
        {
            ImagemRotate.transform.localScale = new Vector3(9, 9, 9);
            rectTransform.offsetMin = new Vector2(450.4805f, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(-806.5195f, rectTransform.offsetMax.y);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 649);
        }

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
        isTextComplete = true;
    }

    public void EndDialogo()
    {
        //Final aqui das animações e reset.
        isDialogoAtivo = false;
        isTextComplete = true;
        animator.SetBool(animationstrings.IsDialogFinish, true);
        dialogo_Trigger.animator.SetBool(animationstrings.InicioDialogo, false);
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
            StopAllCoroutines();
            dialogoArea.text = linhaAtual.linhaTexto;
            isTextComplete = true;
        }
    }
}
