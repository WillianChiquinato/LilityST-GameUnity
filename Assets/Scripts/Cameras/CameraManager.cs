using UnityEngine;
using Cinemachine;
using System.Collections;

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

        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        transposer.m_TrackedObjectOffset = new Vector3(0, 0, 0);
        _startTrackedObject = transposer.m_TrackedObjectOffset;
    }

    public void PanCameraContact(Vector2 panDistance, float panTime, PanDirecao panDirecao, bool panToStarting)
    {
        if (_pancameraContact != null)
            StopCoroutine(_pancameraContact);

        _pancameraContact = StartCoroutine(PanCamera(panDistance, panTime, panDirecao, panToStarting));
    }

    private IEnumerator PanCamera(Vector2 panDistance, float panTime, PanDirecao panDirecao, bool panToStarting)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStarting)
        {
            // define direção base
            switch (panDirecao)
            {
                case PanDirecao.Up:
                    endPos = new Vector2(0, panDistance.y);
                    break;

                case PanDirecao.Down:
                    endPos = new Vector2(0, -panDistance.y);
                    break;

                case PanDirecao.Left:
                    endPos = new Vector2(-panDistance.x, 0);
                    break;

                case PanDirecao.Right:
                    endPos = new Vector2(panDistance.x, 0);
                    break;

                case PanDirecao.UpRight:
                    endPos = new Vector2(panDistance.x, panDistance.y);
                    break;

                case PanDirecao.UpLeft:
                    endPos = new Vector2(-panDistance.x, panDistance.y);
                    break;

                case PanDirecao.DownRight:
                    endPos = new Vector2(panDistance.x, -panDistance.y);
                    break;

                case PanDirecao.DownLeft:
                    endPos = new Vector2(-panDistance.x, -panDistance.y);
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
