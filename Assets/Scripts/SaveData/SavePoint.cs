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
        if (GameManager.instance.playerMoviment == null)
        {
            GameManager.instance.playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
            if (GameManager.instance.playerMoviment != null)
            {
                playerParent = GameManager.instance.playerMoviment.transform.parent;
            }

        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Cenas modificação de spawn
        if (IsJsonFileEmpty("Assets/Scripts/SaveData/savepoint.json"))
        {
            if (SaveData.Instance.currentScene == "Altior-Quarto")
            {
                Debug.Log("Sem checkpoint, nao salvar nessa cena");
            }

            if (GameManager.instance.playerMoviment != null)
            {
                Transform playerChild = GameManager.instance.playerMoviment.transform; // Obtém o filho do playerParent
                playerChild.localPosition = Vector3.zero;

                if (GameManager.instance.playerMoviment.currentScene == "Altior-PreFuga")
                {
                    instance.defaultPosition = new Vector2(21.4f, 16.6f);
                    playerParent.transform.position = defaultPosition;
                }
                if (GameManager.instance.playerMoviment.currentScene == "Altior-Fuga")
                {
                    instance.defaultPosition = new Vector2(-53f, 16.6f);
                    playerParent.transform.position = defaultPosition;
                }
                if (GameManager.instance.playerMoviment.currentScene == "DimensaoTempo")
                {
                    instance.defaultPosition = new Vector2(-81.6f, 26f);
                    playerParent.transform.position = defaultPosition;
                }
                if (GameManager.instance.playerMoviment.currentScene == "MontanhaIntro")
                {
                    instance.defaultPosition = new Vector2(-85.7f, -25.5f);
                    playerParent.transform.position = defaultPosition;
                }
                if (GameManager.instance.playerMoviment.currentScene == "Boss&NPC")
                {
                    instance.defaultPosition = new Vector2(2.4f, -39.1f);
                    playerParent.transform.position = defaultPosition;
                }
            }
        }
        else
        {
            // Se o arquivo JSON não estiver vazio, carregar os dados salvos
            saveData = SaveManager.Load();
            if (saveData != null)
            {
                if (GameManager.instance.playerMoviment != null)
                {
                    // Se o jogador estiver na mesma cena, carregar os dados salvos
                    if (saveData.currentScene == SceneManager.GetActiveScene().name)
                    {
                        GameManager.instance.playerMoviment.transform.position = saveData.playerCheckpoint;
                    }
                    else
                    {
                        GameManager.instance.playerMoviment.transform.position = defaultPosition;

                        // Se o jogador estiver em uma cena diferente, usar a posição padrão
                        if (playerParent != null)
                        {
                            Transform playerChild = GameManager.instance.playerMoviment.transform;
                            playerChild.localPosition = Vector3.zero;

                            playerParent.position = GameManager.instance.playerMoviment.transform.position;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        if (GameManager.instance.playerMoviment == null)
        {
            GameManager.instance.playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        }
    }

    bool IsJsonFileEmpty(string path)
    {
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);

            return string.IsNullOrWhiteSpace(fileContent);
        }
        else
        {
            Debug.LogError("O arquivo JSON não foi encontrado.");
            return true;
        }
    }

    public void SaveCheckpoint(Vector2 checkpoint, int health, bool DashUnlocked, bool WalljumpUnlocked, bool attackUnlocked, List<PowerUps> powerUps, int XPlayer = 0)
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

            powerUps = new List<PowerUps>(powerUps)
        };

        SaveManager.Save(data);
    }

    public void LoadCheckpoint()
    {
        SaveData data = SaveManager.Load();
        if (data != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMoviment PlayerController = player.GetComponent<PlayerMoviment>();
                if (PlayerController != null)
                {
                    player.transform.position = data.playerCheckpoint;
                }

                Damage health = player.GetComponent<Damage>();
                if (health != null)
                {
                    health.Health = data.playerHealth;
                }

                Debug.Log("Checkpoint restaurado!");
            }
        }
        else
        {
            Debug.Log("Nenhum checkpoint salvo. Posição padrão usada.");
        }
    }
}