using System.Collections;
using TMPro;
using UnityEngine;

public class Cervinho_moviment : MonoBehaviour
{
    public GameObject animatorInteracao;
    public Animator animator;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entrou");
            animatorInteracao.SetActive(true);
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
        animatorInteracao.GetComponent<Animator>().SetBool("isInteracting", true);

        yield return new WaitForSeconds(0.3f);
        animatorInteracao.SetActive(false);
    }
}
