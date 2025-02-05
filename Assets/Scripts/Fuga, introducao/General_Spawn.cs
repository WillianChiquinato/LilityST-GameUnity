using System.Collections;
using Cinemachine;
using UnityEngine;

public class General_Spawn : MonoBehaviour
{
    [Header("Instancias")]
    public GameObject spawnGeneral;
    public GameObject prefabGeneral;
    public Collider2D colisor;
    public PlayerMoviment playerMoviment;
    public vidroScript vidroScript;
    public Collider2D LiberarSalao;



    public GameObject playerTESTE;



    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Transform targetObject;
    public Vector3 localPosition;


    [Header("Timers")]
    public float TimerSpawnGeneral = 0f;
    public bool SpawnGeneral;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        vidroScript = GameObject.FindFirstObjectByType<vidroScript>();
        colisor = GetComponent<Collider2D>();
        targetObject = transform.GetChild(0);
        localPosition = targetObject.localPosition;
        colisor.enabled = true;

        Debug.Log(localPosition);
    }

    void Update()
    {
        if (SpawnGeneral)
        {
            TimerSpawnGeneral += Time.deltaTime;
            if (TimerSpawnGeneral >= 3f && targetObject != null)
            {
                playerMoviment.canMove = false;
                playerMoviment.transform.localScale = new Vector3(1, 1, 1);
                playerMoviment.transform.position = new Vector3(playerTESTE.transform.position.x, playerMoviment.transform.position.y, playerMoviment.transform.position.z);
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(localPosition.x, 1, cinemachineVirtualCamera.transform.position.z);
                colisor.enabled = false;
                if (TimerSpawnGeneral >= 5.5f)
                {
                    Instantiate(prefabGeneral, spawnGeneral.transform.position, Quaternion.identity);
                    SpawnGeneral = false;
                    LiberarSalao.enabled = false;
                    vidroScript.GeneralActived = true;
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
