using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static int currentSaveSlot { get; set; }

    public Material FullScreenDamageMaterial;

    [Header("Quests")]
    public QuestEvents questEvents;

    [Header("Player")]
    public PlayerMoviment player;
    public int XpPlayer = 0;


    [Header("Pause Instances")]
    public UI pauseUI;
    public GameObject GUI;
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
    public bool cervinhoOnCheckpoint = false;

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
        Debug.LogWarning("GameManager: " + currentSaveSlot);

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

        UISavePoint.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
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

        yield return new WaitForSeconds(1f);
        cinemachineVirtualCamera.gameObject.GetComponent<CinemachineConfiner2D>().InvalidateCache();
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
        // ToastMessage.Instance.activeToast.gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void AbrirHUD()
    {
        SistemaUI.SetActive(true);
    }

    public void FecharHUD()
    {
        if (ToastMessage.Instance.activeToast != null)
        {
            ToastMessage.Instance.activeToast.gameObject.SetActive(false);
        }
        SistemaUI.SetActive(false);
        StartCoroutine(FadeInCanvasGroup(GUI.GetComponent<CanvasGroup>(), 1.2f));
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

    //Canvas group anims.
    public IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, time / duration);
            yield return null;
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    //Parte de reiniciar
    public IEnumerator TempoMorte()
    {
        if (SceneManager.GetSceneByName("Mapa").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Mapa");
        }
        yield return new WaitForSeconds(2f);

        transicao.Transicao(player.currentScene);
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

    public void TriggerNoUseArgument(string[] message)
    {
        foreach (string msg in message)
        {
            switch (msg)
            {
                case "Dash":
                    playerMoviment.isDashing = false;
                    playerMoviment.timerDash = 1f;
                    break;
                case "WallJump":
                    player.wallSlide = false;
                    break;

                case "Run":
                    player.RunTiming = 0f;
                    player.IsRunning = false;
                    break;
                default:
                    Debug.LogWarning("TriggerNoArgument: Mensagem não reconhecida - " + message);
                    break;
            }
        }
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        FullScreenDamageMaterial.SetFloat("_IsPulseActive", 0);
    }
#endif
}
