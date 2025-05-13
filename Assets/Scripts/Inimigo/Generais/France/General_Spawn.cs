using System.Collections;
using Cinemachine;
using UnityEngine;

public class General_Spawn : MonoBehaviour
{
    [Header("Instancias")]
    public GameObject spawnGeneral;
    public GameObject prefabGeneral;
    public GameObject prefabGeneralInstance = null;
    public grabPlayer grabPlayer;
    public GameObject grabPlayerPosition;
    public Collider2D colisor;
    public PlayerMoviment playerMoviment;
    public GoraflixMoviment goraflixMoviment;
    public vidroScript vidroScript;
    public Collider2D LiberarSalao;


    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;
    public Transform targetObject;


    [Header("Timers")]
    public float TimerSpawnGeneral = 0f;
    public bool SpawnGeneral;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        vidroScript = GameObject.FindFirstObjectByType<vidroScript>();
        colisor = GetComponent<Collider2D>();
        targetObject = transform.GetChild(0);
        colisor.enabled = true;

        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        if (SpawnGeneral)
        {
            TimerSpawnGeneral += Time.deltaTime;
            if (TimerSpawnGeneral >= 3f && targetObject != null)
            {
                playerMoviment.canMove = false;
                playerMoviment.IsRight = true;

                Vector3 diferrenca = targetObject.position - playerMoviment.transform.position;
                framingPosition.m_TrackedObjectOffset = new Vector3(diferrenca.x, diferrenca.y, 0);

                colisor.enabled = false;
                if (TimerSpawnGeneral >= 5.5f)
                {
                    if (prefabGeneralInstance == null)
                    {
                        prefabGeneralInstance = Instantiate(prefabGeneral, spawnGeneral.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        Debug.Log("General já instanciado");
                    }

                    goraflixMoviment = GameObject.FindFirstObjectByType<GoraflixMoviment>();
                    grabPlayer = goraflixMoviment.GetComponent<grabPlayer>();

                    if (TimerSpawnGeneral >= 10f)
                    {
                        goraflixMoviment.animator.SetBool("Grab", true);

                        if (goraflixMoviment.grabActived)
                        {
                            goraflixMoviment.Anelgrab.transform.position = playerMoviment.transform.position + new Vector3(0.5f, -1, 0);
                            goraflixMoviment.Anelgrab.SetActive(true);
                            playerMoviment.animacao.SetBool("Grab", true);
                            playerMoviment.transform.position = grabPlayerPosition.transform.position;
                            playerMoviment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                            framingPosition.m_TrackedObjectOffset = new Vector3(3, 0, 0);

                            //Libera o vidro para usar o IsDashing.
                            LiberarSalao.enabled = false;
                            vidroScript.GeneralActived = true;

                            if (playerMoviment.grabAtivo)
                            {
                                playerMoviment.grabAnim = true;
                                if (playerMoviment.animacao.GetCurrentAnimatorStateInfo(0).IsName("GrabPlayer") && playerMoviment.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                                {
                                    playerMoviment.animacao.SetBool("Grab", false);
                                    SpawnGeneral = false;
                                }
                            }
                        }
                    }
                }
            }

        }
        else
        {
            TimerSpawnGeneral = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.CompareTag("Player"))
        {
            SpawnGeneral = true;
        }
    }
}
