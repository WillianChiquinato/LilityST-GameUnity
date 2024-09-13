using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow_Torax : MonoBehaviour
{
    public Camera cameraArco;
    public Bow bow;
    public PlayerMoviment playerMoviment;

    public Vector2 offset;
    [HideInInspector]
    public Vector3 Direcao;
    [SerializeField]
    private bool canRotate = true;
    public float angle;

    void Start()
    {
        cameraArco = FindObjectOfType<Camera>();
        bow = GameObject.FindObjectOfType<Bow>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
    }


    void Update()
    {
        //Teste do arco KRL DE ASAAAASSSS
        angle = Mathf.Atan2(Direcao.y, Direcao.x) * Mathf.Rad2Deg;

        if (angle < -10f)
        {
            canRotate = false;
            transform.rotation = Quaternion.Euler(0, 0, -10);
        }
        else
        {
            canRotate = true;
        }

        if (canRotate)
        {
            // Logica para arrumar o arco
            Vector3 mousePositionArco = cameraArco.ScreenToWorldPoint(Input.mousePosition);
            Vector2 BowPosition = transform.position;
            Vector2 mousePosition = new Vector2(mousePositionArco.x + offset.x, mousePositionArco.y + offset.y);

            Direcao = mousePosition - BowPosition;
            transform.right = Direcao;
        }
        else
        {
            // Logica para arrumar o arco
            Vector3 mousePositionArco = cameraArco.ScreenToWorldPoint(Input.mousePosition);
            Vector2 BowPosition = transform.position;
            Vector2 mousePosition = new Vector2(mousePositionArco.x + offset.x, mousePositionArco.y + offset.y);
            Direcao = mousePosition - BowPosition;
        }
    }
}
