using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sistema_Pause : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public GameObject pauseMenu;
    public GameObject MainCamera;
    public bool IsPaused;
    private LevelTransicao transicao;
    public string sceneName;
    public bool IrMenu = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        transicao = GameObject.FindObjectOfType<LevelTransicao>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
    }

    void Update()
    {
        if (playerMoviment.IsAlive == false)
        {
            StartCoroutine(TempoMorte());
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

    public IEnumerator TempoMorte()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(sceneName);
        IrMenu = true;
    }
}
