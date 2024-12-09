using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameraFollowBaby : MonoBehaviour
{
    [Header("Field of View")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation")]
    [SerializeField] private float _flipRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PlayerBebe_Moviment playerBaby;
    private bool _isfacingRight;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField]
    public CinemachineFramingTransposer transposer;
    public Vector3 newOffset;

    void Start()
    {
        playerBaby = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        _playerTransform = playerBaby.GetComponentInChildren<Transform>();
        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        _isfacingRight = playerBaby.IsRight;
        transposer.m_TrackedObjectOffset = newOffset;
    }

    void Update()
    {
        transform.position = _playerTransform.position;
    }

    public void chamarTurn()
    {
        _turnCoroutine = StartCoroutine(flipXlerp());
    }

    public IEnumerator flipXlerp()
    {
        float startScaleX = transform.localScale.x;
        float endRotationAmount = DeterminarEndScaleX();
        float xScale = startScaleX;

        float startOffset = transposer.m_TrackedObjectOffset.x;
        float endOffset = DeterminarEndOffset().x;

        float elapsedTime = 0f;
        while (elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;
            xScale = Mathf.Lerp(startScaleX, endRotationAmount, elapsedTime / _flipRotationTime);

            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

            float currentOffsetX = Mathf.Lerp(startOffset, endOffset, elapsedTime / _flipRotationTime);
            transposer.m_TrackedObjectOffset = new Vector3(currentOffsetX, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);

            yield return null;
        }
    }

    private float DeterminarEndScaleX()
    {
        _isfacingRight = !_isfacingRight;
        return _isfacingRight ? 1f : -1f;
    }

    private Vector3 DeterminarEndOffset()
    {
        return _isfacingRight ? newOffset : -newOffset;
    }
}
