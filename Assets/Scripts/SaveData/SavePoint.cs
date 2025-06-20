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
    public PlayerMoviment playerMoviment;
    private Transform playerParent;

    private void Awake()
    {
        if (playerMoviment == null)
        {
            playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
            if (playerMoviment != null)
            {
                playerParent = playerMoviment.transform.parent;
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

            if (playerMoviment != null)
            {
                if (playerMoviment.currentScene == "Altior-Fuga")
                {
                    instance.defaultPosition = new Vector2(-53f, 16.6f);
                    playerParent.transform.position = defaultPosition;
                }
                if (playerMoviment.currentScene == "DimensaoTempo")
                {
                    instance.defaultPosition = new Vector2(-81.6f, 26f);
                    playerParent.transform.position = defaultPosition;
                }
                if (playerMoviment.currentScene == "MontanhaIntro")
                {
                    instance.defaultPosition = new Vector2(-85.7f, -25.5f);
                    playerParent.transform.position = defaultPosition;
                }
                if (playerMoviment.currentScene == "Boss&NPC")
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
                if (playerMoviment != null)
                {
                    // Se o jogador estiver na mesma cena, carregar os dados salvos
                    if (saveData.currentScene == SceneManager.GetActiveScene().name)
                    {
                        playerMoviment.transform.position = saveData.playerCheckpoint;
                    }
                    else
                    {
                        playerMoviment.transform.position = defaultPosition;

                        // Se o jogador estiver em uma cena diferente, usar a posição padrão
                        if (playerParent != null)
                        {
                            Transform playerChild = playerMoviment.transform; // Obtém o filho do playerParent
                            playerChild.localPosition = Vector3.zero; // Ajusta a posição local do filho para (0,0,0)

                            // Move o playerParent para a posição atual (onde o player está)
                            playerParent.position = playerMoviment.transform.position;
                        }
                    }
                }
            }
        }
    }

    void Start()
    {
        //
        if (saveData.CameraCorrected)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.m_Lens.OrthographicSize = 7f;
        }
        else
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.m_Lens.OrthographicSize = 6f;
        }
    }

    void Update()
    {
        if (playerMoviment == null)
        {
            playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
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

    public void SaveCheckpoint(Vector2 checkpoint, int health, bool CameraCorrected, bool DashUnlocked, bool WalljumpUnlocked, bool JumpUnlocked, bool attackUnlocked, List<PowerUps> powerUps, int XPlayer = 0)
    {
        SaveData data = new SaveData
        {
            playerCheckpoint = checkpoint,
            playerHealth = health,
            currentScene = SceneManager.GetActiveScene().name,
            CameraCorrected = CameraCorrected,
            DashUnlocked = DashUnlocked,
            WalljumpUnlocked = WalljumpUnlocked,
            JumpUnlocked = JumpUnlocked,
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