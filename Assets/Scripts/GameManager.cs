using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Quests")]
    public QuestEvents questEvents;

    [Header("Player")]
    public PlayerMoviment player;
    public int XpPlayer = 0;


    [Header("Pause Instances")]
    public UI pauseUI;
    public BossFight bossFight;
    public PlayerMoviment playerMoviment;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public Damage playerHealth;
    private LevelTransicao transicao;

    public Dialogo_Trigger dialogoCervo;

    [Header("Variaveis")]
    public bool IsPaused;
    public string sceneName;
    public bool IrMenu = false;

    [Header("Apresentacao")]
    public GameObject[] apresentaocao;
    public GameObject SistemaUI;
    public GameObject CutSumir;

    [Header("Cinemachine")]
    public bool isCameraCorrected = true;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;

    public ShakeCamera shakeCamera;

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
        shakeCamera = cinemachineVirtualCamera.GetComponent<ShakeCamera>();
        player = FindFirstObjectByType<PlayerMoviment>();
    }

    void Start()
    {
        DOTween.Init();
        apresentaocao = GameObject.FindGameObjectsWithTag("Apresentacao");

        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        playerHealth = playerMoviment.GetComponent<Damage>();
        bossFight = FindAnyObjectByType<BossFight>();


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

        if (isCameraCorrected)
        {
           cinemachineVirtualCamera.m_Lens.OrthographicSize = 7f; 
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
        pauseMenu.transform.GetChild(0).gameObject.SetActive(true);
        ToastMessage.Instance.activeToast.gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void AbrirHUD()
    {
        SistemaUI.SetActive(true);
        pauseMenu.transform.GetChild(0).gameObject.SetActive(false);
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
