using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirMouse : MonoBehaviour
{
    public Camera cameraMouse;

    void Start()
    {
        cameraMouse = FindObjectOfType<Camera>();
    }
    void Update()
    {
        // Converter a posição da tela para a posição do mundo
        Vector3 mouseWorldPosition = cameraMouse.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        // Atualizar a posição do GameObject para a posição do mouse
        transform.position = mouseWorldPosition;
    }
}
