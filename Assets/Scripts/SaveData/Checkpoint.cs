using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [Header("Checkpoints")]
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

                    SaveManager.Save(SaveData.Instance, GameManager.currentSaveSlot);
                }
                else
                {
                    Debug.LogError("Savepoint.instance ou player está null. O checkpoint não foi salvo.");
                }

                StartCoroutine(AutoMoveSave());
            }

            // Se o cervinho está no trigger, ativa a UI do diálogo
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
    }

    IEnumerator AutoMoveSave()
    {
        isMovingAutomatically = true;
        GameManager.instance.player.canMove = false;
        if (direcao == -1)
        {
            GameManager.instance.player.IsRight = true;
        }
        else
        {
            GameManager.instance.player.IsRight = false;
        }

        while (paiCheckpoint.transform.position.x != GameManager.instance.player.transform.position.x)
        {
            float step = GameManager.instance.player.speed * Time.deltaTime;
            GameManager.instance.player.transform.position = Vector3.MoveTowards(GameManager.instance.player.transform.position, new Vector3(paiCheckpoint.transform.position.x, GameManager.instance.player.transform.position.y, GameManager.instance.player.transform.position.z), step);
            GameManager.instance.player.IsMoving = true;
            yield return null;
        }
        GameManager.instance.player.IsMoving = false;

        yield return new WaitForSeconds(0.7f);
        GameManager.instance.player.animacao.SetBool("Checkpoint", true);
        GameManager.instance.player.IsRight = false;

        isMovingAutomatically = true;

        yield return new WaitForSeconds(0.5f);
        animTicket.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        CutSumir.SetActive(false);
        framingPosition.m_TrackedObjectOffset = new Vector3(-5, 0, 0);

        yield return new WaitForSeconds(0.5f);
        GameManager.instance.UISavePoint.SetActive(true);

        yield return new WaitForSeconds(2.1f);
        animTicket.gameObject.SetActive(false);
    }
}