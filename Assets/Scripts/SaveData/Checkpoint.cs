using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public Sistema_Pause gameManager;

    [Header("Checkpoints")]
    public List<Collider2D> collidersNoTrigger = new List<Collider2D>();
    public bool playerNoTrigger = false;
    public bool cervoNoTrigger = false;
    public GameObject paiCheckpoint;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;

    [Header("Player e variaveis")]
    [SerializeField] private PlayerMoviment player;
    public bool isMovingAutomatically = false;
    public float direcao;
    public bool triggerCheckpoint;
    public GameObject CutSumir;

    void Awake()
    {
        gameManager = GameObject.FindFirstObjectByType<Sistema_Pause>();
        paiCheckpoint = transform.parent != null ? transform.parent.gameObject : gameObject;

        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        CutSumir = GameObject.FindGameObjectWithTag("Sumir");
        player = GameObject.FindFirstObjectByType<PlayerMoviment>();

        if (player == null)
        {
            Debug.LogError("PlayerMoviment não encontrado! Verifique se o Player tem esse componente.");
        }
    }

    void Update()
    {
        direcao = (player.transform.position.x - transform.position.x) > 0 ? 1f : -1f;

        if (gameManager.UISavePoint.activeSelf == true)
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
            if (player.entrar)
            {
                if (Savepoint.instance != null && player != null)
                {
                    Savepoint.instance.SaveCheckpoint(
                        transform.position,
                        player.GetComponent<Damage>().maxHealth,
                        SaveData.Instance.CameraCorrected,
                        SaveData.Instance.DashUnlocked,
                        SaveData.Instance.WalljumpUnlocked,
                        SaveData.Instance.JumpUnlocked,
                        SaveData.Instance.attackUnlocked,
                        SaveData.Instance.powerUps
                    );
                    inventory_System.instance.SaveInventory();
                    Debug.Log("Checkpoint salvo na posição: " + transform.position);
                }
                else
                {
                    Debug.LogError("Savepoint.instance ou player está null. O checkpoint não foi salvo.");
                }

                StartCoroutine(AutoMoveSave());
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
        player.canMove = false;
        if (direcao == -1)
        {
            player.IsRight = true;
        }
        else
        {
            player.IsRight = false;
        }

        while (paiCheckpoint.transform.position.x != player.transform.position.x)
        {
            float step = player.speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(paiCheckpoint.transform.position.x, player.transform.position.y, player.transform.position.z), step);
            player.IsMoving = true;
            yield return null;
        }
        player.IsMoving = false;

        yield return new WaitForSeconds(0.7f);
        player.animacao.SetBool("Checkpoint", true);
        player.IsRight = false;

        isMovingAutomatically = true;

        yield return new WaitForSeconds(1.5f);
        CutSumir.SetActive(false);
        framingPosition.m_TrackedObjectOffset = new Vector3(-5, 0, 0);

        if (!cervoNoTrigger)
        {
            yield return new WaitForSeconds(1f);
            gameManager.UISavePoint.SetActive(true);
        }
    }
}