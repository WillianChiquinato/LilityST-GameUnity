using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SeguirMouse : MonoBehaviour
{
    public Camera cameraMouse;
    public Bow bow;

    void Start()
    {
        cameraMouse = FindObjectOfType<Camera>();
        bow = GameObject.FindObjectOfType<Bow>();
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
