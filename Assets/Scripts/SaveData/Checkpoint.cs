using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [Header("Checkpoints")]
    public Vector2 offSetCheckpointIcon;

    public List<Collider2D> collidersNoTrigger = new List<Collider2D>();
    public bool playerNoTrigger = false;
    public bool cervoNoTrigger = false;
    public GameObject paiCheckpoint;
    public Animator animTicket;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;

    [Header("Player e variaveis")]
    public bool isMovingAutomatically = false;
    public float direcao;
    public bool triggerCheckpoint;
    public GameObject CutSumir;

    void Awake()
    {
        paiCheckpoint = transform.parent != null ? transform.parent.gameObject : gameObject;

        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        CutSumir = GameObject.FindGameObjectWithTag("Sumir");
    }

    void Start()
    {
        if (GameManager.instance.player == null)
        {
            Debug.LogError("PlayerMoviment não encontrado! Verifique se o Player tem esse componente.");
        }
        animTicket.gameObject.SetActive(false);
    }

    void Update()
    {
        direcao = (GameManager.instance.player.transform.position.x - transform.position.x) > 0 ? 1f : -1f;

        if (GameManager.instance.UISavePoint.activeSelf == true)
        {
            triggerCheckpoint = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(transform);
            GameManagerInteract.Instance.interactIcon.transform.localPosition = offSetCheckpointIcon;
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collidersNoTrigger.Contains(collision))
        {
            collidersNoTrigger.Add(collision);
        }

        playerNoTrigger = collidersNoTrigger.Exists(col => col.CompareTag("Player"));
        cervoNoTrigger = collidersNoTrigger.Exists(col => col.CompareTag("Cervo"));

        if (playerNoTrigger)
        {
            if (GameManager.instance.player.entrar)
            {
                if (Savepoint.instance != null && GameManager.instance.player != null)
                {
                    Savepoint.instance.SaveCheckpoint(
                        SaveData.Instance.playTime,
                        transform.position,
                        GameManager.instance.player.GetComponent<Damage>().maxHealth,
                        SaveData.Instance.DashUnlocked,
                        SaveData.Instance.WalljumpUnlocked,
                        SaveData.Instance.powerUps,
                        SaveData.Instance.XPlayer
                    );
                    inventory_System.instance.SaveInventory();
                    QuestManager.instance.SaveAllQuests();
                    FragmentoSystem.instance.SaveFragment();
                    InfoManager.instance.SaveAllInfos();

                    SaveManager.Save(SaveData.Instance, GameManager.currentSaveSlot);

                    GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
                    GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
                }
                else
                {
                    Debug.LogError("Savepoint.instance ou player está null. O checkpoint não foi salvo.");
                }

                StartCoroutine(AutoMoveSave(paiCheckpoint.transform.position.x, direcao < 0));
            }

            // Se o cervinho está no trigger, ativa a UI do diálogo.
            if (cervoNoTrigger)
            {
                GameManager.instance.UISavePoint.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                GameManager.instance.UISavePoint.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collidersNoTrigger.Contains(collision))
        {
            collidersNoTrigger.Remove(collision);
        }

        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }

    IEnumerator AutoMoveSave(float targetX, bool faceRightToCheckpoint)
    {
        isMovingAutomatically = true;
        GameManager.instance.player.AutoMoveAnimations = true;
        GameManager.instance.player.canMove = false;
        Transform playerTransform = GameManager.instance.player.transform;
        Rigidbody2D playerRb = GameManager.instance.player.rb;
        GameManager.instance.player.IsRight = faceRightToCheckpoint;
        
        GameManager.instance.player.moveInput = Vector2.zero;
        GameManager.instance.player.rb.linearVelocity = new Vector2(0f, GameManager.instance.player.rb.linearVelocity.y);

        while (Mathf.Abs(playerRb.position.x - targetX) > 0.05f)
        {
            float newX = Mathf.MoveTowards(playerRb.position.x, targetX, GameManager.instance.player.maxSpeed * Time.fixedDeltaTime);
            playerRb.MovePosition(new Vector2(newX, playerRb.position.y));

            GameManager.instance.player.IsMoving = true;
            yield return null;
        }

        GameManager.instance.player.rb.linearVelocity = new Vector2(0f, GameManager.instance.player.rb.linearVelocity.y);
        GameManager.instance.player.moveInput = Vector2.zero;
        GameManager.instance.player.IsMoving = false;
        GameManager.instance.player.IsRight = faceRightToCheckpoint;
        playerTransform.position = new Vector3(
            targetX,
            playerTransform.position.y,
            playerTransform.position.z
        );

        yield return new WaitForSeconds(0.7f);
        GameManager.instance.player.animacao.SetBool("Checkpoint", true);
        GameManager.instance.player.IsRight = false;

        isMovingAutomatically = false;
        GameManager.instance.player.AutoMoveAnimations = false;

        yield return new WaitForSeconds(0.5f);
        animTicket.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        CutSumir.SetActive(false);
        framingPosition.m_TrackedObjectOffset = new Vector3(-5, 0, 0);

        yield return new WaitForSeconds(0.5f);
        GameManager.instance.UISavePoint.SetActive(true);

        yield return new WaitForSeconds(1.85f);
        animTicket.gameObject.SetActive(false);
    }
}