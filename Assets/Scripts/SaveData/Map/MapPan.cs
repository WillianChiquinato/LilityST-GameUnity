using UnityEngine;

public class MapPan : MonoBehaviour
{
    public float panSpeed = 1f;

    private Camera cam;
    private Vector3 lastMouseWorld;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        if (Input.GetMouseButtonDown(0))
        {
            lastMouseWorld = mouseWorld;
            return;
        }

        Vector3 delta = lastMouseWorld - mouseWorld;
        transform.position += delta * panSpeed;

        lastMouseWorld = mouseWorld;
    }
}
