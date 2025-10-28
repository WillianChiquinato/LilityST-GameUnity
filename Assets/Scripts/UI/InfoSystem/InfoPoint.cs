using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InfoPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] public InfoData infoData;
    public bool isDialoguePoint = false;

    public bool PlayerAtivo = false;
    private bool PlayerEstaPerto = false;

    void Awake()
    {
        PlayerAtivo = false;
        PlayerEstaPerto = false;
    }

    private void SubmitPressed()
    {
        if (!PlayerEstaPerto)
        {
            return;
        }

        if (!PlayerAtivo)
        {
            if (isDialoguePoint)
            {
                return;
            }

            PlayerAtivo = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEstaPerto = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.player.entrar && !PlayerAtivo)
            {
                SubmitPressed();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!PlayerAtivo)
            {
                PlayerEstaPerto = false;
            }
        }
    }
}
