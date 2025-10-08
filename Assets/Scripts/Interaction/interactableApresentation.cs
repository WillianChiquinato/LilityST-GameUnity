using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;

public class interactableApresentation : CollidableObjects
{
    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;


    [Header("Referencias")]
    public PlayerMoviment playerMoviment;
    public bool playerIsDeath = false;

    public FranceMoviment goraflixMoviment;
    public grabPlayer grabPlayer;

    public SpriteRenderer imagem;
    public TextMeshPro textContagem;
    public float startTime = 4f;
    private float currentTime;

    public string GetInput;
    public bool ativo = false;
    public bool timerApres = false;

    protected override void Start()
    {
        base.Start();

        currentTime = startTime;
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        grabPlayer = GameObject.FindFirstObjectByType<grabPlayer>();

        imagem = GetComponentInChildren<SpriteRenderer>();
        imagem.gameObject.SetActive(false);
        if (GetInput == "Dash")
        {
            textContagem = GetComponentInChildren<TextMeshPro>();
            textContagem.gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (ativo)
        {
            StartCoroutine(Countdown());
            if (timerApres)
            {
                if (GetInput == "Pedrinha")
                {
                    imagem.gameObject.SetActive(true);

                    if (playerMoviment.GetComponentInChildren<Throw_Item>().arremessar)
                    {
                        if (GetInput == "Pedrinha")
                        {
                            ativo = false;
                            imagem.GetComponent<Animator>().SetBool("Desativar", true);
                            Destroy(this.gameObject, 1.5f);
                        }
                    }
                }

                if (GetInput == "WallJump")
                {
                    imagem.gameObject.SetActive(true);
                    playerMoviment.RunTiming = 0f;
                    playerMoviment.accelerationTimer = 0f;
                    playerMoviment.airSpeed = 7f;
                    playerMoviment.maxSpeed = 7f;
                    playerMoviment.IsRunning = false;

                    if (playerMoviment.isWallJumping)
                    {
                        if (GetInput == "WallJump")
                        {
                            ativo = false;
                            imagem.GetComponent<Animator>().SetBool("Desativar", true);
                            Destroy(this.gameObject, 1.5f);
                        }
                    }
                }
                if (GetInput == "Dash")
                {
                    goraflixMoviment = GameObject.FindFirstObjectByType<FranceMoviment>();

                    imagem.gameObject.SetActive(true);
                    textContagem.gameObject.SetActive(true);
                    if (currentTime >= 0f)
                    {
                        currentTime -= Time.deltaTime;
                    }

                    if (currentTime < 0f && !playerIsDeath)
                    {
                        currentTime = 0f;
                        textContagem.text = "00:00";
                        StartCoroutine(ZoomBackCoroutine());
                        imagem.GetComponent<Animator>().SetBool("Desativar", true);

                        goraflixMoviment.Anelgrab.GetComponent<Animator>().SetTrigger("Death");
                        goraflixMoviment.animator.SetBool("isKilling", true);
                        playerMoviment.DamageScript.IsAlive = false;
                        playerMoviment.OnHit(1, new Vector2(0, 0));
                        playerIsDeath = true;
                    }
                    else
                    {
                        int seconds = Mathf.FloorToInt(currentTime);
                        int milliseconds = Mathf.FloorToInt((currentTime - seconds) * 1000f);

                        textContagem.text = $"{seconds:00}:{milliseconds:00}";
                        float t = 1f - (currentTime / startTime);

                        //Escala do anel do general.
                        Vector3 startScale = Vector3.one;
                        Vector3 endScale = Vector3.one * 0.85f;
                        goraflixMoviment.Anelgrab.transform.localScale = Vector3.Lerp(startScale, endScale, t);

                        //Escala camera.
                        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, 6.4f, t);
                    }

                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        StopAllCoroutines();
                        StartCoroutine(ZoomBackCoroutine());
                        playerIsDeath = true;
                        currentTime = 0f;
                        textContagem.text = "00:00";

                        goraflixMoviment.Anelgrab.GetComponent<Animator>().SetTrigger("Death");
                        Destroy(goraflixMoviment.Anelgrab.gameObject, 1.5f);

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

                        imagem.GetComponent<Animator>().SetBool("Desativar", true);
                        textContagem.gameObject.SetActive(false);
                        Destroy(this.gameObject, 1.5f);
                    }
                }
            }
        }
    }

    public IEnumerator ZoomBackCoroutine()
    {
        float startSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        float targetSize = 7f;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cinemachineVirtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }
        cinemachineVirtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        timerApres = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GetInput == "Pedrinha")
            {
                ativo = true;
            }

            if (GetInput == "WallJump" && !SaveData.Instance.WalljumpUnlocked)
            {
                SaveData.Instance.WalljumpUnlocked = true;
                ativo = true;
            }

            if (GetInput == "Dash" && playerMoviment.grabAnim)
            {
                SaveData.Instance.DashUnlocked = true;
                StartCoroutine(StartDash());
            }
        }
    }

    IEnumerator StartDash()
    {
        yield return new WaitForSeconds(1.5f);

        ativo = true;
        // Exibir icone dash
    }
}
