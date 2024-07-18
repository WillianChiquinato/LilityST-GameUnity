using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirMouse : MonoBehaviour
{
    public float zOffset = -10f; // Deslocamento no eixo Z (caso 2D)
    public Camera cameraMouse;

    void Start()
    {
        cameraMouse = FindObjectOfType<Camera>();
    }
    void Update()
    {
        // Obter a posição do mouse em coordenadas de tela
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Converter a posição da tela para a posição do mundo
        Vector3 mouseWorldPosition = cameraMouse.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));

        // Atualizar a posição do GameObject para a posição do mouse
        transform.position = mouseWorldPosition;
    }
}
