using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSet_PedrinhaAttack : MonoBehaviour
{

    public Transform player;
    public Maga_Movement maga_Movement;
    private float tempoAtraso = 0.5f;
    private float velocidade = 0.5f;
    private float velocidadeInitial = 1.0f;
    public bool subindo = false;

    void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().GetComponentInChildren<Transform>();
        maga_Movement = GameObject.FindFirstObjectByType<Maga_Movement>();

        Invoke("MudarDirecao", tempoAtraso);
    }

    void Update()
    {
        if (maga_Movement.damageScript.IsAlive)
        {
            if (subindo)
            {
                // Mover o objeto para cima
                transform.position += Vector3.up * velocidade * Time.deltaTime;
            }
            else
            {
                // Mover o objeto para baixo
                transform.position += Vector3.down * velocidadeInitial * Time.deltaTime;
            }
        }
    }

    void MudarDirecao()
    {
        // Após 0.2 segundos, inverte a direção (começa a subir)
        subindo = true;
    }
}
