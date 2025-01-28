using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class interactableApresentation : CollidableObjects
{
    public PlayerMoviment playerMoviment;
    public GoraflixMoviment goraflixMoviment;
    public grabPlayer grabPlayer;
    public GameObject ApresInput;

    public TextMeshProUGUI texto01;
    public TextMeshProUGUI texto02;
    public Texture referenciaImg;
    public RawImage imagem;

    public string GetInput;
    public bool ativo = false;
    public bool timerApres = false;

    protected override void Start()
    {
        base.Start();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        goraflixMoviment = GameObject.FindFirstObjectByType<GoraflixMoviment>();
        grabPlayer = GameObject.FindFirstObjectByType<grabPlayer>();

        ApresInput.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (ativo)
        {
            StartCoroutine(Countdown());
            if (timerApres)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (GetInput == "Jump" && !SavePoint.JumpApres)
                    {
                        SavePoint.JumpApres = true;
                        Time.timeScale = 1f;
                        playerMoviment.playerInput.enabled = true;
                        ApresInput.SetActive(false);
                        
                        ativo = false;
                        Destroy(gameObject);
                    }

                    if (GetInput == "WallJump" && !SavePoint.WallApres)
                    {
                        SavePoint.WallApres = true;
                        Time.timeScale = 1f;
                        playerMoviment.playerInput.enabled = true;
                        ApresInput.SetActive(false);

                        ativo = false;
                        Destroy(this.gameObject);
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (GetInput == "Dash" && !SavePoint.DashApres)
                    {
                        SavePoint.DashApres = true;
                        Time.timeScale = 1f;
                        ApresInput.SetActive(false);
                        goraflixMoviment.grab = false;

                        ativo = false;
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(0.6f);

        timerApres = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GetInput == "Jump" && !SavePoint.JumpApres)
            {
                ativo = true;
                Time.timeScale = 0f;
                ApresInput.SetActive(true);
                playerMoviment.playerInput.enabled = false;
                texto01.text = "Pressione";
                texto02.text = "Para pular";
                imagem.texture = referenciaImg;
            }

            if (GetInput == "WallJump" && !SavePoint.WallApres)
            {
                ativo = true;
                Time.timeScale = 0f;
                ApresInput.SetActive(true);
                playerMoviment.playerInput.enabled = false;
                texto01.text = "Vá na parede, press W";
                texto02.text = "Para WallJump";
                imagem.texture = referenciaImg;
            }

            if (GetInput == "Dash" && goraflixMoviment.playerSeguir && !SavePoint.DashApres)
            {
                StartCoroutine(StartDash());
            }
        }
    }

    IEnumerator StartDash()
    {
        yield return new WaitForSeconds(2f);

        ativo = true;
        Time.timeScale = 0f;
        ApresInput.SetActive(true);
        playerMoviment.playerInput.enabled = false;

        texto01.text = "Pressione SHIFT";
        texto02.text = "Para Dash";
        imagem.texture = referenciaImg;
    }
}
