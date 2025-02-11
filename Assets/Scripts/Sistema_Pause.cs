using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sistema_Pause : MonoBehaviour
{
    public GameObject[] objs;
    private Damage playerDamage;

    public BossFight bossFight;
    public SavePoint savePoint;
    public PlayerMoviment playerMoviment;
    public FadeStartGame fadeStartGame;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public GameObject CutSceneDroggo;
    public Damage playerHealth;
    public bool IsPaused;
    private LevelTransicao transicao;
    public string sceneName;
    public bool IrMenu = false;

    public GameObject[] apresentaocao;
    public GameObject SistemaUI;

    [Header("Savepoint")]
    //Savepoint
    public string CurrentSceneName { get; private set; }

    void Start()
    {
        SistemaUI.SetActive(false);
        apresentaocao = GameObject.FindGameObjectsWithTag("Apresentacao");
        foreach (var obj in apresentaocao)
        {
            obj.SetActive(false);
        }
        pauseMenu.SetActive(false);

        // Verifica se já existe uma instância desse objeto na cena
        objs = GameObject.FindGameObjectsWithTag("DontDestroy");

        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        playerDamage = playerMoviment.GetComponent<Damage>();
        playerHealth = playerMoviment.GetComponent<Damage>();
        bossFight = FindAnyObjectByType<BossFight>();

        CutSceneDroggo = GameObject.FindWithTag("CutScene");
        CutSceneDroggo.SetActive(false);

        //Cenas modificação de spawn
        if (SavePoint.nomeCenaMenu == "Altior-Fuga" && !SavePoint.CheckpointAnim)
        {
            SavePoint.CheckpointPosition = new Vector2(-53.93f, 16.6f);
            playerMoviment.transform.position = SavePoint.CheckpointPosition;
        }
    }

    void Update()
    {
        if (playerMoviment.IsAlive == false)
        {
            StartCoroutine(TempoMorte());
            playerMoviment.rb.sharedMaterial = null;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        playerMoviment.playerInput.enabled = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        playerMoviment.playerInput.enabled = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void AbrirHUD()
    {
        SistemaUI.SetActive(true);
    }

    public void ParaOMenu()
    {
        //Teste
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        Debug.Log("Cena: " + SavePoint.nomeCenaMenu);
        IrMenu = true;
    }

    public void Exit()
    {
        Application.Quit();
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
        //Atualiza o nome da cena atual quando uma nova cena é carregada
        CurrentSceneName = scene.name;
    }



    //Parte de reiniciar
    public IEnumerator TempoMorte()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(CurrentSceneName);
    }
}
