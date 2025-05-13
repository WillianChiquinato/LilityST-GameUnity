using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class interactableApresentation : CollidableObjects
{
    public SaveData saveData;

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
        grabPlayer = GameObject.FindFirstObjectByType<grabPlayer>();

        ApresInput.SetActive(false);
        saveData = SaveData.Instance;
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
                    if (GetInput == "Jump")
                    {
                        Time.timeScale = 1f;
                        playerMoviment.playerInput.enabled = true;
                        ApresInput.SetActive(false);

                        ativo = false;
                        Destroy(gameObject);
                    }

                    if (GetInput == "WallJump")
                    {
                        Time.timeScale = 1f;
                        playerMoviment.playerInput.enabled = true;
                        ApresInput.SetActive(false);

                        ativo = false;
                        Destroy(this.gameObject);
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (GetInput == "Dash")
                    {
                        goraflixMoviment = GameObject.FindFirstObjectByType<GoraflixMoviment>();
                        Time.timeScale = 1f;
                        ApresInput.SetActive(false);
                        goraflixMoviment.Anelgrab.SetActive(false);

                        ativo = false;
                        playerMoviment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                        playerMoviment.canMove = true;

                        playerMoviment.isDashing = true;
                        playerMoviment.animacao.SetBool(animationstrings.isDashing, true);
                        playerMoviment.timerDash = playerMoviment.dashCooldown;
                        playerMoviment.DamageScript.isInvicible = true;

                        playerMoviment.IsRight = false;
                        playerMoviment.rb.linearVelocity = new Vector2(playerMoviment.dashSpeed * -1, 0);
                        playerMoviment.rb.gravityScale = 0f;

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
            if (GetInput == "Jump" && !saveData.JumpUnlocked)
            {
                SaveData.Instance.JumpUnlocked = true;

                // Atualiza a referência local
                saveData.JumpUnlocked = true;

                ativo = true;
                Time.timeScale = 0f;
                ApresInput.SetActive(true);
                playerMoviment.playerInput.enabled = false;
                texto01.text = "Pressione";
                texto02.text = "Para pular";
                imagem.texture = referenciaImg;

                // Debug para verificar se a variável foi realmente alterada
                Debug.Log("Jump desbloqueado? " + SaveData.Instance.JumpUnlocked);
            }

            if (GetInput == "WallJump" && !saveData.WalljumpUnlocked)
            {
                SaveData.Instance.WalljumpUnlocked = true;

                // Atualiza a referência local
                saveData.WalljumpUnlocked = true;

                ativo = true;
                Time.timeScale = 0f;
                ApresInput.SetActive(true);
                playerMoviment.playerInput.enabled = false;
                texto01.text = "Vá na parede, press W";
                texto02.text = "Para WallJump";
                imagem.texture = referenciaImg;
            }

            if (GetInput == "Dash" && playerMoviment.grabAnim)
            {
                SaveData.Instance.DashUnlocked = true;

                // Atualiza a referência local
                saveData.DashUnlocked = true;
                StartCoroutine(StartDash());
            }
        }
    }

    IEnumerator StartDash()
    {
        yield return new WaitForSeconds(2.5f);

        ativo = true;
        Time.timeScale = 0f;
        ApresInput.SetActive(true);
        playerMoviment.playerInput.enabled = false;

        texto01.text = "Pressione SHIFT";
        texto02.text = "Para Dash";
        imagem.texture = referenciaImg;
    }
}
