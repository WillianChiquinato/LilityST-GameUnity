using System.Collections;
using TMPro;
using UnityEngine;

public class Cervinho_moviment : MonoBehaviour
{
    public TextMeshPro textoEnter;
    public GameObject animatorInteracao;
    public Animator animator;


    void Start()
    {
        animatorInteracao.SetActive(false);
        textoEnter.text = "";
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entrou");
            animatorInteracao.SetActive(true);
            textoEnter.text = "Pressione E para interagir";
            animatorInteracao.GetComponent<Animator>().SetBool("isInteracting", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TimerExit());
        }
    }

    IEnumerator TimerExit()
    {
        textoEnter.text = "";
        animatorInteracao.GetComponent<Animator>().SetBool("isInteracting", true);

        yield return new WaitForSeconds(1f);
        animatorInteracao.SetActive(false);
    }
}
