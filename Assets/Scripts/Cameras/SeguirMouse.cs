using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SeguirMouse : MonoBehaviour
{
    public Camera cameraMouse;

    void Start()
    {
        cameraMouse = FindFirstObjectByType<Camera>();
    }
    
    void Update()
    {
        Vector3 mousePosition = cameraMouse.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
