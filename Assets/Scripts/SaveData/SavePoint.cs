using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;

public class Savepoint : MonoBehaviour
{
    public static Savepoint instance;
    public SaveData saveData = new SaveData();

    public Vector2 defaultPosition = Vector2.zero;
    private Transform playerParent;
    private bool pendingCheckpointLoad;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        int currentSlot = GameManager.currentSaveSlot;

        //Carrega os itens do SaveData pre setados.
        SaveData.Instance = SaveManager.Load(currentSlot);

        // Verifica se o save do slot existe
        if (!SaveManager.SaveExists(currentSlot))
        {
            // Nenhum save ainda, usa posição padrão
            pendingCheckpointLoad = !TrySetDefaultSpawnPosition();
            Debug.LogWarning("Checkpoint não encontrado, posição padrão aplicada, nem o slot achou");
        }
        else
        {
            if (SaveData.Instance != null)
            {
                pendingCheckpointLoad = !TryApplyInitialSpawn();
            }
            else
            {
                //Fallback.
                pendingCheckpointLoad = !TrySetDefaultSpawnPosition();
                Debug.LogWarning("Fallback: Dados de salvamento não encontrados.");
            }
        }
    }

    private bool TrySetDefaultSpawnPosition()
    {
        PlayerMoviment playerMoviment = GetPlayerMoviment();
        if (playerMoviment == null) return false;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Altior-PreFuga":
                instance.defaultPosition = new Vector2(42.2f, 15.61f);
                break;
            case "Altior-Fuga":
                instance.defaultPosition = new Vector2(-16.19f, 16.6f);
                break;
            case "DimensaoTempo":
                instance.defaultPosition = new Vector2(-81.6f, 26f);
                break;
            case "SopeInicial":
                instance.defaultPosition = new Vector2(-95.1f, 27.7f);
                break;
            case "Boss&NPC":
                instance.defaultPosition = new Vector2(2.4f, -39.1f);
                break;
            default:
                instance.defaultPosition = new Vector2(playerMoviment.transform.position.x, playerMoviment.transform.position.y);
                break;
        }

        playerMoviment.transform.position = instance.defaultPosition;
        return true;
    }


    void Update()
    {
        PlayerMoviment playerMoviment = GetPlayerMoviment();

        if (pendingCheckpointLoad && playerMoviment != null)
        {
            pendingCheckpointLoad = !LoadCheckpoint();
        }

        if (GameManager.instance != null && GameManager.instance.playerMoviment == null && playerMoviment != null)
        {
            GameManager.instance.playerMoviment = playerMoviment;
        }
    }

    public void SaveCheckpoint(float playTime, Vector2 checkpoint, int health, bool DashUnlocked, bool WalljumpUnlocked, List<PowerUpData> powerUps, int XPlayer = 0)
    {
        // 1. Carregar o save atual do slot
        var currentData = SaveManager.Load(GameManager.currentSaveSlot);
        if (currentData == null)
        {
            currentData = new SaveData();
        }

        // 2. Atualizar só os campos do checkpoint
        currentData.playerCheckpoint = checkpoint;
        currentData.playerHealth = health;
        currentData.currentScene = SceneManager.GetActiveScene().name;
        currentData.DashUnlocked = DashUnlocked;
        currentData.WalljumpUnlocked = WalljumpUnlocked;
        currentData.XPlayer = XPlayer;
        currentData.powerUps = new List<PowerUpData>(powerUps);
        currentData.playTime = playTime;

        // 3. Salvar de volta (preservando inventário e tudo mais)
        SaveManager.Save(currentData, GameManager.currentSaveSlot);

        // 4. Atualizar também o Instance em memória
        SaveData.Instance = currentData;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Atualiza o player ao carregar a cena
        pendingCheckpointLoad = true;
        LoadCheckpoint();
    }

    private bool LoadCheckpoint()
    {
        Debug.LogWarning("Slot: " + GameManager.currentSaveSlot);
        PlayerMoviment playerMoviment = GetPlayerMoviment();
        if (playerMoviment == null) return false;

        if (!SaveManager.SaveExists(GameManager.currentSaveSlot))
        {
            pendingCheckpointLoad = !TrySetDefaultSpawnPosition();
            return !pendingCheckpointLoad;
        }
        else
        {
            saveData = SaveManager.Load(GameManager.currentSaveSlot);

            if (saveData.currentScene == SceneManager.GetActiveScene().name)
            {
                playerMoviment.transform.position = saveData.playerCheckpoint;

                Damage health = playerMoviment.GetComponent<Damage>();
                if (health != null)
                {
                    health.Health = saveData.playerHealth;
                }

                pendingCheckpointLoad = false;
                return true;
            }
            else
            {
                pendingCheckpointLoad = !TrySetDefaultSpawnPosition();
                return !pendingCheckpointLoad;
            }
        }
    }

    private bool TryApplyInitialSpawn()
    {
        if (SaveData.Instance.currentScene == SceneManager.GetActiveScene().name)
        {
            PlayerMoviment playerMoviment = GetPlayerMoviment();
            if (playerMoviment == null) return false;

            playerMoviment.transform.position = SaveData.Instance.playerCheckpoint;
            return true;
        }

        return TrySetDefaultSpawnPosition();
    }

    private PlayerMoviment GetPlayerMoviment()
    {
        PlayerMoviment playerMoviment = null;

        if (GameManager.instance != null)
        {
            playerMoviment = GameManager.instance.playerMoviment;
        }

        if (playerMoviment == null)
        {
            playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        }

        if (GameManager.instance != null && GameManager.instance.playerMoviment == null && playerMoviment != null)
        {
            GameManager.instance.playerMoviment = playerMoviment;
        }

        return playerMoviment;
    }
}