using Cinemachine;
using UnityEngine;

public class TriggerCameraAjustes : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;

    void Start()
    {
        cinemachineVirtualCamera = GameManager.instance.cinemachineVirtualCamera;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.isCameraCorrected = false;
            cinemachineVirtualCamera.m_Lens.OrthographicSize = 6f;
        }
    }
}
