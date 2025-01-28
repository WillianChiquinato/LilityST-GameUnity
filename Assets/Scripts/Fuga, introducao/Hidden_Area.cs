using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Hidden_Area : MonoBehaviour
{
    public bool oneShot = false;
    private bool alreadyEntered = false;

    private string collisionTag = "Player";
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    public Animator animator;
    public PolygonCollider2D polygonCollider2D;
    public CinemachineConfiner2D CinemachineRecalculo;
    public CinemachineVirtualCamera virtualCamera;
    public Camera cameras;
    public float duration = 1f;

    private static bool isObjectDestroyed = false;

    void Awake()
    {
        cameras = FindFirstObjectByType<Camera>();

        if (isObjectDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AnimateOrthographicSize(6f, 7f, duration));
            isObjectDestroyed = true;
            Debug.Log("Entrou");
        }

        if (alreadyEntered)
            return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag))
            return;

        onTriggerEnter?.Invoke();

        if (oneShot)
            alreadyEntered = true;

        Destroy(this.gameObject, 2f);
    }

    IEnumerator AnimateOrthographicSize(float startSize, float endSize, float duration)
    {
        float timeElapsed = 0f;

        // Garantir que a câmera esteja em modo ortográfico
        cameras.orthographic = true;

        // Obter o componente de lente da Cinemachine
        var lens = virtualCamera.m_Lens;

        // Definir o valor inicial do OrthographicSize
        lens.OrthographicSize = startSize;
        virtualCamera.m_Lens = lens;

        // Interpolação suave do OrthographicSize durante o tempo
        while (timeElapsed < duration)
        {
            // Interpolando entre o tamanho inicial e final
            lens.OrthographicSize = Mathf.Lerp(startSize, endSize, timeElapsed / duration);

            // Atualizar o componente de lente da câmera
            virtualCamera.m_Lens = lens;

            // Aumentar o tempo decorrido
            timeElapsed += Time.deltaTime;
            yield return null;  // Esperar o próximo frame
        }

        // Garantir que o OrthographicSize final seja exatamente o alvo
        lens.OrthographicSize = endSize;
        virtualCamera.m_Lens = lens;
    }
}
