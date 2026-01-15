using System.Collections;
using UnityEngine;

public class PuzzlePart : MonoBehaviour
{
    public PuzzlePieceType pieceType;
    public bool isCollected = false;
    public bool isPlaced = false;
    public bool isInPuzzleSlot = false;
    public Vector3 OffSetPosition;
    public Vector3 OffSetRotation;

    public bool progressBool = false;
    private float progressTake = 0f;
    private GameObject playerMovimentTarget;
    private Vector3 posicaoInicial;

    public Rigidbody2D rb;
    Vector3 pieceStartPos;
    Vector3 pieceTargetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerMovimentTarget = GameManager.instance.player.transform.GetChild(13).gameObject;
        posicaoInicial = playerMovimentTarget.transform.localPosition;
    }

    void Update()
    {
        if (progressBool && !isCollected && !isPlaced)
        {
            Collect();
        }

        // Só processa se a peça NÃO estiver permanentemente no slot do puzzle
        if (isCollected && GameManager.instance.player.entrar && !isPlaced && !isInPuzzleSlot)
        {
            // Verifica se o player está dentro de um PuzzleEnvolved (slot do puzzle)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(GameManager.instance.player.transform.position, 1f);
            bool isInPuzzleSlot = false;
            
            foreach (Collider2D col in colliders)
            {
                if (col.GetComponent<PuzzleEnvolved>() != null)
                {
                    isInPuzzleSlot = true;
                    break;
                }
            }

            if (!isInPuzzleSlot)
            {
                Place();
            }
        }
    }

    public void Collect()
    {
        GameManager.instance.player.animacao.SetBool("IsFilhote", true);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        pieceTargetPos = GameManager.instance.player.transform.position + OffSetPosition;
        pieceTargetPos.z = 0f;

        progressTake += Time.deltaTime * 2f; // velocidade da animação
        progressTake = Mathf.Clamp01(progressTake);

        transform.position = Vector3.Lerp(pieceStartPos, pieceTargetPos, progressTake);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(OffSetRotation),
            progressTake
        );

        if (progressTake >= 1f)
        {
            isCollected = true;
            progressBool = false;

            // Agora SIM pode virar filho do player
            transform.SetParent(GameManager.instance.player.transform);
            transform.localPosition = OffSetPosition;
        }
    }

    public void Place()
    {
        isPlaced = true;

        // Desvincula do player
        transform.SetParent(null);
        this.rb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(ResetTakeItem());
    }

    IEnumerator ResetTakeItem()
    {
        yield return new WaitForSeconds(0.5f);

        progressTake = 0f;
        isPlaced = false;
        isCollected = false;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.player.entrar && !isCollected && !progressBool && !isPlaced)
            {
                pieceStartPos = transform.position;
                progressTake = 0f;
                progressBool = true;

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
