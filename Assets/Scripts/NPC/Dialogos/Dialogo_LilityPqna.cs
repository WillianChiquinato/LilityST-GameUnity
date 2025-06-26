using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerRobert
{
    public string nome;
    public Sprite icone;
}

[System.Serializable]
public class DialogoTextoRobert
{
    public DialogoTriggerRobert caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
    public bool isLility = false;
}

[System.Serializable]
public class DialogosRobert
{
    public List<DialogoTextoRobert> dialogoTextos = new List<DialogoTextoRobert>();
}

public class Dialogo_LilityPqna : MonoBehaviour
{
    public DialogosRobert dialogos;
    public PlayerBebe_Moviment playerBebe;
    public GameObject robert;
    public Animator animator;
    public bool AjusteAnimation = false;

    [Header("Alvo Dialog")]
    public Transform alvoDialog;
    public Vector3 offset;

    private void Start()
    {
        playerBebe = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        animator = GetComponent<Animator>();
        robert = GameObject.Find("Robert");
    }

    void Update()
    {
        if (AjusteAnimation)
        {
            StartCoroutine(lilityAjusteAnim(alvoDialog));
        }
    }

    IEnumerator lilityAjusteAnim(Transform alvo)
    {
        AjusteAnimation = false;
        playerBebe.canMove = false;
        playerBebe.IsRight = true;

        yield return new WaitForSeconds(1f);

        Vector2 direcao = (alvo.position - playerBebe.transform.position).normalized;
        if (direcao.x >= 0)
        {
            playerBebe.IsRight = true;
        }
        else
        {
            playerBebe.IsRight = false;
        }

        float distanciaMinima = 0.1f;
        float velocidade = playerBebe.speed;

        while (Mathf.Abs(playerBebe.transform.position.x - alvo.position.x) > distanciaMinima)
        {
            Vector3 novaPosicao = new Vector3(
                Mathf.MoveTowards(playerBebe.transform.position.x, alvo.position.x, velocidade * Time.deltaTime),
                playerBebe.transform.position.y,
                playerBebe.transform.position.z
            );
            playerBebe.transform.position = novaPosicao;
            playerBebe.IsMoving = true;

            yield return null;
        }

        playerBebe.IsMoving = false;
        robert.GetComponent<Animator>().SetBool("isTalking", true);
        yield return new WaitForSeconds(0.5f);
        playerBebe.IsRight = true;

        yield return new WaitForSeconds(2f);
        TriggerDialogo();

        yield return new WaitForSeconds(1.5f);
        robert.GetComponent<Animator>().SetBool("isTalking", false);
    }

    public void TriggerDialogo()
    {
        Dialogos_Manager2.dialogos_Manager.StartDialogos(dialogos);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + offset;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        if (collision.CompareTag("Player") && playerBebe.entrar == true)
        {
            playerBebe.camerafollowObject.transposer.m_TrackedObjectOffset = new Vector3(1.4f, 0.6f, 0);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
            this.GetComponent<BoxCollider2D>().enabled = false;
            AjusteAnimation = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
    }
}
