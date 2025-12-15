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
            SetDefaultSpawnPosition();
            Debug.LogWarning("Checkpoint não encontrado, posição padrão aplicada, nem o slot achou");
        }
        else
        {
            if (SaveData.Instance != null && GameManager.instance.playerMoviment != null)
            {
                if (SaveData.Instance.currentScene == SceneManager.GetActiveScene().name)
                {
                    // Se a cena é a mesma, coloca no checkpoint salvo
                    GameManager.instance.playerMoviment.transform.position = SaveData.Instance.playerCheckpoint;
                }
                else
                {
                    SetDefaultSpawnPosition();
                }
            }
            else
            {
                //Fallback.
                SetDefaultSpawnPosition();
                Debug.LogWarning("Fallback: Dados de salvamento não encontrados.");
            }
        }
    }

    private void SetDefaultSpawnPosition()
    {
        if (GameManager.instance.playerMoviment == null) return;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Altior-PreFuga":
                instance.defaultPosition = new Vector2(21.4f, 16.6f);
                break;
            case "Altior-Fuga":
                instance.defaultPosition = new Vector2(-16.19f, 16.6f);
                break;
            case "DimensaoTempo":
                instance.defaultPosition = new Vector2(-81.6f, 26f);
                break;
            case "SopeInicial":
                instance.defaultPosition = new Vector2(-95.1f, 25.7f);
                break;
            case "Boss&NPC":
                instance.defaultPosition = new Vector2(2.4f, -39.1f);
                break;
            default:
                instance.defaultPosition = new Vector2(GameManager.instance.playerMoviment.transform.position.x, GameManager.instance.playerMoviment.transform.position.y);
                break;
        }

        GameManager.instance.playerMoviment.transform.position = instance.defaultPosition;
    }


    void Update()
    {
        if (GameManager.instance.playerMoviment == null)
        {
            GameManager.instance.playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        }
    }

    public void SaveCheckpoint(float playTime, Vector2 checkpoint, int health, bool DashUnlocked, bool WalljumpUnlocked, List<PowerUps> powerUps, int XPlayer = 0)
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
        currentData.powerUps = new List<PowerUps>(powerUps);
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
        LoadCheckpoint();
    }

    private void LoadCheckpoint()
    {
        Debug.LogWarning("Slot: " + GameManager.currentSaveSlot);
        if (GameManager.instance.playerMoviment == null) return;

        if (!SaveManager.SaveExists(GameManager.currentSaveSlot))
        {
            SetDefaultSpawnPosition();
        }
        else
        {
            saveData = SaveManager.Load(GameManager.currentSaveSlot);

            if (saveData.currentScene == SceneManager.GetActiveScene().name)
            {
                GameManager.instance.playerMoviment.transform.position = saveData.playerCheckpoint;

                Damage health = GameManager.instance.playerMoviment.GetComponent<Damage>();
                if (health != null)
                {
                    health.Health = saveData.playerHealth;
                }
            }
            else
            {
                SetDefaultSpawnPosition();
            }
        }
    }
}