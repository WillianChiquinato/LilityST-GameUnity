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

        // Verifica se o save do slot existe
        if (!SaveManager.SaveExists(currentSlot))
        {
            // Nenhum save ainda, usa posição padrão
            SetDefaultSpawnPosition();
            Debug.LogWarning("Checkpoint não encontrado, posição padrão aplicada, nem o slot achou");
        }
        else
        {
            // Carrega dados do slot
            saveData = SaveManager.Load(currentSlot);

            if (saveData != null && GameManager.instance.playerMoviment != null)
            {
                if (saveData.currentScene == SceneManager.GetActiveScene().name)
                {
                    // Se a cena é a mesma, coloca no checkpoint salvo
                    GameManager.instance.playerMoviment.transform.position = saveData.playerCheckpoint;
                }
                else
                {
                    SetDefaultSpawnPosition();
                }
            }
            else
            {
                //Fallback.
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
                instance.defaultPosition = new Vector2(-53f, 16.6f);
                break;
            case "DimensaoTempo":
                instance.defaultPosition = new Vector2(-81.6f, 26f);
                break;
            case "MontanhaIntro":
                instance.defaultPosition = new Vector2(-85.7f, -25.5f);
                break;
            case "Boss&NPC":
                instance.defaultPosition = new Vector2(2.4f, -39.1f);
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

    public void SaveCheckpoint(float playTime, Vector2 checkpoint, int health, bool DashUnlocked, bool WalljumpUnlocked, bool attackUnlocked, List<PowerUps> powerUps, int XPlayer = 0)
    {
        SaveData data = new SaveData
        {
            playerCheckpoint = checkpoint,
            playerHealth = health,
            currentScene = SceneManager.GetActiveScene().name,
            DashUnlocked = DashUnlocked,
            WalljumpUnlocked = WalljumpUnlocked,
            attackUnlocked = attackUnlocked,
            XPlayer = XPlayer,
            powerUps = new List<PowerUps>(powerUps),
            playTime = playTime
        };
        Debug.LogWarning("Salvando no slot " + GameManager.currentSaveSlot);

        SaveManager.Save(data, GameManager.currentSaveSlot);
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