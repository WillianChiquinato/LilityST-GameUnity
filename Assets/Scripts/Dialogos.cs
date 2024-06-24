using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogos : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Vector3 offset;
    public Transform player;

    void Awake()
    {
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
        player = playerMoviment.GetComponent<Transform>();
    }

    void Update()
    {
         // Atualiza a posição da caixa de diálogo para seguir o player com o deslocamento
        transform.position = player.position + offset;

        // Define a rotação da caixa de diálogo como constante (zero rotação)
        transform.rotation = Quaternion.identity;
    }
}
