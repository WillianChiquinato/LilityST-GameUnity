using UnityEngine;

public class MapCameraZoom : MonoBehaviour
{
    public Camera mapCamera;
    public float zoomSpeed = 5f;
    public float startZoom = 11f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    void Start()
    {
        mapCamera = GetComponent<Camera>();

        mapCamera.orthographicSize = startZoom;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.01f) return;

        mapCamera.orthographicSize -= scroll * zoomSpeed;
        mapCamera.orthographicSize = Mathf.Clamp(
            mapCamera.orthographicSize,
            minZoom,
            maxZoom
        );
    }
}