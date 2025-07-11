using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos_Manager2 : MonoBehaviour
{
    public static Dialogos_Manager2 dialogos_Manager;
    public DialogoTextoRobert linhaAtual;
    private bool personagemAnteriorIsLility = false;

    [Header("Dialogos")]
    public string initialNameLeft = "Bebe Lility";
    public string initialNameRight;
    public Image iconeCaracterLeft;
    public Image iconeCaracterRight;
    public TextMeshProUGUI TitleLeft;
    public TextMeshProUGUI TitleRight;
    public TextMeshProUGUI dialogoArea;
    public GameObject UISavePoint;
    public bool setCheckpointUI = false;


    [Header("Situacoes Gerais")]
    public bool isTextComplete = false;
    public Queue<DialogoTextoRobert> linhas;
    public float tempoDeTransicao = 0.4f;
    public LevelTransicao levelTransicao;

    public bool isDialogoAtivo = false;
    public float speedTexto = 0.2f;
    public Animator animator;
    public PlayerBebe_Moviment playerMoviment;
    public GameObject Robert;
    public Transform alvoFinaly;
    public GameObject cadeiraLayer;

    void Start()
    {
        Robert = GameObject.Find("Robert");
        animator = GetComponent<Animator>();
        linhas = new Queue<DialogoTextoRobert>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        levelTransicao = GameObject.FindFirstObjectByType<LevelTransicao>();

        if (dialogos_Manager == null)
        {
            dialogos_Manager = this;
        }
    }

    public void StartDialogos(DialogosRobert dialogos)
    {
        isDialogoAtivo = true;

        animator.SetBool(animationstrings.isDialog, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, false);

        linhas.Clear();

        foreach (DialogoTextoRobert dialogoTexto in dialogos.dialogoTextos)
        {
            linhas.Enqueue(dialogoTexto);
        }

        DisplayNextLinha();
        iconeCaracterLeft.transform.localScale = linhaAtual.isLility ? new Vector3(1f, 1f, 1f) : new Vector3(0.65f, 0.65f, 0.65f);
        iconeCaracterRight.transform.localScale = linhaAtual.isLility ? new Vector3(0.65f, 0.65f, 0.65f) : new Vector3(1f, 1f, 1f);

        iconeCaracterLeft.color = linhaAtual.isLility ? new Color32(0xFF, 0xFF, 0xFF, 0xFF) : new Color32(63, 63, 63, 255);
        iconeCaracterRight.color = linhaAtual.isLility ? new Color32(63, 63, 63, 255) : new Color32(0xFF, 0xFF, 0xFF, 0xFF);
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

        initialNameRight = linhaAtual.caracter.nome;
        iconeCaracterRight.sprite = linhaAtual.caracter.icone;
        TitleLeft.text = initialNameLeft;
        TitleRight.text = initialNameRight;
        StopAllCoroutines();
        StartCoroutine(Sequencial(linhaAtual));

        bool isPersonagemMudou = (linhaAtual.isLility != personagemAnteriorIsLility);
        personagemAnteriorIsLility = linhaAtual.isLility;

        if (isPersonagemMudou)
        {
            StartCoroutine(ImagemTransform());
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

    IEnumerator ImagemTransform()
    {
        float t = 0f;
        Color corAlvoLeft, corAlvoRight;
        Vector3 escalaAlvoLeft, escalaAlvoRight;

        // Define os alvos conforme quem está falando
        if (linhaAtual.isLility)
        {
            corAlvoRight = new Color32(63, 63, 63, 255);
            escalaAlvoRight = new Vector3(0.70f, 0.70f, 0.70f);

            corAlvoLeft = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            escalaAlvoLeft = new Vector3(1f, 1f, 1f);
        }
        else
        {
            corAlvoLeft = new Color32(63, 63, 63, 255);
            escalaAlvoLeft = new Vector3(0.70f, 0.70f, 0.70f);

            corAlvoRight = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            escalaAlvoRight = new Vector3(1f, 1f, 1f);
        }

        Color corInicialLeft = iconeCaracterLeft.color;
        Color corInicialRight = iconeCaracterRight.color;
        Vector3 escalaInicialLeft = iconeCaracterLeft.transform.localScale;
        Vector3 escalaInicialRight = iconeCaracterRight.transform.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime / tempoDeTransicao;

            iconeCaracterLeft.color = Color.Lerp(corInicialLeft, corAlvoLeft, t);
            iconeCaracterLeft.transform.localScale = Vector3.Lerp(escalaInicialLeft, escalaAlvoLeft, t);

            iconeCaracterRight.color = Color.Lerp(corInicialRight, corAlvoRight, t);
            iconeCaracterRight.transform.localScale = Vector3.Lerp(escalaInicialRight, escalaAlvoRight, t);

            yield return null;
        }
    }

    public void EndDialogo()
    {
        Robert.GetComponent<Animator>().SetBool("isTalking", true);
        Robert.GetComponent<Animator>().SetBool("Abraco", true);
        isDialogoAtivo = false;
        isTextComplete = true;
        animator.SetBool(animationstrings.IsDialogFinish, true);
        playerMoviment.animacao.SetBool(animationstrings.canMove, true);

        GameObject npc = GameObject.FindGameObjectWithTag("Cervo");
        if (npc != null)
        {
            Dialogo_Trigger dialogoTrigger = npc.GetComponent<Dialogo_Trigger>();
            if (dialogoTrigger != null)
            {
                dialogoTrigger.NotificarDialogoFinalizado();
            }
        }

        StartCoroutine(lilityFinaly(alvoFinaly));
    }

    IEnumerator lilityFinaly(Transform alvo)
    {
        float pontoInicialX = playerMoviment.transform.position.x;
        float destinoX = alvo.position.x;
        float distanciaTotal = Mathf.Abs(destinoX - pontoInicialX);
        bool jaPulou = false;

        playerMoviment.canMove = false;
        playerMoviment.IsRight = true;

        yield return new WaitForSeconds(0.7f);

        Robert.GetComponent<Animator>().SetBool("isTalking", true);

        yield return new WaitForSeconds(0.3f);
        Robert.GetComponent<Animator>().SetBool("Abraco", true);
        yield return new WaitForSeconds(0.08f);
        cadeiraLayer.GetComponent<SpriteRenderer>().sortingOrder = 7;

        yield return new WaitForSeconds(1.7f);

        float distanciaMinima = 0.1f;
        float velocidade = playerMoviment.speed;

        while (Mathf.Abs(playerMoviment.transform.position.x - destinoX) > distanciaMinima)
        {
            float distanciaRestante = Mathf.Abs(playerMoviment.transform.position.x - destinoX);
            float percorrido = 1f - (distanciaRestante / distanciaTotal);

            // Pula quando estiver a 40% do caminho
            if (percorrido >= 0.53f && !jaPulou)
            {
                jaPulou = true;
                playerMoviment.animacao.SetTrigger(animationstrings.jump);
                playerMoviment.Jump();
            }

            // Movimentação horizontal com física, sem mexer no Y
            float direcaoX = Mathf.Sign(destinoX - playerMoviment.transform.position.x);
            playerMoviment.rb.linearVelocity = new Vector2(
                direcaoX * velocidade,
                playerMoviment.rb.linearVelocity.y
            );

            playerMoviment.IsMoving = true;

            yield return null;
        }

        // Parar movimento horizontal ao chegar
        playerMoviment.rb.linearVelocity = new Vector2(0f, playerMoviment.rb.linearVelocity.y);
        playerMoviment.IsMoving = false;

        Destroy(playerMoviment.gameObject);
        Debug.Log("Finalizando dialogo");

        yield return new WaitForSeconds(1.5f);
        levelTransicao.Transicao("Altior-Fuga");
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
