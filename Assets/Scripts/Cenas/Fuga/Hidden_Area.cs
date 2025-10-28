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

    public CinemachineVirtualCamera virtualCamera;
    public Camera cameras;
    public float duration = 1f;

    public int indexToOpacity = 0;

    void Awake()
    {
        virtualCamera.m_Lens.OrthographicSize = 6f;
        cameras = FindFirstObjectByType<Camera>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (indexToOpacity == 0)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(AnimateOrthographicSize(6f, 7f, duration));
            }
        }

        if (alreadyEntered)
            return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag))
            return;

        onTriggerEnter?.Invoke();

        if (oneShot)
        {
            alreadyEntered = true;
        }

        Destroy(this.gameObject, 2f);
    }

    IEnumerator AnimateOrthographicSize(float startSize, float endSize, float duration)
    {
        float timeElapsed = 0f;

        cameras.orthographic = true;
        var lens = virtualCamera.m_Lens;

        // Definir o valor inicial do OrthographicSize
        lens.OrthographicSize = startSize;
        virtualCamera.m_Lens = lens;

        // Interpolação suave do OrthographicSize durante o tempo
        while (timeElapsed < duration)
        {
            lens.OrthographicSize = Mathf.Lerp(startSize, endSize, timeElapsed / duration);
            virtualCamera.m_Lens = lens;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        lens.OrthographicSize = endSize;
        virtualCamera.m_Lens = lens;
    }
}
