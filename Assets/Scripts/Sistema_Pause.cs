using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sistema_Pause : MonoBehaviour
{
    public GameObject prefabSpawn;

    public BossFight bossFight;
    public SavePoint savePoint;
    public PlayerMoviment playerMoviment;
    public DontDestroy[] dontDestroy;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public GameObject CutSceneDroggo;
    public Damage playerHealth;
    public bool IsPaused;
    private LevelTransicao transicao;
    public string sceneName;
    public bool IrMenu = false;

    //Savepoint
    public bool IrRestart = false;
    public string CurrentSceneName { get; private set; }

    void Start()
    {
        pauseMenu.SetActive(false);
        transicao = GameObject.FindObjectOfType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        playerHealth = playerMoviment.GetComponent<Damage>();
        dontDestroy = GameObject.FindObjectsOfType<DontDestroy>();
        savePoint = GameObject.FindObjectOfType<SavePoint>();
        bossFight = FindAnyObjectByType<BossFight>();

        CutSceneDroggo = GameObject.FindWithTag("CutScene");
        CutSceneDroggo.SetActive(false);

        CurrentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (bossFight.SceneDroggo == true)
        {
            CutSceneDroggo.SetActive(true);
        }
        else
        {
            CutSceneDroggo.SetActive(false);
        }

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

    public void ParaOMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
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

    public IEnumerator TempoMorte()
    {
        yield return new WaitForSeconds(2f);

        IrRestart = true;
        SceneManager.LoadScene(CurrentSceneName);
    }
}
