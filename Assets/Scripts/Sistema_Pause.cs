using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sistema_Pause : MonoBehaviour
{
    public static Sistema_Pause instance { get; private set; }

    [Header("Quests")]
    public QuestEvents questEvents;

    [Header("Pause Instances")]
    public UI pauseUI;
    public BossFight bossFight;
    public PlayerMoviment playerMoviment;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public GameObject CutSceneDroggo;
    public Damage playerHealth;
    private LevelTransicao transicao;

    public Dialogo_Trigger dialogoCervo;

    [Header("Player")]
    public PlayerMoviment player;
    public int XpPlayer = 0;
    // public GameObject[] checkpoints;

    [Header("Variaveis")]
    public bool IsPaused;
    public string sceneName;
    public bool IrMenu = false;

    [Header("Apresentacao")]
    public GameObject[] apresentaocao;
    public GameObject SistemaUI;
    public GameObject CutSumir;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;

    [Header("Savepoint")]
    //Savepoint
    public string CurrentSceneName { get; private set; }
    public GameObject UISavePoint;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            questEvents = new QuestEvents();
        }
        else
        {
            Destroy(gameObject);
        }

        pauseUI = GameObject.FindFirstObjectByType<UI>();
        UISavePoint = GameObject.FindGameObjectWithTag("SavePointUI");
        UISavePoint.SetActive(false);
        CutSumir = GameObject.FindGameObjectWithTag("Sumir");

        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerMoviment>();
        apresentaocao = GameObject.FindGameObjectsWithTag("Apresentacao");

        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        playerHealth = playerMoviment.GetComponent<Damage>();
        bossFight = FindAnyObjectByType<BossFight>();

        CutSceneDroggo = GameObject.FindWithTag("CutScene");
        CutSceneDroggo.SetActive(false);
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.1f);

        SistemaUI.SetActive(false);
        foreach (var obj in apresentaocao)
        {
            obj.SetActive(false);
        }
        pauseMenu.SetActive(false);
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
        pauseUI.transform.GetChild(1).gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void AbrirHUD()
    {
        SistemaUI.SetActive(true);
    }

    public void AbrirHUDInventário()
    {
        Debug.Log("Abrir inventário");
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

    public void ExitCheckpoint()
    {
        Debug.Log("Saindo do checkpoint");

        CutSumir.SetActive(true);
        framingPosition.m_TrackedObjectOffset = new Vector3(0, 0, 0);

        UISavePoint.SetActive(false);
        playerMoviment.animacao.SetBool("Checkpoint", false);
        playerMoviment.canMove = true;
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

    public event Action<int> onPlayerChange;
    public void PlayerChange(int level)
    {
        level = 0;
        QuestPoint questAtual = FindFirstObjectByType<QuestPoint>();
        // if (onPlayerChange != null && questAtual.questInfopoint.tipoMissao.Equals("CriaturaFolclorica"))
        // {
        //     onPlayerChange(level);
        // }
    }

    public void AddXP(int xp)
    {
        XpPlayer += xp;
        SaveData.Instance.XPlayer += xp;
    }
}
