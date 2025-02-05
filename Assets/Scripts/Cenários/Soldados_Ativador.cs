using System.Collections;
using Cinemachine;
using UnityEngine;

public class Soldados_Ativador : MonoBehaviour
{
    public bool isAtivado = false;
    public float timerAtivado = 0f;
    public float timerTargtAtivado;
    public Collider2D colisor;
    public PlayerMoviment playerMoviment;
    public Animator animator;

    [Header("Soldados")]
    public GameObject prefabGeneralMelee;
    public GameObject prefabGeneralLanceiro;
    public GameObject spawnSoldados;

    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Transform targetObject;
    public Vector3 localPosition;

    //
    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        colisor = GetComponent<Collider2D>();
        localPosition = targetObject.localPosition;
    }

    void Update()
    {
        if (isAtivado)
        {
            timerAtivado += Time.deltaTime;
            if (timerAtivado >= timerTargtAtivado)
            {
                Instantiate(prefabGeneralMelee, spawnSoldados.transform.position, Quaternion.identity);
                Instantiate(prefabGeneralLanceiro, spawnSoldados.transform.position, Quaternion.identity);

                isAtivado = false;
                StartCoroutine(AtivarSoldados());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAtivado = true;
        }
    }

    IEnumerator AtivarSoldados()
    {
        playerMoviment.canMove = false;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(localPosition.x * 2.2f, 1, cinemachineVirtualCamera.transform.position.z);
        colisor.enabled = false;

        yield return new WaitForSeconds(0.5f);
        animator = GameObject.FindFirstObjectByType<GoraflixMoviment>().GetComponent<Animator>();
        animator.SetBool("Soldados", true);

        yield return new WaitForSeconds(3.5f);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0, 0, 0);
        playerMoviment.canMove = true;
        Destroy(this.gameObject);
    }
}
