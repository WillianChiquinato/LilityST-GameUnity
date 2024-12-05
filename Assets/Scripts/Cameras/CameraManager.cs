using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private Coroutine _pancameraContact;
    private Vector2 _startTrackedObject;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer transposer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        _startTrackedObject = transposer.m_TrackedObjectOffset;
    }

    public void PanCameraContact(float panDistance, float panTime, PanDirecao panDirecao, bool panToStarting)
    {
        _pancameraContact = StartCoroutine(PanCamera(panDistance, panTime, panDirecao, panToStarting));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirecao panDirecao, bool panToStarting)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStarting)
        {
            switch (panDirecao)
            {
                case PanDirecao.Up:
                    endPos = Vector2.up;
                    break;

                case PanDirecao.Down:
                    endPos = Vector2.down;
                    break;

                case PanDirecao.Left:
                    endPos = Vector2.left;
                    break;

                case PanDirecao.Right:
                    endPos = Vector2.right;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            startingPos = _startTrackedObject;

            endPos += startingPos;
        }
        else
        {
            startingPos = transposer.m_TrackedObjectOffset;
            endPos = _startTrackedObject;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, elapsedTime / panTime);
            transposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }
}
