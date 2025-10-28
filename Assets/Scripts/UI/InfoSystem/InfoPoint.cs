using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InfoPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] public InfoData infoData;
    public bool isDialoguePoint = false;

    public bool PlayerAtivo = false;
    private bool PlayerEstaPerto = false;

    private Dialogo_Trigger dialogoTriggerOption;

    void Awake()
    {
        PlayerAtivo = false;
        PlayerEstaPerto = false;

        dialogoTriggerOption = GetComponent<Dialogo_Trigger>();
    }

    void Update()
    {
        dialogoTriggerOption.enabled = isDialoguePoint;

        if (isDialoguePoint)
        {
            if (Dialogos_Manager.dialogos_Manager.finishedDialogo)
            {
                GameManager.instance.player.canMove = false;
                InfoManager.instance.InfoReferenciaContainer.SetActive(true);
                InfoManager.instance.ButtonConfirmInfo.onClick.RemoveAllListeners();
                InfoManager.instance.ButtonConfirmInfo.onClick.AddListener(ButtonConfirmInfo);
                Dialogos_Manager.dialogos_Manager.finishedDialogo = false;
                return;
            }
        }
    }

    private void SubmitPressed()
    {
        if (!PlayerEstaPerto)
        {
            return;
        }

        if (!PlayerAtivo)
        {
            if (isDialoguePoint) return;
            if (InfoManager.instance.infosObtidas.Exists(i => i.id == infoData.id)) return;

            GameManager.instance.player.canMove = false;
            InfoManager.instance.InfoReferenciaContainer.SetActive(true);
            InfoManager.instance.ButtonConfirmInfo.onClick.RemoveAllListeners();
            InfoManager.instance.ButtonConfirmInfo.onClick.AddListener(ButtonConfirmInfo);
            PlayerAtivo = true;
        }
    }

    public void ButtonConfirmInfo()
    {
        InfoManager.instance.AdicionarInfo(infoData);
        InfoManager.instance.InfoReferenciaContainer.SetActive(false);
        PlayerAtivo = false;
        PlayerEstaPerto = false;

        GameManager.instance.player.canMove = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEstaPerto = true;

            if (!isDialoguePoint)
            {
                GameManagerInteract.Instance.interactIcon.transform.SetParent(transform);
                GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
                GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
            }
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

                if (!isDialoguePoint)
                {
                    GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
                    GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
                }
            }
        }
    }
}
