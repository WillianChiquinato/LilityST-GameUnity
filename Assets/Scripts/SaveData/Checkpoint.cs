using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] private PlayerMoviment player;

    void Awake()
    {
        player = GameObject.FindFirstObjectByType<PlayerMoviment>();

        if (player == null)
        {
            Debug.LogError("PlayerMoviment não encontrado! Verifique se o Player tem esse componente.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
                    SaveData.Instance.attackUnlocked
                );
                Debug.Log("Checkpoint salvo na posição: " + transform.position);
            }
            else
            {
                Debug.LogError("Savepoint.instance ou player está null. O checkpoint não foi salvo.");
            }
        }
    }
}