using System.Collections;
using Cinemachine;
using UnityEngine;

public class Soldados_Ativador : MonoBehaviour
{
    [Header("Ativacao Soldados")]
    public bool isAtivado = false;
    public float timerAtivado = 0f;
    public float timerTargtAtivado;
    public float timerAtivadoSoldados = 0f;
    public float timerTargetAtivadoSoldados;

    public float CameraTransitionValue;

    [Header("Instances")]
    public Collider2D colisor;
    public PlayerMoviment playerMoviment;
    public GoraflixMoviment goraflixMoviment;
    public Animator animator;

    [Header("Soldados")]
    public GameObject prefabGeneralMelee;
    public GameObject prefabGeneralLanceiro;
    public GameObject spawnSoldados;
    public GameObject passagemOpen;

    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;
    public Transform targetObject;
    public Vector3 localPosition;

    void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        colisor = GetComponent<Collider2D>();
        localPosition = targetObject.localPosition;

        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        if (isAtivado)
        {
            timerAtivado += Time.deltaTime;
            if (timerAtivado >= timerTargtAtivado)
            {
                timerAtivadoSoldados += Time.deltaTime;
                if (timerAtivadoSoldados >= timerTargetAtivadoSoldados)
                {
                    if (prefabGeneralLanceiro != null && prefabGeneralMelee != null)
                    {
                        Instantiate(prefabGeneralMelee, spawnSoldados.transform.position, Quaternion.identity);
                        Instantiate(prefabGeneralLanceiro, spawnSoldados.transform.position, Quaternion.identity);
                    }
                    isAtivado = false;
                }

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
        Vector3 diferrenca = targetObject.position - playerMoviment.transform.position;

        framingPosition.m_TrackedObjectOffset = new Vector3(diferrenca.x, diferrenca.y, 0);

        colisor.enabled = false;

        yield return new WaitForSeconds(0.5f);
        goraflixMoviment = GameObject.FindFirstObjectByType<GoraflixMoviment>();
        animator = goraflixMoviment.GetComponent<Animator>();
        animator.SetBool("Soldados", true);

        yield return new WaitForSeconds(2f);
        if (goraflixMoviment.Target)
        {
            animator.SetBool("Lanca", true);
            if (passagemOpen != null)
            {
                yield return new WaitForSeconds(0.7f);
                Destroy(passagemOpen);
            }
            else
            {
                Debug.Log("Passagem Ainda nao ativa");
            }
        }

        yield return new WaitForSeconds(CameraTransitionValue);

        framingPosition.m_TrackedObjectOffset = new Vector3(0, 0, 0);
        playerMoviment.canMove = true;
        Destroy(this.gameObject);
    }
}
