using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sistema_Pause : MonoBehaviour
{
    [Header("Pause Instances")]
    private Damage playerDamage;

    public BossFight bossFight;
    public PlayerMoviment playerMoviment;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public GameObject CutSceneDroggo;
    public Damage playerHealth;
    private LevelTransicao transicao;

    public Dialogo_TriggerCervo dialogoCervo;

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
    public GameObject objectoSaveUI;
    


    void Awake()
    {
        UISavePoint = GameObject.FindGameObjectWithTag("SavePointUI");
        objectoSaveUI = GameObject.FindGameObjectWithTag("CheckpointUI");
        UISavePoint.SetActive(false);
        objectoSaveUI.SetActive(false);
        CutSumir = GameObject.FindGameObjectWithTag("Sumir");
        dialogoCervo = GameObject.FindFirstObjectByType<Dialogo_TriggerCervo>();

        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Start()
    {
        
        SistemaUI.SetActive(false);
        apresentaocao = GameObject.FindGameObjectsWithTag("Apresentacao");
        foreach (var obj in apresentaocao)
        {
            obj.SetActive(false);
        }
        pauseMenu.SetActive(false);

        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        playerDamage = playerMoviment.GetComponent<Damage>();
        playerHealth = playerMoviment.GetComponent<Damage>();
        bossFight = FindAnyObjectByType<BossFight>();

        CutSceneDroggo = GameObject.FindWithTag("CutScene");
        CutSceneDroggo.SetActive(false);
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

    public void AbrirHUDInventário()
    {
        Debug.Log("Abrir inventário");
    }

    public void ParaOMenu()
    {
        //Teste
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

    public void CervoDialog()
    {
        UISavePoint = GameObject.FindGameObjectWithTag("SavePointUI");
        UISavePoint.SetActive(false);
        if (dialogoCervo != null)
        {
            dialogoCervo.TriggerDialogo();
            dialogoCervo.animator.SetBool(animationstrings.InicioDialogo, true);
        }
    }

    public void Dimensoes()
    {
        Debug.Log("Dimensoes");
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
