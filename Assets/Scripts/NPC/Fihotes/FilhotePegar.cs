using TMPro;
using UnityEngine;

public class FilhotePegar : MonoBehaviour
{
    public FilhoteDragão filhoteDragão;
    void Awake()
    {
        filhoteDragão = GameObject.FindFirstObjectByType<FilhoteDragão>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !filhoteDragão.FugaLility)
        {
            if (filhoteDragão.rb.linearVelocity.x == 0)
            {
                if (GameManager.instance.player.entrar && filhoteDragão.rb.linearVelocity.x <= 0)
                {
                    filhoteDragão.TimerFindObject = 0f;
                    GameManager.instance.player.animacao.SetTrigger("TakeObjeto");
                    filhoteDragão.LilityPegarFilhote = true;
                }
            }
            else
            {
                GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
                GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
