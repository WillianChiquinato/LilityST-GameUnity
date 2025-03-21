using TMPro;
using UnityEngine;

public class FilhotePegar : MonoBehaviour
{
    public FilhoteDragão filhoteDragão;
    public TextMeshPro textoFilhote;

    void Awake()
    {
        filhoteDragão = GameObject.FindFirstObjectByType<FilhoteDragão>();
        textoFilhote = GetComponentInChildren<TextMeshPro>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !filhoteDragão.FugaLility)
        {
            if (filhoteDragão.rb.linearVelocity.x == 0)
            {
                textoFilhote.text = "Pressione E para pegar o filhote";
                if (filhoteDragão.playerMoviment.entrar && filhoteDragão.rb.linearVelocity.x <= 0)
                {
                    Debug.Log("Pegou o filhote");
                    filhoteDragão.TimerFindObject = 0f;
                    filhoteDragão.playerMoviment.animacao.SetBool("IsFilhote", true);
                    filhoteDragão.LilityPegarFilhote = true;
                }
            }
            else
            {
                textoFilhote.text = "";
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            textoFilhote.text = "";
        }
    }
}
