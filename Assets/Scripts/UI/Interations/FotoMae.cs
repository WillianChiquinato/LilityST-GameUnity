using UnityEngine;
using UnityEngine.UI;

public class FotoMae : MonoBehaviour
{
    public GameObject FotoContainer;

    public Animator FotoAnim;
    public Animator BtnFotoAnim;
    public Button BtnFoto;

    void Awake()
    {
        FotoAnim = FotoContainer.transform.GetChild(1).GetComponent<Animator>();
        BtnFotoAnim = FotoContainer.transform.GetChild(0).GetComponent<Animator>();

        Debug.Log(FotoAnim != null ? "FotoAnim OK" : "FotoAnim NULL");
        Debug.Log(BtnFotoAnim != null ? "BtnFotoAnim OK" : "BtnFotoAnim NULL");
    }

    void Start()
    {
        FotoContainer.SetActive(false);
    }

    public void MostrarFoto()
    {
        FotoContainer.SetActive(true);
    }

    public void FecharFoto()
    {
        Debug.Log("FecharFoto called");
        FotoContainer.SetActive(true);
        FotoAnim.SetTrigger("FecharFoto");
        BtnFotoAnim.SetTrigger("BtnFecharFoto");
        Invoke(nameof(DesativarFoto), 0.8f);
    }

    private void DesativarFoto()
    {
        FotoContainer.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BtnFoto.onClick.RemoveAllListeners();
            BtnFoto.onClick.AddListener(FecharFoto);
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 0.8f, 0);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
            if (GameManagerInteract.Instance.player.entrar)
            {
                MostrarFoto();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
